using Moq;
using Xunit;
using UserService.Core.Errors;
using UserService.Core.Interfaces;
using UserService.Core.Models;
using UserService.Data.Repositories;
using UserService.Services;

namespace UserService.Tests.Unit
{
    public class UsersServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
        private readonly Mock<IJWTService> _jwtServiceMock = new();
        private readonly UsersService _sut;

        public UsersServiceTests()
        {
            _sut = new UsersService(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _jwtServiceMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_NewUser_ReturnsUserResult()
        {
            // Arrange
            var email = "test@example.com";
            var password = "Password123!";

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            _passwordHasherMock
                .Setup(h => h.HasPassword(password))
                .Returns("hashed-password");

            _jwtServiceMock
                .Setup(j => j.GenerateAcsessToken(It.IsAny<User>()))
                .Returns("access-token");

            _jwtServiceMock
                .Setup(j => j.GenerateRefreshToken())
                .Returns("refresh-token");

            // Act
            var result = await _sut.RegisterAsync(email, password);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal("access-token", result.Value.AccessToken);
            Assert.Equal("refresh-token", result.Value.RefreshToken);

            _userRepositoryMock.Verify(r => r.AddAsync(
                It.Is<User>(u => u.Email == email && u.PasswordHash == "hashed-password"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_EmailAlreadyExists_ReturnsDuplicateEmailError()
        {
            // Arrange
            var email = "test@example.com";
            var existingUser = new User { Id = Guid.NewGuid(), Email = email };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _sut.RegisterAsync(email, "any-password");

            // Assert
            Assert.True(result.IsError);
            Assert.Contains(UserError.DublicateEmain, result.Errors);

            _userRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsUserResult()
        {
            // Arrange
            var email = "test@example.com";
            var password = "Password123!";
            var user = new User { Id = Guid.NewGuid(), Email = email, PasswordHash = "hashed" };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h => h.Verify(password, user.PasswordHash))
                .Returns(true);

            _jwtServiceMock.Setup(j => j.GenerateAcsessToken(user)).Returns("access-token");
            _jwtServiceMock.Setup(j => j.GenerateRefreshToken()).Returns("refresh-token");

            // Act
            var result = await _sut.LoginAsync(email, password);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal("access-token", result.Value.AccessToken);
            Assert.Equal("refresh-token", result.Value.RefreshToken);

            _userRepositoryMock.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_UserNotFound_ReturnsInvalidCredentialsError()
        {
            // Arrange
            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _sut.LoginAsync("missing@example.com", "any-password");

            // Assert
            Assert.True(result.IsError);
            Assert.Contains(UserError.InvalidCreditianals, result.Errors);
        }

        [Fact]
        public async Task LoginAsync_WrongPassword_ReturnsInvalidCredentialsError()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", PasswordHash = "hashed" };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h => h.Verify(It.IsAny<string>(), user.PasswordHash))
                .Returns(false);

            // Act
            var result = await _sut.LoginAsync(user.Email, "wrong-password");

            // Assert
            Assert.True(result.IsError);
            Assert.Contains(UserError.InvalidCreditianals, result.Errors);
        }

        [Fact]
        public async Task DeleteAsync_CorrectPassword_ReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, PasswordHash = "hashed" };

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h => h.Verify("correct-password", user.PasswordHash))
                .Returns(true);

            // Act
            var result = await _sut.DeleteAsync("correct-password", userId);

            // Assert
            Assert.False(result.IsError);
            Assert.True(result.Value.result);

            _userRepositoryMock.Verify(r => r.DeleteAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_UserNotFoundOrWrongPassword_ReturnsInvalidCredentialsError()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _sut.DeleteAsync("any-password", userId);

            // Assert
            Assert.True(result.IsError);
            Assert.Contains(UserError.InvalidCreditianals, result.Errors);

            _userRepositoryMock.Verify(
                r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
