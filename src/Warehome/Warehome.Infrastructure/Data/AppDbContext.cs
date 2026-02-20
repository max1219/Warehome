using Microsoft.EntityFrameworkCore;
using Warehome.Infrastructure.Data.Entities;

namespace Warehome.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Storage> Storages => Set<Storage>();
    public DbSet<StorageCategory> StorageCategories => Set<StorageCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Storage>()
            .HasOne(s => s.Category)
            .WithMany()
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<StorageCategory>()
            .HasOne(c => c.Parent)
            .WithMany()
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Storage>()
            .HasIndex(s => new { s.Name, s.CategoryId })
            .IsUnique()
            .HasFilter("CategoryId IS NOT NULL"); 

        modelBuilder.Entity<Storage>()
            .HasIndex(s => s.Name)
            .IsUnique()
            .HasFilter("CategoryId IS NULL");
        
        modelBuilder.Entity<StorageCategory>()
            .HasIndex(c => c.Path)
            .IsUnique();
    }
}