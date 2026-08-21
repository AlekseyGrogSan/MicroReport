using Microsoft.EntityFrameworkCore;
using DocumentService.Core.Entities;
using System.Reflection;

namespace DocumentService.Date
{
    public class DocumentDbContext : DbContext
    {
        public DocumentDbContext(DbContextOptions<DocumentDbContext> options) : base(options) { }

        public DbSet<Document> Documents { get; set; }
        public DbSet<ReportRequest> ReportRequests { get; set; }
        public DbSet<ReportRequestDocument> ReportRequestDocuments { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

    }
}
