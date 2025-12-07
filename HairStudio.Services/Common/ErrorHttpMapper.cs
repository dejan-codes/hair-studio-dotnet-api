using HairStudio.Services.Errors;
using Microsoft.AspNetCore.Http;

namespace HairStudio.Services.Common
{
    public static class ErrorHttpMapper
    {
        private static readonly Dictionary<string, int> _errorStatusCodes = new()
        {
            // User errors
            { UserErrors.UserExists.Code, StatusCodes.Status400BadRequest },
            { UserErrors.InvalidConfirmationCode.Code, StatusCodes.Status400BadRequest },
            { UserErrors.EmailNotFound.Code, StatusCodes.Status404NotFound },
            { UserErrors.UserNotFound.Code, StatusCodes.Status404NotFound },
            { UserErrors.UserNoRoles.Code, StatusCodes.Status404NotFound },
            { UserErrors.EmailConfirmationNotFound.Code, StatusCodes.Status404NotFound },
            { UserErrors.IncorrectPassword.Code, StatusCodes.Status400BadRequest },
            { UserErrors.UserNoPermissionForReservationCancellation.Code, StatusCodes.Status401Unauthorized },
            { UserErrors.NoRolesSpecified.Code, StatusCodes.Status400BadRequest },
            { UserErrors.InvalidRoles.Code, StatusCodes.Status400BadRequest },

            // Brand errors
            { BrandErrors.BrandNotFound.Code, StatusCodes.Status404NotFound },
            { BrandErrors.BrandHasProduct.Code, StatusCodes.Status400BadRequest },
        
            // Product type errors
            { ProductTypeErrors.ProductTypeNotFound.Code, StatusCodes.Status404NotFound },
            { ProductTypeErrors.ProductTypeHasProduct.Code, StatusCodes.Status400BadRequest },
        
            // Product errors
            { ProductErrors.ProductNotFound.Code, StatusCodes.Status404NotFound },
        
            // Service errors
            { ServiceErrors.ServiceNotFound.Code, StatusCodes.Status404NotFound },
        
            // Order errors
            { OrderErrors.OrderNotFound.Code, StatusCodes.Status404NotFound },
            { OrderErrors.OrderStatusNotFound.Code, StatusCodes.Status404NotFound },
        
            // Reservation errors
            { ReservationErrors.ReservationNotFound.Code, StatusCodes.Status404NotFound },

            // Work hour errors
            { WorkHourErrors.TimeRangeError.Code, StatusCodes.Status400BadRequest },

            // Validation errors
            { ValidationErrors.InvalidData.Code, StatusCodes.Status400BadRequest },
            { ValidationErrors.NumberOfPages.Code, StatusCodes.Status400BadRequest }
        };

        public static int GetStatusCode(Error error)
        {
            if (_errorStatusCodes.TryGetValue(error.Code, out var status))
                return status;

            return StatusCodes.Status400BadRequest;
        }
    }
}
