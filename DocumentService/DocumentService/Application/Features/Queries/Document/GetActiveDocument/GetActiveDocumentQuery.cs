using DocumentService.Core.DTOs;
using MediatR;

namespace DocumentService.Application.Features.Queries.Document.GetActiveDocument
{
    public record GetActiveDocumentQuery(Guid UserId) : IRequest<IEnumerable<DocumentDto>>;
}
