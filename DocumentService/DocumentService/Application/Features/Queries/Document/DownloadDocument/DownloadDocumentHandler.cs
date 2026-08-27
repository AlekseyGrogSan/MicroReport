using DocumentService.Application.Interfaces;
using DocumentService.Core.DTOs;
using MediatR;

namespace DocumentService.Application.Features.Queries.Document.DownloadDocument
{
    public class DownloadDocumentHandler(
        IFileStorageService _storage,
        IDocumentReadRepository _repository) : IRequestHandler<DownloadDocumentCommand, DownloadfResult>
    {
        public async Task<DownloadfResult> Handle(DownloadDocumentCommand request, CancellationToken cancellationToken)
        {
            var metadate = await _repository.GetByIdAsync(request.id, cancellationToken);

            var stream =  await _storage.DownloadFileAsync(metadate.S3Key, cancellationToken);
            
            return new DownloadfResult(stream, metadate.ContentType, metadate.FileName);
        }
    }
}
