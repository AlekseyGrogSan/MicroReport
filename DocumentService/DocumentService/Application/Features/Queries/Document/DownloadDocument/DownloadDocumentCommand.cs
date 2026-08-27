using DocumentService.Core.DTOs;
using MediatR;

namespace DocumentService.Application.Features.Queries.Document.DownloadDocument
{
    public record DownloadDocumentCommand(Guid id) : IRequest<DownloadfResult>;
}
