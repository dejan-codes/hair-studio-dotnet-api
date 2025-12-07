namespace HairStudio.Services.DTOs.Reservations
{
    public class ReservationDetailsDTO
    {
        public string ServiceName { get; set; } = string.Empty;
        public string ClientFullName { get; set; } = string.Empty;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Note { get; set; }
    }
}
