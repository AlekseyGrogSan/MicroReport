using ErrorOr;

namespace UserService.Core.Errors
{
    public static class UserError
    {
        public static Error DublicateEmain => Error.Conflict(
            code: "User.DublicateEmail",
            description: "User with this email already exist"
            );

        public static Error InvalidCreditianals => Error.Validation(
            code: "User.InvalidCreditionals",
            description: "Invalid password or email"
            );

        public static Error InvalidRefreshToken => Error.Unauthorized(
            code:"User.InvalidRefreshToken",
            description:"Invalid Refresh Token"
            );

        public static Error NotFoundUser => Error.NotFound(
            code: "User.NotFoundUser",
            description: "User with this email or id not exist"
            );
    }
}
