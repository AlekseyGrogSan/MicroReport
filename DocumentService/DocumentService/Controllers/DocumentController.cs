using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DocumentService.Core.DTOs;
using System.Security.Claims;
using DocumentService.Application.Features.Commands.Document.UploadDocument;
using DocumentService.Application.Features.Queries.Document.GetActiveDocument;

namespace DocumentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentController(IMediator _mediator) : ControllerBase
    {
        private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? throw new UnauthorizedAccessException("User ID is missing in token."));

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadDocument(IFormFile File, CancellationToken token)
        {
            var command = new UploadDocumentCommand(File, UserId);
            var documentId = await _mediator.Send(command, token);

            return Ok(new { id = documentId });
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveDocument(CancellationToken token)
        {
            var query = new GetActiveDocumentQuery(UserId);
            var documents = await _mediator.Send(query, token);

            return Ok(documents);
        }
    }
}
