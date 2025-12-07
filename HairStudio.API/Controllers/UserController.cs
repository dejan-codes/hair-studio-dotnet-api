using HairStudio.API.Infrastructure;
using HairStudio.Services.Common;
using HairStudio.Services.DTOs.Users;
using HairStudio.Services.Errors;
using HairStudio.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HairStudio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserContext _currentUserContext;

        public UserController(IUserService userService, ICurrentUserContext currentUserContext)
        {
            _userService = userService;
            _currentUserContext = currentUserContext;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] UserRegistrationDTO userRegistrationDTO)
        {
            var result = await _userService.RegisterUserAsync(userRegistrationDTO);
            return result.ToActionResult();
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmUserEmailAsync([FromBody] EmailConfirmationDTO emailConfirmationDTO)
        {
            var result = await _userService.ConfirmUserEmailAsync(emailConfirmationDTO.Code);
            return result.ToActionResult();
        }

        [HttpPost("password-reset")]
        public async Task<IActionResult> UserPasswordResetAsync([FromBody] PasswordResetDTO passwordResetDTO)
        {
            var result = await _userService.UserPasswordResetAsync(passwordResetDTO.Email);
            return result.ToActionResult();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO loginDTO)
        {
            var result = await _userService.LoginAsync(loginDTO);
            return result.ToActionResult();
        }

        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendEmailConfirmationAsync([FromBody] ResendConfirmationCodeDTO resendConfirmationCodeDTO)
        {
            var result = await _userService.ResendConfirmationCodeAsync(resendConfirmationCodeDTO.Email);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromForm] UserCreateDTO userCreateDTO)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _userService.CreateUserAsync(userCreateDTO, tokenUserId.Value);
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUserAsync(short userId)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _userService.DeleteUserAsync(userId, tokenUserId.Value);
            return result.ToActionResult();
        }

        [HttpGet("employee-dropdown")]
        public async Task<IActionResult> GetEmployeeForDropdownAsync()
        {
            var result = await _userService.GetEmployeeForDropdownAsync();
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("users-administration")]
        public async Task<IActionResult> GetUsersForAdminAsync(int page, int rowsPerPage, string? search)
        {
            var result = await _userService.GetUsersForAdminAsync(page, rowsPerPage, search);
            return result.ToActionResult();
        }

        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployeesAsync()
        {
            var result = await _userService.GetEmployeesAsync();
            return result.ToActionResult();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUserAsync(short userId, [FromForm] UserUpdateDTO userUpdateDTO)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _userService.UpdateUserAsync(userId, userUpdateDTO, tokenUserId.Value);
            return result.ToActionResult();
        }

        [Authorize(Roles = "User,Employee,Administrator")]
        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePasswordAsync([FromBody] PasswordUpdateDTO passwordUpdateDTO)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _userService.UpdatePasswordAsync(tokenUserId.Value, passwordUpdateDTO);
            return result.ToActionResult();
        }
    }
}
