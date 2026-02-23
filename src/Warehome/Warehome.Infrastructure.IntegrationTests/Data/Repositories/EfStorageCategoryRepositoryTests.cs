using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        Category<Storage> category2 = new Category<Storage> { Path = path2 };
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

        Category<Storage> parent = new Category<Storage> { Path = parentPath };
        Category<Storage> category = new Category<Storage> { Path = path };

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
        Category<Storage> category = new Category<Storage> { Path = path };
        await _context.StorageCategories.AddAsync(new StorageCategory { Path = path });
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _repository.AddAsync(category, null));
    }

    [Fact]
    public async Task GetAllByParent_WithParentNotRecursive()
    {
        // Arrange
        string parentPath = "parentPath";
        string wrongParentPath = "wrongParentPath";
        string[] childNames = ["child1", "child2", "child3"];
        string[] childPaths = childNames.Select(name => $"{parentPath}/{name}").ToArray();
        string[] wrongChildPaths = childNames.Select(name => $"{wrongParentPath}/{name}").ToArray();

        EntityEntry<StorageCategory> parent =
            await _context.StorageCategories.AddAsync(new StorageCategory { Path = parentPath });
        EntityEntry<StorageCategory> wrongParent =
            await _context.StorageCategories.AddAsync(new StorageCategory { Path = wrongParentPath });
        await _context.SaveChangesAsync();
        await _context.StorageCategories.AddRangeAsync(childPaths.Select(
            path => new StorageCategory { Path = path, ParentId = parent.Entity.Id }));
        await _context.StorageCategories.AddRangeAsync(wrongChildPaths.Select(
            path => new StorageCategory { Path = path, ParentId = wrongParent.Entity.Id }));
        await _context.SaveChangesAsync();

        // Act
        IAsyncEnumerable<Category<Storage>> result =
            _repository.GetAllByParentAsync(new Category<Storage> { Path = parentPath }, false);

        // Assert
        Assert.Equal(
            childPaths.Order(),
            result.Select(category => category.Path).Order()
        );
    }
    
    [Fact]
    public async Task GetAllByParent_WithoutParentNotRecursive()
    {
        // Arrange
        string wrongParentPath = "wrongParentPath";
        string[] childNames = ["child1", "child2", "child3"];
        string[] wrongChildPaths = childNames.Select(name => $"{wrongParentPath}/{name}").ToArray();

        EntityEntry<StorageCategory> wrongParent =
            await _context.StorageCategories.AddAsync(new StorageCategory { Path = wrongParentPath });
        await _context.SaveChangesAsync();
        await _context.StorageCategories.AddRangeAsync(childNames.Select(
            path => new StorageCategory { Path = path}));
        await _context.StorageCategories.AddRangeAsync(wrongChildPaths.Select(
            path => new StorageCategory { Path = path, ParentId = wrongParent.Entity.Id }));
        await _context.SaveChangesAsync();

        // Act
        IAsyncEnumerable<Category<Storage>> result =
            _repository.GetAllByParentAsync(null, false);

        // Assert
        Assert.Equal(
            childNames.Concat([wrongParentPath]).Order(),
            result.Select(category => category.Path).Order()
        );
    }  
    
    [Fact]
    public async Task GetAllByParent_WithoutParentRecursive()
    {
        // Arrange
        string wrongParentPath = "wrongParentPath";
        string[] childNames = ["child1", "child2", "child3"];
        string[] wrongChildPaths = childNames.Select(name => $"{wrongParentPath}/{name}").ToArray();

        EntityEntry<StorageCategory> wrongParent =
            await _context.StorageCategories.AddAsync(new StorageCategory { Path = wrongParentPath });
        await _context.SaveChangesAsync();
        await _context.StorageCategories.AddRangeAsync(childNames.Select(
            path => new StorageCategory { Path = path}));
        await _context.StorageCategories.AddRangeAsync(wrongChildPaths.Select(
            path => new StorageCategory { Path = path, ParentId = wrongParent.Entity.Id }));
        await _context.SaveChangesAsync();

        // Act
        IAsyncEnumerable<Category<Storage>> result =
            _repository.GetAllByParentAsync(null, true);

        // Assert
        Assert.Equal(
            childNames.Concat([wrongParentPath]).Concat(wrongChildPaths).Order(),
            result.Select(category => category.Path).Order()
        );
    }
}