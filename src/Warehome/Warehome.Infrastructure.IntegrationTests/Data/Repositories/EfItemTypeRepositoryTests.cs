using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Warehome.Domain.Entities;
using Warehome.Infrastructure.Data;
using Warehome.Infrastructure.Data.Entities;
using Warehome.Infrastructure.Data.Repositories;
using ItemType = Warehome.Domain.Entities.ItemType;

namespace Warehome.Infrastructure.IntegrationTests.Data.Repositories;

public class EfItemTypeRepositoryTests
{
    private readonly EfItemTypeRepository _repository;
    private readonly AppDbContext _context;

    public EfItemTypeRepositoryTests()
    {
        SqliteConnection connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        _context =
            new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options);
        _context.Database.EnsureCreated();
        _repository = new EfItemTypeRepository(_context);
    }

    [Fact]
    public async Task Add_WithoutCategory_Success()
    {
        // Arrange
        string itemTypeName1 = "test1";
        string itemTypeName2 = "test2";
        ItemType itemType2 = new ItemType { Name = itemTypeName2 };
        await _context.ItemTypes.AddAsync(new Infrastructure.Data.Entities.ItemType() { Name = itemTypeName1 });
        await _context.SaveChangesAsync();
        
        // Act & Assert
        await _repository.AddAsync(itemType2);
    }
    
    [Fact]
    public async Task Add_WithCategory_Success()
    {
        // Arrange
        string itemTypeName = "test1";
        string categoryPath = "c1/c2/c3";
        Category<ItemType> category = new Category<ItemType> { Path = categoryPath };
        ItemType itemType = new ItemType { Name = itemTypeName, Category = category};
        await _context.ItemTypeCategories.AddAsync(new ItemTypeCategory { Path = categoryPath });
        await _context.SaveChangesAsync();
        
        // Act & Assert
        await _repository.AddAsync(itemType);
        
    }    
    
    [Fact]
    public async Task Add_WithCategory_SavedWithoutChanges()
    {
        // Arrange
        string itemTypeName = "test1";
        string categoryPath = "c1/c2/c3";
        Category<ItemType> category = new Category<ItemType> { Path = categoryPath };
        ItemType itemType = new ItemType { Name = itemTypeName, Category = category};
        await _context.ItemTypeCategories.AddAsync(new ItemTypeCategory { Path = categoryPath });
        await _context.SaveChangesAsync();

        // Act
        await _repository.AddAsync(itemType);
        ItemType? result = await _repository.GetAsync(itemTypeName, category);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(itemTypeName, result.Name);
        Assert.NotNull(result.Category);
        Assert.Equal(categoryPath, result.Category.Path);
    }

    [Fact]
    public async Task Add_SameNameInDifferentCategories_Success()
    {
        // Arrange
        string itemTypeName = "test";
        string categoryPath1 = "c1/c2/c3";
        string categoryPath2 = "c1/c2/c4";
        Category<ItemType> category1 = new Category<ItemType> { Path = categoryPath1 };
        Category<ItemType> category2 = new Category<ItemType> { Path = categoryPath2 };
        ItemType itemType1 = new ItemType { Name = itemTypeName, Category = category1 };
        ItemType itemType2 = new ItemType { Name = itemTypeName, Category = category2 };
        
        await _context.ItemTypeCategories.AddAsync(new ItemTypeCategory { Path = categoryPath1 });
        await _context.ItemTypeCategories.AddAsync(new ItemTypeCategory { Path = categoryPath2 });
        await _context.SaveChangesAsync();

        // Act & Assert
        await _repository.AddAsync(itemType1);
        await _repository.AddAsync(itemType2);
    }
    
    [Fact]
    public async Task Add_AlreadyExists_ThrowsException()
    {
        // Arrange
        string itemTypeName = "test";
        ItemType itemType = new ItemType { Name = itemTypeName };
        await _context.ItemTypes.AddAsync(new Infrastructure.Data.Entities.ItemType { Name = itemTypeName });
        await _context.SaveChangesAsync();
        
        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _repository.AddAsync(itemType));
    }

    [Fact]
    public async Task Get_DoesNotExist_ReturnsNull()
    {
        // Arrange
        string itemTypeName1 = "test1";
        string itemTypeName2 = "test2";
        
        await _context.ItemTypes.AddAsync(new Infrastructure.Data.Entities.ItemType { Name = itemTypeName1 });
        
        // Act
        ItemType? actual = await _repository.GetAsync(itemTypeName2, null);
        
        // Assert
        Assert.Null(actual);
    }
    
    [Fact]
    public async Task GetAllByCategory_WithCategory()
    {
        // Arrange
        string categoryPath = "categoryPath";
        string wrongCategoryPath = "wrongCategoryPath";
        string[] itemTypeNames = ["child1", "child2", "child3"];
        string[] wrongItemTypeNames = ["wrong1", "wrong2"];

        EntityEntry<ItemTypeCategory> category =
            await _context.ItemTypeCategories.AddAsync(new ItemTypeCategory { Path = categoryPath });
        EntityEntry<ItemTypeCategory> wrongCategory =
            await _context.ItemTypeCategories.AddAsync(new ItemTypeCategory { Path = wrongCategoryPath });
        await _context.SaveChangesAsync();
        await _context.ItemTypes.AddRangeAsync(itemTypeNames.Select(
            name => new Infrastructure.Data.Entities.ItemType { Name = name, CategoryId = category.Entity.Id }));
        await _context.ItemTypes.AddRangeAsync(wrongItemTypeNames.Select(
            name => new Infrastructure.Data.Entities.ItemType { Name = name, CategoryId = wrongCategory.Entity.Id }));
        await _context.SaveChangesAsync();

        // Act
        IAsyncEnumerable<ItemType> result =
            _repository.GetAllByCategoryAsync(new Category<ItemType> { Path = categoryPath });

        // Assert
        Assert.Equal(
            itemTypeNames.Order(),
            result.Select(itemType => itemType.Name).Order()
        );
    }    
    
    [Fact]
    public async Task GetAllByCategory_WithOutCategory()
    {
        // Arrange
        string wrongCategoryPath = "wrongCategoryPath";
        string[] itemTypeNames = ["child1", "child2", "child3"];
        string[] wrongItemTypeNames = ["wrong1", "wrong2"];

        EntityEntry<ItemTypeCategory> wrongCategory =
            await _context.ItemTypeCategories.AddAsync(new ItemTypeCategory { Path = wrongCategoryPath });
        await _context.SaveChangesAsync();
        await _context.ItemTypes.AddRangeAsync(itemTypeNames.Select(
            name => new Infrastructure.Data.Entities.ItemType { Name = name }));
        await _context.ItemTypes.AddRangeAsync(wrongItemTypeNames.Select(
            name => new Infrastructure.Data.Entities.ItemType { Name = name, CategoryId = wrongCategory.Entity.Id }));
        await _context.SaveChangesAsync();

        // Act
        IAsyncEnumerable<ItemType> result =
            _repository.GetAllByCategoryAsync(null);

        // Assert
        Assert.Equal(
            itemTypeNames.Order(),
            result.Select(itemType => itemType.Name).Order()
        );
    }
}