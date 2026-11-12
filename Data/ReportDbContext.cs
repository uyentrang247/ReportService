using Microsoft.EntityFrameworkCore;
using ReportService.Models;

namespace ReportService.Data;

public class ReportDbContext : DbContext
{
    public ReportDbContext(DbContextOptions<ReportDbContext> options)
        : base(options)
    {
    }

    public DbSet<OrderReport> OrderReports => Set<OrderReport>();
    public DbSet<ProductReport> ProductReports => Set<ProductReport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderReport>()
            .HasMany(o => o.ProductReports)
            .WithOne(p => p.OrderReport)
            .HasForeignKey(p => p.OrderReportId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

