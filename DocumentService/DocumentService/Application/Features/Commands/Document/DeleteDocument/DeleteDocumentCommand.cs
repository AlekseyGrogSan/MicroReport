using MediatR;

namespace DocumentService.Application.Features.Commands.Document.DeleteDocument
{
    public record DeleteDocumentCommand(Guid documentId) : IRequest<bool>;
}
