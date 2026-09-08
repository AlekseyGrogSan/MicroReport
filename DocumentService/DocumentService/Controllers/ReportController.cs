using DocumentService.Application.Features.Commands.ReportRequest;
using DocumentService.Application.Features.Queries.Report.DownloadReport;
using DocumentService.Application.Features.Queries.Report.GetAllReports;
using DocumentService.Application.Features.Queries.ReportRequest.GetStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Security.Claims;

namespace DocumentService.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportController(IMediator _mediator, ILogger<ReportController> _logger) : ControllerBase
    {
        private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? throw new UnauthorizedAccessException("User ID is missing in token."));

        [HttpPost("generate")]
        public async Task<IActionResult> CreateReportRequest([FromBody] CreateReportRequestCommand command, CancellationToken token)
        {
            var secureCommand = command with { UserId = UserId };
            var result = await _mediator.Send(secureCommand, cancellationToken: token);

            return Accepted(new { RequestId = result, status = "Pending" });
        }

        [HttpGet("{id:guid}/download")]
        public async Task<IActionResult> DownloadReport(Guid id, CancellationToken token)
        {
            _logger.LogInformation("Начали скачивать документ с ID {ID}", id);
            var result = await _mediator.Send(new DownloadReportQuery(id), token);

            var contentDisposition = new ContentDisposition
            {
                FileName = result.filename
            };

            Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

            var content_type = result.contentType switch
            {
                ".md" => "text/markdown",
                _ => "text/markdown"
            };
            return File(
               fileStream: result.stream,
               contentType: content_type,
               fileDownloadName: result.filename,
               enableRangeProcessing: true
               );
        }

        [HttpGet("all-reports")]
        public async Task<IActionResult> GetAllReportsByUserId(CancellationToken token)
        {
            var result = await _mediator.Send(new GetAllReportQuery(UserId), cancellationToken: token);
            return Ok(result);
        }

        [HttpGet("{id:guid}/get-status")]
        public async Task<IActionResult> GetStatusRequest(Guid id, CancellationToken token)
        {
            var result = await _mediator.Send(new GetStatusQuery(id), token);
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
    }
}
