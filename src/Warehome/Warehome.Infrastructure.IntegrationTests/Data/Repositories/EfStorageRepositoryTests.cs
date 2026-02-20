using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Warehome.Domain.Entities;
using Warehome.Infrastructure.Data;
using Warehome.Infrastructure.Data.Entities;
using Warehome.Infrastructure.Data.Repositories;
using Storage = Warehome.Domain.Entities.Storage;

namespace Warehome.Infrastructure.IntegrationTests.Data.Repositories;

public class EfStorageRepositoryTests
{
    private readonly EfStorageRepository _repository;
    private readonly AppDbContext _context;

    public EfStorageRepositoryTests()
    {
        SqliteConnection connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        _context =
            new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options);
        _context.Database.EnsureCreated();
        _repository = new EfStorageRepository(_context);
    }

    [Fact]
    public async Task Add_WithoutCategory_Success()
    {
        // Arrange
        string storageName1 = "test1";
        string storageName2 = "test2";
        Storage storage2 = new Storage { Name = storageName2 };
        await _context.Storages.AddAsync(new Infrastructure.Data.Entities.Storage() { Name = storageName1 });
        await _context.SaveChangesAsync();
        
        // Act & Assert
        await _repository.AddAsync(storage2);
    }
    
    [Fact]
    public async Task Add_WithCategory_Success()
    {
        // Arrange
        string storageName = "test1";
        string categoryPath = "c1/c2/c3";
        Category<Storage> category = new Category<Storage> { Path = categoryPath };
        Storage storage = new Storage { Name = storageName, Category = category};
        await _context.StorageCategories.AddAsync(new StorageCategory { Path = categoryPath });
        await _context.SaveChangesAsync();
        
        // Act & Assert
        await _repository.AddAsync(storage);
        
    }    
    
    [Fact]
    public async Task Add_WithCategory_SavedWithoutChanges()
    {
        // Arrange
        string storageName = "test1";
        string categoryPath = "c1/c2/c3";
        Category<Storage> category = new Category<Storage> { Path = categoryPath };
        Storage storage = new Storage { Name = storageName, Category = category};
        await _context.StorageCategories.AddAsync(new StorageCategory { Path = categoryPath });
        await _context.SaveChangesAsync();

        // Act
        await _repository.AddAsync(storage);
        Storage? result = await _repository.GetAsync(storageName, category);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(storageName, result.Name);
        Assert.NotNull(result.Category);
        Assert.Equal(categoryPath, result.Category.Path);
    }

    [Fact]
    public async Task Add_SameNameInDifferentCategories_Success()
    {
        // Arrange
        string storageName = "test";
        string categoryPath1 = "c1/c2/c3";
        string categoryPath2 = "c1/c2/c4";
        Category<Storage> category1 = new Category<Storage> { Path = categoryPath1 };
        Category<Storage> category2 = new Category<Storage> { Path = categoryPath2 };
        Storage storage1 = new Storage { Name = storageName, Category = category1 };
        Storage storage2 = new Storage { Name = storageName, Category = category2 };
        
        await _context.StorageCategories.AddAsync(new StorageCategory { Path = categoryPath1 });
        await _context.StorageCategories.AddAsync(new StorageCategory { Path = categoryPath2 });
        await _context.SaveChangesAsync();

        // Act & Assert
        await _repository.AddAsync(storage1);
        await _repository.AddAsync(storage2);
    }
    
    [Fact]
    public async Task Add_AlreadyExists_ThrowsException()
    {
        // Arrange
        string storageName = "test";
        Storage storage = new Storage { Name = storageName };
        await _context.Storages.AddAsync(new Infrastructure.Data.Entities.Storage { Name = storageName });
        await _context.SaveChangesAsync();
        
        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _repository.AddAsync(storage));
    }

    [Fact]
    public async Task Get_DoesNotExist_ReturnsNull()
    {
        // Arrange
        string storageName1 = "test1";
        string storageName2 = "test2";
        
        await _context.Storages.AddAsync(new Infrastructure.Data.Entities.Storage { Name = storageName1 });
        
        // Act
        Storage? actual = await _repository.GetAsync(storageName2, null);
        
        // Assert
        Assert.Null(actual);
    }
}