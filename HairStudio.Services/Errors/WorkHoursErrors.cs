using HairStudio.Services.Common;

namespace HairStudio.Services.Errors
{
    public static class WorkHourErrors
    {
        public static readonly Error TimeRangeError = new Error(
            "WorkHour.TimeRangeError", "Time from cannot be greater than time to.");
    }
}
