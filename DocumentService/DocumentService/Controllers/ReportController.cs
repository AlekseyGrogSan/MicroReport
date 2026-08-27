using DocumentService.Application.Features.Commands.ReportRequest;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DocumentService.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportController(IMediator _mediator) : ControllerBase
    {
        private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? throw new UnauthorizedAccessException("User ID is missing in token."));

        [HttpPost("generate")]
        public async Task<IActionResult> CreateReportRequest([FromBody] CreateReportRequestCommand command, CancellationToken token)
        {
            var secureCommand = command with { UserId = UserId };
            var result = await _mediator.Send(command, cancellationToken: token);

            return Accepted(new { RequestId = result, status = "Pending" });
        }
    }
}
