namespace HairStudio.Services.DTOs.Users
{
    public class EmployeeDetailsDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string Email { get; set; } = string.Empty;
        public byte[]? Image { get; set; } = Array.Empty<byte>();
    }
}
