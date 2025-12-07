namespace HairStudio.Services.DTOs.Users
{
    public class EmployeeAdminDTO
    {
        public short UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public string Email { get; set; } = string.Empty;
        public byte[]? Image { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
