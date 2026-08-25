using DocumentService.Core.DTOs;
using DocumentService.Application.Interfaces;
using MediatR;

namespace DocumentService.Application.Features.Queries.Document.GetActiveDocument
{
    public class GetActiveDocumentHabdler(
        IDocumentReadRepository _repository) : IRequestHandler<GetActiveDocumentQuery, IEnumerable<DocumentDto>>
    {
        public async Task<IEnumerable<DocumentDto>> Handle(GetActiveDocumentQuery request, CancellationToken cancellationToken)
        {
            var documents = await _repository.GetActiveByUserIdAsync(request.UserId, cancellationToken);

            return documents;
        }
    }
}
