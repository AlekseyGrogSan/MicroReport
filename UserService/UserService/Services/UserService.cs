using UserService.Core.DTOs;
using UserService.Core.Errors;
using UserService.Core.Interfaces;
using UserService.Core.Models;
using UserService.Data.Repositories;
using ErrorOr;

namespace UserService.Services
{
    public class UsersService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJWTService jwtService) : IUserService
    {
        public async Task<ErrorOr<UserResult>> RegisterAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var existingUser = await userRepository.GetByEmailAsync(email, cancellationToken);
            if (existingUser != null)
                return UserError.DublicateEmain;

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = passwordHasher.HasPassword(password),
                CreatedAtUtc = DateTime.UtcNow
            };

            var accessToken = jwtService.GenerateAcsessToken(user);
            var refreshToken = jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await userRepository.AddAsync(user, cancellationToken);

            return new UserResult(accessToken, refreshToken);
        }

        public async Task<ErrorOr<UserResult>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetByEmailAsync(email, cancellationToken);
            if (user == null || !passwordHasher.Verify(password, user.PasswordHash))
                return UserError.InvalidCreditianals;

            var accessToken = jwtService.GenerateAcsessToken(user);
            var refreshToken = jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await userRepository.UpdateAsync(user, cancellationToken);

            return new UserResult(accessToken, refreshToken);
        }

        public async Task<ErrorOr<DeletedResult>> DeleteAsync(string password, Guid id, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetByIdAsync(id);

            if (user == null)
                return UserError.InvalidCreditianals;

            if (!passwordHasher.Verify(password, user.PasswordHash))
                return UserError.InvalidCreditianals;
            
            await userRepository.DeleteAsync(id, cancellationToken);

            return new DeletedResult(true);
        }
    }
}
