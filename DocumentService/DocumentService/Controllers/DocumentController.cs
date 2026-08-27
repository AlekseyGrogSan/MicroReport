using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DocumentService.Core.DTOs;
using System.Security.Claims;
using DocumentService.Application.Features.Commands.Document.UploadDocument;
using DocumentService.Application.Features.Queries.Document.GetActiveDocument;
using DocumentService.Application.Features.Queries.Document.DownloadDocument;
using Microsoft.Net.Http.Headers;
using DocumentService.Application.Features.Commands.Document.DeleteDocument;
using DocumentService.Application.Features.Commands.ReportRequest;

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

        [HttpGet("{id:guid}/download")]
        public async Task<IActionResult> DownloadDocument(Guid id,  CancellationToken token)
        {
            var result = await _mediator.Send(new DownloadDocumentCommand(id), cancellationToken: token);

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileNameStar = result.filename
            };

            Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

            return File(
                fileStream: result.stream,
                contentType: result.contentType,
                fileDownloadName: result.filename,
                enableRangeProcessing: true
                );
        }

        [HttpPatch("{id:guid}/trash")]
        public async Task<IActionResult> SoftDelete(Guid documentId, CancellationToken token)
        {
            var result = await _mediator.Send(new DeleteDocumentCommand(documentId), token);

            if (!result)
            {
                return Conflict($"File {documentId} alredy exist in trash!");
            }

            return Ok("File puted in trash");
        }

    }
}
