namespace HairStudio.Services.DTOs.Services
{
    public class ServiceDropdownDTO
    {
        public short ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
    }
}
