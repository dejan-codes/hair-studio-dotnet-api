using HairStudio.Services.Common;

namespace HairStudio.Services.Errors
{
    public static class ReservationErrors
    {
        public static readonly Error ReservationNotFound = new Error(
            "Reservation.ReservationNotFound", "Reservation not found.");
    }
}
