using MediatR;
using Microsoft.AspNetCore.Http;

namespace DocumentService.Application.Features.Commands.Document.UploadDocument
{
    public record UploadDocumentCommand
    (
        IFormFile file,
        Guid UserId
    ) : IRequest<Guid>;
}
