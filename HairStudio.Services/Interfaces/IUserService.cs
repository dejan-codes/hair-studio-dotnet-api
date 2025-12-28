using HairStudio.Services.Common;
using HairStudio.Services.DTOs.Users;

namespace HairStudio.Services.Interfaces
{
    public interface IUserService
    {
        Task<Result> RegisterUserAsync(UserRegistrationDTO userRegistrationDTO);
        Task<Result> ConfirmUserEmailAsync(string code);
        Task<Result> UserPasswordResetAsync(string email);
        Task<Result<TokenDTO>> LoginAsync(LoginDTO loginDTO);
        Task<Result> ResendConfirmationCodeAsync(string email);
        Task<Result> CreateUserAsync(UserCreateDTO userCreateDTO);
        Task<Result<IEnumerable<EmployeeDropdownDTO>>> GetEmployeeForDropdownAsync();
        Task<Result<object>> GetUsersForAdminAsync(int page, int rowsPerPage, string? search);
        Task<Result<IEnumerable<EmployeeDetailsDTO>>> GetEmployeesAsync();
        Task<Result> DeleteUserAsync(short userId);
        Task<Result> UpdateUserAsync(short userId, UserUpdateDTO userUpdateDTO);
        Task<Result> UpdatePasswordAsync(PasswordUpdateDTO passwordUpdateDTO);
    }
}
