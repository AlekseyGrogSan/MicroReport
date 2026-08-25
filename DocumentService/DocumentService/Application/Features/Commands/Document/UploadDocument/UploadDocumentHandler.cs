using DocumentService.Application.Interfaces;
using MediatR;

namespace DocumentService.Application.Features.Commands.Document.UploadDocument
{
    public class UploadDocumentHandler(
        IFileStorageService _storageService,
        IDocumentRepository _repository) : IRequestHandler<UploadDocumentCommand, Guid>
    {
        public async Task<Guid> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
        {
            using var stream = request.file.OpenReadStream();

            Guid documentId = Guid.NewGuid();

            var s3Key = await _storageService.UploadFileAsync(
                stream,
                request.file.FileName,
                request.file.ContentType,
                documentId,
                cancellationToken
                );

            var document = Core.Entities.Document.Create(
                documentId,
                request.UserId,
                request.file.FileName,
                request.file.ContentType,
                request.file.Length,
                s3Key
                );

            await _repository.AddAsync(document, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return documentId;
        }
    }
}
