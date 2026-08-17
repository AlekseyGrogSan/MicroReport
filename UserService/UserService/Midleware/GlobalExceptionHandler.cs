using Microsoft.AspNetCore.Diagnostics;

namespace UserService.Midleware
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> _logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(
                exception,
                "Unhandled exception: {Message}",
                exception.Message
                );

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(new
            {
                statusCode = 500,
                message = "Internal server error"
            }, cancellationToken);

            return true;
        }
    }
}
