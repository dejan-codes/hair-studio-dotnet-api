namespace HairStudio.Services.DTOs.Reservations
{
    public class ReservationSummaryDTO
    {
        public short ServiceId { get; set; }
        public short ReservationId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string ClientFullName { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool ShowCancelButton { get; set; }
    }
}
