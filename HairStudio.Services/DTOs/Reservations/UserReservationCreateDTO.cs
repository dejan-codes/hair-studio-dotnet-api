using System.ComponentModel.DataAnnotations;

namespace HairStudio.Services.DTOs.Reservations
{
    public class UserReservationCreateDTO : IValidatableObject
    {
        [Required]
        public short ServiceId { get; set; }

        [Required]
        public short EmployeeId { get; set; }

        [Required]
        public DateTime DateFrom { get; set; }

        [Required]
        public DateTime DateTo { get; set; }

        [StringLength(100, ErrorMessage = "Note can't be longer than 100 characters.")]
        public string Note { get; set; } = string.Empty;

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
