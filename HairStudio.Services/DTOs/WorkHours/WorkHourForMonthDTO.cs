namespace HairStudio.Services.DTOs.WorkHours
{
    public class WorkHourForMonthDTO
    {
        public short WorkHourId { get; set; }
        public short EmployeeId { get; set; }
        public int Day { get; set; }
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan TimeTo { get; set; }
    }
}
