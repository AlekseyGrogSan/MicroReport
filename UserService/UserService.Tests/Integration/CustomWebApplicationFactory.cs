using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using UserService.Data;

namespace UserService.Tests.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private SqliteConnection? _connection;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Program.cs must skip dbContext.Database.Migrate() when running under
            // this environment name (see instructions.md).
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((_, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "DataSource=:memory:",
                    ["JWTSettings:SecretKey"] = TestJwtTokenGenerator.SecretKey,
                    ["JWTSettings:Issuer"] = TestJwtTokenGenerator.Issuer,
                    ["JWTSettings:Audience"] = TestJwtTokenGenerator.Audience
                });
            });

            builder.ConfigureServices(services =>
            {
                // AddDbContext<AppDbContext>(UseNpgsql) in Program.cs registers more than just
                // DbContextOptions<AppDbContext> — it also registers an
                // IDbContextOptionsConfiguration<AppDbContext> entry that holds the UseNpgsql(...)
                // delegate. If we only remove DbContextOptions<AppDbContext>, that Npgsql
                // configuration is still applied on top of our UseSqlite(...) call below, and EF
                // Core throws because two providers end up configured on the same context.
                // So: remove everything related to AppDbContext's options before re-adding it.
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.RemoveAll<DbContextOptions>();
                services.RemoveAll<AppDbContext>();
                services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();

                // Keep one open connection alive for the lifetime of the factory so the
                // in-memory SQLite database isn't dropped between requests.
                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });

                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _connection?.Dispose();
        }
    }
}
