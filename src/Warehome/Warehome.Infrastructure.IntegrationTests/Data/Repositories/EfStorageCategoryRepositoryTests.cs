using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Warehome.Domain.Entities;
using Warehome.Infrastructure.Data;
using Warehome.Infrastructure.Data.Entities;
using Warehome.Infrastructure.Data.Repositories;
using Storage = Warehome.Domain.Entities.Storage;

namespace Warehome.Infrastructure.IntegrationTests.Data.Repositories;

public class EfStorageCategoryRepositoryTests
{
    private readonly EfStorageCategoryRepository _repository;
    private readonly AppDbContext _context;

    public EfStorageCategoryRepositoryTests()
    {
        SqliteConnection connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        _context =
            new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options);
        _context.Database.EnsureCreated();
        _repository = new EfStorageCategoryRepository(_context);
    }

    [Fact]
    public async Task Add_WithoutParent_Success()
    {
        // Arrange
        string path1 = "path1";
        string path2 = "path2";
        
        Category<Storage> category2 = new Category<Storage> {Path = path2};
        await _context.StorageCategories.AddAsync(new StorageCategory { Path = path1 });
        await _context.SaveChangesAsync();
        
        // Act & Assert
        await _repository.AddAsync(category2, null);
    }

    [Fact]
    public async Task Add_WithParent_Success()
    {
        // Arrange
        string parentPath = "parentPath";
        string path = $"{parentPath}/name";
        
        Category<Storage> parent = new Category<Storage> {Path = parentPath};
        Category<Storage> category = new Category<Storage> {Path = path};
        
        await _context.StorageCategories.AddAsync(new StorageCategory { Path = parentPath });
        await _context.SaveChangesAsync();
        
        // Act & Assert
        await _repository.AddAsync(category, parent);
    }

    [Fact]
    public async Task Add_AlreadyExists_ReturnFalse()
    {
        // Arrange
        string path = "path";
        Category<Storage> category = new Category<Storage> {Path = path};
        await _context.StorageCategories.AddAsync(new StorageCategory { Path = path });
        await _context.SaveChangesAsync();
        
        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _repository.AddAsync(category, null));
    }

}