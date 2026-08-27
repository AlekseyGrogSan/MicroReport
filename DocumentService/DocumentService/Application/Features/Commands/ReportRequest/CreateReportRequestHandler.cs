using DocumentService.Application.Interfaces;
using DocumentService.Core.DTOs;
using DocumentService.Core.Settings;
using MediatR;
using Microsoft.Extensions.Options;

namespace DocumentService.Application.Features.Commands.ReportRequest
{
    public class CreateReportRequestHandler(
        IReportRequestRepository _ReportRequestRepository,
        IDocumentReadRepository _documentRepository,
        IOptions<KafkaSettings> _settings,
        IKafkaProducer _producer,
        ILogger<CreateReportRequestHandler> _logger) : IRequestHandler<CreateReportRequestCommand, Guid>
    {
        public async Task<Guid> Handle(CreateReportRequestCommand request, CancellationToken cancellationToken)
        {
            var userDocuments = await _documentRepository.GetActiveByUserIdAsync(request.UserId, cancellationToken);
            var userDocumentsIds = userDocuments.Select(d => d.Id).ToHashSet();

            var invalidDocuments = request.documentsIds.Where(dId => !userDocumentsIds.Contains(dId));

            if (invalidDocuments.Any())
            {
                _logger.LogWarning("User {userId} try get report with no your documents", request.UserId);
                throw new UnauthorizedAccessException();
            }

            var reportRequest = Core.Entities.ReportRequest.Create(request.UserId, request.UserPromt, request.TargetContentType, request.documentsIds);

            await _ReportRequestRepository.AddAsync(reportRequest, cancellationToken);
            await _ReportRequestRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Request {requestId} for make report was registy", reportRequest.Id);

            var topic = _settings.Value.Topics["ReportRequestTopic"];

            var s3Keys = await _documentRepository.GetS3KeysByDocumnetIdsAsync(request.documentsIds, cancellationToken);

            var eventDto = new ReportRequestEventDto(reportRequest.Id, reportRequest.UserId, reportRequest.UserPromt, s3Keys.ToList());

            await _producer.ProduceAsync(topic, eventDto, cancellationToken);

            return reportRequest.Id;
        }
    }
}
