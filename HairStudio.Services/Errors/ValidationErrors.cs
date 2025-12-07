using HairStudio.Services.Common;

namespace HairStudio.Services.Errors
{
    public static class ValidationErrors
    {
        public static readonly Error InvalidData = new Error(
            "Validation.InvalidData", "Invalid input data.");

        public static readonly Error NumberOfPages = new Error(
            "Validation.NumberOfPages", "Page and rowsPerPage must be greater than 0.");
    }
}
