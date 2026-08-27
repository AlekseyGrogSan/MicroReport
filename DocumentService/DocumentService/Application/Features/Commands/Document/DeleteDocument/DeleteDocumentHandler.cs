using DocumentService.Application.Interfaces;
using DocumentService.Core.Enums;
using MediatR;

namespace DocumentService.Application.Features.Commands.Document.DeleteDocument
{
    public class DeleteDocumentHandler(
        IDocumentRepository _repository) : IRequestHandler<DeleteDocumentCommand, bool>
    {
        public async Task<bool> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            var document = await _repository.GetByIdAsync(request.documentId, cancellationToken);

            //Проверяем статус файла
            if (document.Status == DocumentStatus.Deleted)
            {
                return false;
            }

            document.MarkAsDeleted();
            await _repository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
