using System.Data;
using Dapper;
using DocumentService.Application.Interfaces;
using DocumentService.Core.DTOs;

namespace DocumentService.Date.Repositories
{
    public class DocumentReadRepository(IDbConnection _connection) : IDocumentReadRepository
    {
        public async Task<IEnumerable<DocumentDto>> GetActiveByUserIdAsync(Guid UserId, CancellationToken token)
        {
            string querry = "SELECT * FROM \"Documents\" WHERE \"UserId\" = @CurrentUserId AND \"Status\" != 3";

            var command = new CommandDefinition(
                commandText: querry,
                parameters: new { CurrentUserId = UserId },
                cancellationToken: token);

            return await _connection.QueryAsync<DocumentDto>(command);
        }

        public async Task<IEnumerable<DocumentDto>> GetTrashByUserIdAsync(Guid UserId, CancellationToken token)
        {
            string querry = "SELECT * FROM \"Documents\" WHERE \"UserId\" = @CurrentUserId AND \"Status\" = 3";

            var command = new CommandDefinition(
                commandText: querry,
                parameters: new { CurrentUserId = UserId },
                cancellationToken: token);

            return await _connection.QueryAsync<DocumentDto>(command);
        }

        public async Task<DocumentDto?> GetByIdAsync(Guid documentId, CancellationToken token)
        {
            string querry = "SELECT * FROM \"Documents\" WHERE \"Id\" = @CurrentDocumentId";

            var command = new CommandDefinition(
                commandText: querry,
                parameters: new { CurrentDocumentId = documentId },
                cancellationToken: token);

            return await _connection.QueryFirstOrDefaultAsync<DocumentDto>(command);
        }
    }
}
