using DocumentService.Application.Interfaces;
using DocumentService.Core.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DocumentService.Application.Features.Queries.Report.DownloadReport
{
    public class DownloadReportHandler(
        IReportReadRepository _repository,
        IFileStorageService _storage
        ) : IRequestHandler<DownloadReportQuery, DownloadfResult>
    {
        public async Task<DownloadfResult> Handle(DownloadReportQuery request, CancellationToken cancellationToken)
        {
            var metadate = await _repository.GetByIdAsync(request.id, cancellationToken);

            if ( metadate == null)
            {
                throw new DirectoryNotFoundException();
            }

            var stream = await _storage.DownloadFileAsync(metadate.S3Key, cancellationToken);

            return new DownloadfResult(stream, metadate.ContentType, metadate.ReportName);
        }
    }
}
