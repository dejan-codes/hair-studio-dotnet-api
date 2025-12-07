using System.ComponentModel.DataAnnotations;

namespace HairStudio.Services.DTOs.WorkHours
{
    public class WorkHourDeleteDTO
    {
        [Required]
        public short EmployeeId { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
