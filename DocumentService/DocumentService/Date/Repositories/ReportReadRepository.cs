using Dapper;
using DocumentService.Application.Interfaces;
using DocumentService.Core.DTOs;
using System.Data;

namespace DocumentService.Date.Repositories
{
    public class ReportReadRepository(IDbConnection _connection) : IReportReadRepository
    {
        // 1. Получить все готовые отчеты конкретного пользователя
        public async Task<IEnumerable<ReportDto>> GetUserReportsAsync(Guid userId, CancellationToken token)
        {
            string query = """
                SELECT 
                   *
                FROM "Reports" r
                WHERE "UserId" = @UserId
                ORDER BY "CreatedAt" DESC
                """;

            var command = new CommandDefinition(query, new { UserId = userId }, cancellationToken: token);
            return await _connection.QueryAsync<ReportDto>(command);
        }

        // 2. Получить конкретный отчет по его ID
        public async Task<ReportDto?> GetByIdAsync(Guid reportId, CancellationToken token)
        {
            string query = """
                SELECT 
                    *
                FROM "Reports" r
                WHERE "Id" = @ReportId
                """;

            var command = new CommandDefinition(query, new { ReportId = reportId }, cancellationToken: token);
            return await _connection.QueryFirstOrDefaultAsync<ReportDto>(command);
        }

        public async Task<string?> GetRequestStatusAsync(Guid requestId, CancellationToken token)
        {
            string query = """
                SELECT "Status"::text 
                FROM "ReportRequests" 
                WHERE "Id" = @RequestId
                """;

            var command = new CommandDefinition(query, new { RequestId = requestId }, cancellationToken: token);
            return await _connection.QueryFirstOrDefaultAsync<string>(command);
        }
    }
}