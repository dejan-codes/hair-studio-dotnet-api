using System.ComponentModel.DataAnnotations;

namespace HairStudio.Services.DTOs.Reservations
{
    public class EmployeeReservationCreateDTO : IValidatableObject
    {
        [Required]
        public short ServiceId { get; set; }

        [Required]
        public short EmployeeId { get; set; }

        [Required]
        public DateTime DateFrom { get; set; }

        [Required]
        public DateTime DateTo { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Full name can't be longer than 50 characters.")]
        public string FullName { get; set; } = string.Empty;
        
        [Phone]
        public string? Phone { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Employees note can't be longer than 100 characters.")]
        public string EmployeesNote { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateFrom >= DateTo)
            {
                yield return new ValidationResult(
                    "DateFrom must be earlier than DateTo.",
                    new[] { nameof(DateFrom), nameof(DateTo) }
                );
            }
        }
    }
}
