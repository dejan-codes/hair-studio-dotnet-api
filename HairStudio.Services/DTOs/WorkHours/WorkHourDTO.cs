using System.ComponentModel.DataAnnotations;

namespace HairStudio.Services.DTOs.WorkHours
{
    public partial class WorkHourDTO : IValidatableObject
    {
        [Required]
        public short EmployeeId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string TimeFrom { get; set; } = String.Empty;

        [Required]
        public string TimeTo { get; set; } = String.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!TimeSpan.TryParse(TimeFrom, out var timeFrom) ||
                !TimeSpan.TryParse(TimeTo, out var timeTo))
            {
                yield return new ValidationResult(
                    "TimeFrom or TimeTo are not in a valid time format (e.g., HH:mm).",
                    new[] { nameof(TimeFrom), nameof(TimeTo) }
                );
            }
            else if (timeFrom >= timeTo)
            {
                yield return new ValidationResult(
                    "TimeFrom must be earlier than TimeTo.",
                    new[] { nameof(TimeFrom), nameof(TimeTo) }
                );
            }
        }
    }
}
