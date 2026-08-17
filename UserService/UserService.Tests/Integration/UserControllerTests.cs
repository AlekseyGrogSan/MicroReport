using System.Net;
using System.Net.Http.Json;
using ErrorOr;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using UserService.Core.DTOs;
using UserService.Core.Interfaces;
using UserService.Data;
using Xunit;

namespace UserService.Tests.Integration
{
    public class UserControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public UserControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        private static string UniqueEmail() => $"user_{Guid.NewGuid():N}@test.com";

        [Fact]
        public async Task Register_NewUser_Returns200WithCookies()
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/User/register", new
            {
                Email = UniqueEmail(),
                Password = "Password123!"
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var cookies = GetSetCookieHeaders(response);
            Assert.Contains(cookies, c => c.StartsWith("accessToken="));
            Assert.Contains(cookies, c => c.StartsWith("refreshToken="));
        }

        [Fact]
        public async Task Register_DuplicateEmail_Returns409()
        {
            var client = _factory.CreateClient();
            var email = UniqueEmail();

            await client.PostAsJsonAsync("/api/User/register", new { Email = email, Password = "Password123!" });
            var response = await client.PostAsJsonAsync("/api/User/register", new { Email = email, Password = "Password123!" });

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Login_ValidCredentials_Returns200WithCookies()
        {
            var client = _factory.CreateClient();
            var email = UniqueEmail();
            var password = "Password123!";

            await client.PostAsJsonAsync("/api/User/register", new { Email = email, Password = password });

            var response = await client.PostAsJsonAsync("/api/User/login", new { Email = email, Password = password });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var cookies = GetSetCookieHeaders(response);
            Assert.Contains(cookies, c => c.StartsWith("accessToken="));
            Assert.Contains(cookies, c => c.StartsWith("refreshToken="));
        }

        [Fact]
        public async Task Login_InvalidCredentials_Returns400()
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/User/login", new
            {
                Email = UniqueEmail(),
                Password = "wrong-password"
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Logout_Authenticated_Returns200AndClearsCookies()
        {
            var client = _factory.CreateClient();
            var token = TestJwtTokenGenerator.GenerateToken(Guid.NewGuid());

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/User/logout");
            request.Headers.Add("Cookie", $"accessToken={token}");

            var response = await client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var cookies = GetSetCookieHeaders(response);
            Assert.Contains(cookies, c => c.StartsWith("accessToken=") && c.Contains("expires="));
            Assert.Contains(cookies, c => c.StartsWith("refreshToken=") && c.Contains("expires="));
        }

        [Fact]
        public async Task Logout_Unauthenticated_Returns401()
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsync("/api/User/logout", content: null);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_AuthenticatedWithCorrectPassword_Returns200AndClearsCookies()
        {
            var client = _factory.CreateClient();
            var email = UniqueEmail();
            var password = "Password123!";

            await client.PostAsJsonAsync("/api/User/register", new { Email = email, Password = password });
            var userId = await GetUserIdFromDbAsync(email);
            var token = TestJwtTokenGenerator.GenerateToken(userId);

            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/User")
            {
                Content = JsonContent.Create(new { Password = password })
            };
            request.Headers.Add("Cookie", $"accessToken={token}");

            var response = await client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var cookies = GetSetCookieHeaders(response);
            Assert.Contains(cookies, c => c.StartsWith("accessToken=") && c.Contains("expires="));
        }

        [Fact]
        public async Task DeleteUser_Unauthenticated_Returns401()
        {
            var client = _factory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/User")
            {
                Content = JsonContent.Create(new { Password = "any" })
            };

            var response = await client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_WrongPassword_ReturnsBadRequest()
        {
            var client = _factory.CreateClient();
            var email = UniqueEmail();
            var password = "Password123!";

            await client.PostAsJsonAsync("/api/User/register", new { Email = email, Password = password });
            var userId = await GetUserIdFromDbAsync(email);
            var token = TestJwtTokenGenerator.GenerateToken(userId);

            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/User")
            {
                Content = JsonContent.Create(new { Password = "wrong-password" })
            };
            request.Headers.Add("Cookie", $"accessToken={token}");

            var response = await client.SendAsync(request);

            // UserError.InvalidCreditianals -> ErrorType.Validation -> 400 (see UserController.Problem)
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UnhandledException_Returns500WithProblemBody()
        {
            // Swap the real IUserService for one that always throws, only for this test's client,
            // so we can exercise GlobalExceptionHandler end-to-end without touching Program.cs.
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<IUserService>();
                    services.AddScoped<IUserService, ThrowingUserService>();
                });
            }).CreateClient();

            var response = await client.PostAsJsonAsync("/api/User/register", new
            {
                Email = UniqueEmail(),
                Password = "Password123!"
            });

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<ExceptionResponse>();
            Assert.NotNull(body);
            Assert.Equal(500, body!.statusCode);
            Assert.Equal("Internal server error", body.message);
        }

        private record ExceptionResponse(int statusCode, string message);

        private async Task<Guid> GetUserIdFromDbAsync(string email)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var user = await db.Users.FirstAsync(u => u.Email == email);
            return user.Id;
        }

        private static IEnumerable<string> GetSetCookieHeaders(HttpResponseMessage response)
        {
            return response.Headers.TryGetValues("Set-Cookie", out var values)
                ? values
                : Enumerable.Empty<string>();
        }

        private sealed class ThrowingUserService : IUserService
        {
            public Task<ErrorOr<UserResult>> RegisterAsync(string email, string password, CancellationToken cancellationToken = default)
                => throw new Exception("Test exception");

            public Task<ErrorOr<UserResult>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
                => throw new Exception("Test exception");

            public Task<ErrorOr<DeletedResult>> DeleteAsync(string password, Guid id, CancellationToken cancellationToken = default)
                => throw new Exception("Test exception");
        }
    }
}
