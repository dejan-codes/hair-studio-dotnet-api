using HairStudio.Services.Common;

namespace HairStudio.Services.Errors
{
    public static class UserErrors
    {
        public static readonly Error UserExists = new Error(
            "User.UserExists", "User with entered email already exists.");

        public static readonly Error InvalidConfirmationCode = new Error(
            "User.InvalidConfirmationCode", "Invalid or expired confirmation code.");

        public static readonly Error EmailNotFound = new Error(
            "User.EmailNotFound", "Email not found.");

        public static readonly Error UserNotFound = new Error(
            "User.UserNotFound", "User not found.");

        public static readonly Error EmailConfirmationNotFound = new Error(
            "User.EmailConfirmationNotFound", "Error finding email confirmation.");

        public static readonly Error IncorrectPassword = new Error(
            "User.IncorrectPassword", "Incorrect password.");

        public static readonly Error NoRolesSpecified = new Error(
            "User.NoRolesSpecified", "At least one role must be specified.");

        public static readonly Error InvalidRoles = new Error(
            "User.InvalidRoles", "Invalid role(s).");

        public static readonly Error UserNoRoles = new Error(
            "User.UserNoRoles", "User doesn't have any role.");

        public static readonly Error UserNoPermissionForReservationCancellation = new Error(
            "User.UserNoPermissionForReservationCancellation", "User doesn't have permission to cancel this reservation.");
    }
}
