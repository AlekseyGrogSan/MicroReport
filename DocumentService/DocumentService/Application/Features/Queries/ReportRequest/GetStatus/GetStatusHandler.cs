using DocumentService.Application.Interfaces;
using MediatR;

namespace DocumentService.Application.Features.Queries.ReportRequest.GetStatus
{
    public class GetStatusHandler(
        IReportReadRepository _repository
        ) : IRequestHandler<GetStatusQuery, string>
    {
        public async Task<string?> Handle(GetStatusQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetRequestStatusAsync(request.RequestId, cancellationToken);
        }
    }
}
