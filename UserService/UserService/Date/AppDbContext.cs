using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using UserService.Core.Models;

namespace UserService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users {  get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Автоматически применяет все IEntityTypeConfiguration из сборки
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}