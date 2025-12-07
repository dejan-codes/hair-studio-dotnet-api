namespace HairStudio.Services.DTOs.Services
{
    public class ServiceDTO
    {
        public short? ServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public int DurationMinutes { get; set; }
        public byte GenderId { get; set; }
        public byte[] Image { get; set; } = null!;
        public byte SequenceNumber { get; set; }
    }
}
