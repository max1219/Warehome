using Microsoft.EntityFrameworkCore;
using Warehome.Infrastructure.Data.Entities;

namespace Warehome.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Storage> Storages => Set<Storage>();
    public DbSet<StorageCategory> StorageCategories => Set<StorageCategory>();
    
    public DbSet<ItemType> ItemTypes => Set<ItemType>();
    public DbSet<ItemTypeCategory> ItemTypeCategories => Set<ItemTypeCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureStorageEntities(modelBuilder);
        ConfigureItemTypeEntities(modelBuilder);
    }

    private static void ConfigureStorageEntities(ModelBuilder modelBuilder)
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

    private static void ConfigureItemTypeEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemType>()
            .HasOne(i => i.Category)
            .WithMany()
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<ItemTypeCategory>()
            .HasOne(c => c.Parent)
            .WithMany()
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<ItemType>()
            .HasIndex(i => new { i.Name, i.CategoryId })
            .IsUnique()
            .HasFilter("CategoryId IS NOT NULL"); 

        modelBuilder.Entity<ItemType>()
            .HasIndex(i => i.Name)
            .IsUnique()
            .HasFilter("CategoryId IS NULL");
        
        modelBuilder.Entity<ItemTypeCategory>()
            .HasIndex(c => c.Path)
            .IsUnique();
    }
}