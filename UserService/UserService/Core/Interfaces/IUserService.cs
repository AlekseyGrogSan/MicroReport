using UserService.Core.DTOs;
using ErrorOr;

namespace UserService.Core.Interfaces
{
    public interface IUserService
    {
        Task<ErrorOr<DeletedResult>> DeleteAsync(string password, Guid id, CancellationToken cancellationToken = default);
        Task<ErrorOr<UserResult>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<ErrorOr<UserResult>> RegisterAsync(string email, string password, CancellationToken cancellationToken = default);

    }
}