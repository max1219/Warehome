using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Warehome.Domain.Entities;
using Warehome.Infrastructure.Data;
using Warehome.Infrastructure.Data.Repositories;

namespace Warehome.Infrastructure.IntegrationTests.Data.Repositories;

public class EfStorageRepositoryTests
{
    private readonly EfStorageRepository _repository;

    public EfStorageRepositoryTests()
    {
        SqliteConnection connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        AppDbContext context =
            new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options);
        context.Database.EnsureCreated();
        _repository = new EfStorageRepository(context);
    }

    [Fact]
    public async Task Add_Success()
    {
        // Arrange
        string storageName1 = "test1";
        string storageName2 = "test2";
        Storage storage1 = new Storage { Name = storageName1 };
        Storage storage2 = new Storage { Name = storageName2 };
        await _repository.TryAddAsync(storage1);
        
        // Act
        bool result = await _repository.TryAddAsync(storage2);
        
        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public async Task Add_AlreadyExists_ReturnsFalse()
    {
        // Arrange
        string storageName = "test";
        Storage storage = new Storage { Name = storageName };
        await _repository.TryAddAsync(storage);

        // Act
        bool result = await _repository.TryAddAsync(storage);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetByPath_Exists_ReturnsStorage()
    {
        // Arrange
        string storageName1 = "test1";
        string storageName2 = "test2";
        Storage storage1 = new Storage { Name = storageName1 };
        Storage storage2 = new Storage { Name = storageName2 };

        await _repository.TryAddAsync(storage1);
        await _repository.TryAddAsync(storage2);
        
        // Act
        Storage? actual = await _repository.GetByPathAsync(storageName1);
        
        // Assert
        Assert.NotNull(actual);
        Assert.Equal(storageName1, actual.Name);
    }

    [Fact]
    public async Task GetByPath_DoesNotExist_ReturnsNull()
    {
        // Arrange
        string storageName1 = "test1";
        string storageName2 = "test2";
        Storage storage1 = new Storage { Name = storageName1 };
        
        await _repository.TryAddAsync(storage1);
        
        // Act
        Storage? actual = await _repository.GetByPathAsync(storageName2);
        
        // Assert
        Assert.Null(actual);
    }
}