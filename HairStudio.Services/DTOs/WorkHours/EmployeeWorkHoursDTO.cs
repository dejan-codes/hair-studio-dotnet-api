namespace HairStudio.Services.DTOs.WorkHours
{
    public class EmployeeWorkHoursDTO
    {
        public DateTime Date { get; set; }
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan TimeTo { get; set; }
    }
}
