using HairStudio.Services.Common;

namespace HairStudio.Services.Errors
{
    public static class ServiceErrors
    {
        public static readonly Error ServiceNotFound = new Error(
            "Service.ServiceNotFound", "Service not found.");
    }
}
