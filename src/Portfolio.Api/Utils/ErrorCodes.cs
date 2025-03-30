namespace Portfolio.Api.Utils;

public static class ErrorCodes
{
    public static class Users
    {
        public const string NotFound = "User.NotFound";
        public const string InvalidCredentials = "User.InvalidCredentials";
        public const string EmailConflict = "User.EmailConflict";
        public const string RefreshToken = "User.RefreshToken";
        public const string KeycloakServerError = "User.KeycloakServerError";
    }

    public static class Tags
    {
        public const string UserNotFound = "Tag.UserNotFound";
        public const string NotFound = "Tag.NotFound";
        public const string Conflict = "Tag.Conflict";
    }
}
