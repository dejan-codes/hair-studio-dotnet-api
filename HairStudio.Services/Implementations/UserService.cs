using System.Security.Cryptography;
using HairStudio.Model.Models;
using HairStudio.Repository.Interfaces;
using HairStudio.Services.DTOs.Users;
using HairStudio.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using HairStudio.Services.Common;
using HairStudio.Services.Errors;
using HairStudio.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using HairStudio.Services.Audit;
using HairStudio.Services.Enums;

namespace HairStudio.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly JwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IEmailConfirmationRepository _emailConfirmationRepository;
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(JwtService jwtService, IEmailService emailService, IMessageRepository messageRepository, IUserRepository userRepository, IRoleRepository roleRepository, IEmailConfirmationRepository emailConfirmationRepository, IPasswordResetTokenRepository passwordResetTokenRepository, IConfiguration configuration)
        {
            _jwtService = jwtService;
            _emailService = emailService;
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _emailConfirmationRepository = emailConfirmationRepository;
            _passwordResetTokenRepository = passwordResetTokenRepository;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        [Auditable("REGISTER_USER")]
        public async Task<Result> RegisterUserAsync(UserRegistrationDTO userRegistrationDTO)
        {
            var existingUser = await _userRepository.GetByEmailAsync(userRegistrationDTO.Email);
            if (existingUser != null)
                return Result.Failure(UserErrors.UserExists);

            var user = new User
            {
                FirstName = userRegistrationDTO.FirstName,
                LastName = userRegistrationDTO.LastName,
                Email = userRegistrationDTO.Email,
                PhoneNumber = userRegistrationDTO.PhoneNumber,
                EmailConfirmed = false
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, userRegistrationDTO.Password);

            var tokenBytes = RandomNumberGenerator.GetBytes(32);
            var confirmationCode = WebEncoders.Base64UrlEncode(tokenBytes);

            var emailConfirmation = new EmailConfirmation
            {
                EmailConfirmationId = Guid.NewGuid(),
                User = user,
                ConfirmationCode = confirmationCode,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            await _emailConfirmationRepository.AddAndSaveAsync(emailConfirmation);
            var confirmationLink = $"{_configuration["FrontendUrl"]}/confirm-email?code={confirmationCode}";
            await _emailService.SendEmailAsync(user.Email, "Confirm Your Email", confirmationLink);

            return Result.Success();
        }

        [Auditable("CONFIRM_USER_EMAIL")]
        public async Task<Result> ConfirmUserEmailAsync(string code)
        {
            var confirmation = await _emailConfirmationRepository.GetByCodeAsync(code);

            if (confirmation == null || confirmation.ExpiresAt < DateTime.UtcNow)
                return Result.Failure(UserErrors.InvalidConfirmationCode);

            confirmation.User.EmailConfirmed = true;
            confirmation.User.IsActive = true;

            var userRole = await _roleRepository.GetByIdAsync((short)RoleEnum.User);
            if(userRole != null)
                confirmation.User.Roles.Add(userRole);

            await _userRepository.ExecuteInTransactionAsync(() =>
            {
                _userRepository.Update(confirmation.User);
                _emailConfirmationRepository.Remove(confirmation);
                return Task.CompletedTask;
            });

            return Result.Success();
        }

        [Auditable("USER_PASSWORD_RESET")]
        public async Task<Result> UserPasswordResetAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                return Result.Failure(UserErrors.EmailNotFound);

            var resetToken = Guid.NewGuid().ToString();

            var passwordResetToken = new PasswordResetToken
            {
                UserId = user.UserId,
                ResetToken = resetToken,
                ExpiryDate = DateTime.UtcNow.AddMinutes(120)
            };

            await _passwordResetTokenRepository.AddAndSaveAsync(passwordResetToken);

            await _emailService.SendEmailAsync(
                user.Email,
                "Password reset",
                $"New password is: {resetToken}. After logging in, it is advisable to reset password."
            );

            return Result.Success();
        }

        [Auditable("LOGIN")]
        public async Task<Result<TokenDTO>> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _userRepository.GetUserWithRolesAsync(loginDTO.Email);
            if (user == null)
                return Result<TokenDTO>.Failure(UserErrors.UserNotFound);

            if (!user.EmailConfirmed)
                return Result<TokenDTO>.Failure(UserErrors.EmailConfirmationNotFound);

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDTO.Password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                var passwordResetTokens = user.PasswordResetTokens.FirstOrDefault(t => t.ExpiryDate > DateTime.UtcNow);

                if (passwordResetTokens == null || !passwordResetTokens.ResetToken.Equals(loginDTO.Password))
                    return Result<TokenDTO>.Failure(UserErrors.IncorrectPassword);

                user.PasswordHash = _passwordHasher.HashPassword(user, passwordResetTokens.ResetToken);

                await _userRepository.ExecuteInTransactionAsync(() =>
                {
                    _passwordResetTokenRepository.Remove(passwordResetTokens);
                    _userRepository.Update(user);
                    return Task.CompletedTask;
                });
            }

            var token = _jwtService.GenerateToken(user);
            return Result<TokenDTO>.Success(new TokenDTO { Token = token });
        }

        [Auditable("RESEND_CONFIRMATION_CODE")]
        public async Task<Result> ResendConfirmationCodeAsync(string email)
        {
            var user = await _userRepository.GetUnconfirmedUserByEmailAsync(email);
            if (user == null)
                return Result.Failure(UserErrors.UserNotFound);

            var emailConfirmation = await _emailConfirmationRepository.GetByUserIdAsync(user.UserId);
            if (emailConfirmation == null)
                return Result.Failure(UserErrors.EmailConfirmationNotFound);

            emailConfirmation.ConfirmationCode = Guid.NewGuid().ToString();
            emailConfirmation.ExpiresAt = DateTime.UtcNow.AddHours(24);

            await _emailConfirmationRepository.UpdateAndSaveAsync(emailConfirmation);

            var confirmationLink = $"{_configuration["FrontendUrl"]}/confirm-email?code={emailConfirmation.ConfirmationCode}";
            await _emailService.SendEmailAsync(user.Email, "Confirm Your Email",
                $"Click here to confirm your email: {confirmationLink}");

            return Result.Success();
        }

        [Auditable("CREATE_USER")]
        public async Task<Result> CreateUserAsync(UserCreateDTO userCreateDTO, short userId)
        {
            var createdBy = await _userRepository.GetByIdAsync(userId);
            if (createdBy == null)
                return Result.Failure(UserErrors.UserNotFound);

            var existingUser = await _userRepository.GetByEmailAsync(userCreateDTO.Email);
            if (existingUser != null)
                return Result.Failure(UserErrors.UserExists);

            if (!userCreateDTO.Roles.Any())
                return Result.Failure(UserErrors.NoRolesSpecified);

            var roles = await _roleRepository.GetRolesByIdsAsync(userCreateDTO.Roles);

            if (roles.Count != userCreateDTO.Roles.Count)
                return Result.Failure(UserErrors.InvalidRoles);

            var user = new User
            {
                FirstName = userCreateDTO.FirstName,
                LastName = userCreateDTO.LastName,
                Email = userCreateDTO.Email,
                PhoneNumber = userCreateDTO.PhoneNumber,
                Bio = userCreateDTO.Bio,
                EmailConfirmed = true,
                IsActive = true,
                Roles = roles.ToHashSet(),
                Image = userCreateDTO.Image != null ? FileHelper.FileToByteArray(userCreateDTO.Image) : null
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, userCreateDTO.Password);

            await _userRepository.ExecuteInTransactionAsync(() =>
            {
                _userRepository.Add(user);
                _messageRepository.SaveMessage(
                    createdBy.UserId,
                    $"User {createdBy.FirstName} {createdBy.LastName} created a user {user.FirstName} {user.LastName}."
                );
                return Task.CompletedTask;
            });

            return Result.Success();
        }

        public async Task<Result<IEnumerable<EmployeeDropdownDTO>>> GetEmployeeForDropdownAsync()
        {
            var employees = await _userRepository.GetEmployeesAsync();

            return Result<IEnumerable<EmployeeDropdownDTO>>.Success(employees.Select(o => new EmployeeDropdownDTO
            {
                EmployeeId = o.UserId,
                FirstName = o.FirstName,
                LastName = o.LastName
            }));
        }

        public async Task<Result<object>> GetUsersForAdminAsync(int page, int rowsPerPage, string? search)
        {
            if (page < 1 || rowsPerPage < 1)
                return Result<object>.Failure(ValidationErrors.NumberOfPages);

            var usersQuery = _userRepository.GetAll().Active(b => b.IsActive);
            var totalCount = await usersQuery.CountAsync();

            var employeeAdminDTOForTable = await usersQuery.Paged(page, rowsPerPage).Select(u => new EmployeeAdminDTO
            {
                UserId = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Bio = u.Bio,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Image = u.Image,
                Roles = u.Roles.Select(r => r.Name).ToList()
            }).ToListAsync();

            return Result<object>.Success(new {
                TotalCount = totalCount,
                Employees = employeeAdminDTOForTable
            });
        }

        public async Task<Result<IEnumerable<EmployeeDetailsDTO>>> GetEmployeesAsync()
        {
            var employees = await _userRepository.GetEmployeesAsync();

            return Result<IEnumerable<EmployeeDetailsDTO>>.Success(employees.Select(o => new EmployeeDetailsDTO
            {
                Name = o.FirstName + " " + o.LastName,
                Bio = o.Bio,
                Email = o.Email,
                Image = o.Image
            }));
        }

        [Auditable("DELETE_USER")]
        public async Task<Result> DeleteUserAsync(short userId, short tokenUserId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null || !user.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            var tokenUser = await _userRepository.GetByIdAsync(tokenUserId);

            if (tokenUser == null || !tokenUser.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            user.IsActive = false;
            string message = $"User {tokenUser.FirstName} {tokenUser.LastName} deleted a user {user.FirstName} {user.LastName}.";

            await _userRepository.ExecuteInTransactionAsync(() =>
            {
                _userRepository.Update(user);
                _messageRepository.SaveMessage(tokenUser.UserId, message);
                return Task.CompletedTask;
            });

            return Result.Success();
        }

        [Auditable("UPDATE_USER")]
        public async Task<Result> UpdateUserAsync(short userId, UserUpdateDTO userUpdateDTO, short tokenUserId)
        {
            var existingUser = await _userRepository.GetUserWithRolesAsync(userId);
            if (existingUser == null || !existingUser.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            var tokenUser = await _userRepository.GetByIdAsync(tokenUserId);
            if (tokenUser == null || !tokenUser.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            existingUser.FirstName = userUpdateDTO.FirstName;
            existingUser.LastName = userUpdateDTO.LastName;
            existingUser.Email = userUpdateDTO.Email;
            existingUser.PhoneNumber = userUpdateDTO.PhoneNumber;
            existingUser.Bio = userUpdateDTO.Bio;

            if (userUpdateDTO.Image != null)
                existingUser.Image = FileHelper.FileToByteArray(userUpdateDTO.Image);

            var roles = await _roleRepository.GetAll()
                .Where(r => userUpdateDTO.Roles.Contains(r.RoleId))
                .ToListAsync();

            if (roles.Count != userUpdateDTO.Roles.Count)
                return Result.Failure(UserErrors.InvalidRoles);

            existingUser.Roles.Clear();
            foreach (var role in roles)
                existingUser.Roles.Add(role);

            if (!existingUser.Roles.Any())
            {
                return Result.Failure(UserErrors.NoRolesSpecified);
            }

            string message = $"User {tokenUser.FirstName} {tokenUser.LastName} updated a user {userUpdateDTO.FirstName} {userUpdateDTO.LastName}.";

            await _userRepository.ExecuteInTransactionAsync(() =>
            {
                _userRepository.Update(existingUser);
                _messageRepository.SaveMessage(tokenUser.UserId, message);
                return Task.CompletedTask;
            });

            return Result.Success();
        }

        [Auditable("UPDATE_PASSWORD")]
        public async Task<Result> UpdatePasswordAsync(short tokenUserId, PasswordUpdateDTO passwordChangeDTO)
        {
            var existingUser = await _userRepository.GetByIdAsync(tokenUserId);
            if (existingUser == null || !existingUser.IsActive)
                return Result.Failure(UserErrors.UserNotFound);

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash, passwordChangeDTO.OldPassword) == PasswordVerificationResult.Success;
            if (existingUser == null || !passwordVerificationResult)
                return Result.Failure(UserErrors.UserNotFound);

            existingUser.PasswordHash = _passwordHasher.HashPassword(existingUser, passwordChangeDTO.NewPassword);

            await _userRepository.UpdateAndSaveAsync(existingUser);

            return Result.Success();
        }
    }
}
