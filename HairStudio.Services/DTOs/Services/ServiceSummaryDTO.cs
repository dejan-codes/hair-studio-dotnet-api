namespace HairStudio.Services.DTOs.Services
{
    public class ServiceSummaryDTO
    {
        public short ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string Gender { get; set; } = string.Empty;
        public byte[] Image { get; set; } = null!;
    }
}
