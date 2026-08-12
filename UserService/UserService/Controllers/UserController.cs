using Microsoft.AspNetCore.Mvc;
using UserService.Core.Interfaces;
using ErrorOr;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace UserService.Controllers
{
    public record RegisterRequest(string Email, string Password);
    public record LoginRequest(string Email, string Password);
    public record DeleteRequest(string Password);

    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var result = await userService.RegisterAsync(request.Email, request.Password, cancellationToken);

            return result.Match(
                    authResult =>
                    {
                        SetTokenCookies(authResult.AccessToken, authResult.RefreshToken);
                        return Ok(new { message = "Succsess registry!" });
                    },
                    errors => Problem(errors)
                );
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var result = await userService.LoginAsync(request.Email, request.Password, cancellationToken);
            return result.Match(
                    authResult =>
                    {
                        SetTokenCookies(authResult.AccessToken, authResult.RefreshToken);
                        return Ok(new { message = "Succsess login!" });
                    },
                    errors => Problem(errors)
                );
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");
            return Ok();
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteRequest request, CancellationToken cancellation)
        {
            var res = GetUserId();

            if (!res.result)
                return BadRequest();

            var result = await userService.DeleteAsync(request.Password, res.id, cancellation);

            return result.Match(
                    authResult =>
                    {
                        Response.Cookies.Delete("accessToken");
                        Response.Cookies.Delete("refreshToken");
                        return Ok(new { message = "Succsess delete!" });
                    },
                    errors => Problem(errors)
                );
        }


        private void SetTokenCookies(string acsessToken, string refreshToken)
        {
            var cookiesOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("accessToken", acsessToken, cookiesOptions);
            Response.Cookies.Append("refreshToken", refreshToken, cookiesOptions);
        }

        private IActionResult Problem(List<Error> errors)
        {
            if (errors.Count == 0) return Problem();

            var firstError = errors[0];

            var statusCode = firstError.Type switch
            {
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };

            return Problem(statusCode: statusCode, title: firstError.Description);
        }

        private (bool result, Guid id) GetUserId()
        {
            var userId = User.FindFirstValue("sub");

            if (!Guid.TryParse(userId, out var id))
            {
                return (false, Guid.NewGuid());
            }

            return (true, id);
        }
    }

}
