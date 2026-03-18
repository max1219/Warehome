using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Warehome.Domain.Entities;
using Warehome.Infrastructure.Data;
using Warehome.Infrastructure.Data.Entities;
using Warehome.Infrastructure.Data.Repositories;
using ItemType = Warehome.Domain.Entities.ItemType;
using Storage = Warehome.Domain.Entities.Storage;
using DomainItemStock = Warehome.Domain.Entities.ItemStock;

namespace Warehome.Infrastructure.IntegrationTests.Data.Repositories;

public class EfItemStockRepositoryTests
{
    private readonly EfItemStockRepository _repository;
    private readonly AppDbContext _context;

    public EfItemStockRepositoryTests()
    {
        SqliteConnection connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        _context =
            new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options);
        _context.Database.EnsureCreated();
        _repository = new EfItemStockRepository(_context);
    }

    [Fact]
    public async Task Add_WithoutCategories_Success()
    {
        // Arrange
        string itemTypeName = "test type";
        string storageName = "test storage";
        int quantity = 10;

        ItemType itemType = new ItemType { Name = itemTypeName };
        Storage storage = new Storage { Name = storageName };
        DomainItemStock itemStock = new DomainItemStock
        {
            ItemType = itemType,
            Storage = storage,
            Quantity = quantity
        };

        await _context.ItemTypes.AddAsync(new Infrastructure.Data.Entities.ItemType { Name = itemTypeName });
        await _context.Storages.AddAsync(new Infrastructure.Data.Entities.Storage { Name = storageName });
        await _context.SaveChangesAsync();

        // Act
        int id = await _repository.AddAsync(itemStock);

        // Assert
        Infrastructure.Data.Entities.ItemStock? saved = await _context.ItemStocks.FindAsync(id);
        Assert.NotNull(saved);
        Assert.Equal(quantity, saved.Quantity);
    }

    [Fact]
    public async Task Add_WithCategories_Success()
    {
        // Arrange
        string itemTypeName = "test type";
        string storageName = "test storage";
        string categoryPath = "c1/c2/c3";
        int quantity = 10;

        Category<ItemType> itemTypeCategory = new Category<ItemType> { Path = categoryPath };
        Category<Storage> storageCategory = new Category<Storage> { Path = categoryPath };
        ItemType itemType = new ItemType { Name = itemTypeName, Category = itemTypeCategory };
        Storage storage = new Storage { Name = storageName, Category = storageCategory };
        DomainItemStock itemStock = new DomainItemStock
        {
            ItemType = itemType,
            Storage = storage,
            Quantity = quantity
        };

        await _context.ItemTypeCategories.AddAsync(new ItemTypeCategory { Path = categoryPath });
        await _context.StorageCategories.AddAsync(new StorageCategory { Path = categoryPath });
        await _context.SaveChangesAsync();

        EntityEntry<Infrastructure.Data.Entities.ItemType> typeEntity =
            await _context.ItemTypes.AddAsync(new Infrastructure.Data.Entities.ItemType 
                { Name = itemTypeName, CategoryId = (await _context.ItemTypeCategories.FirstAsync()).Id });
        EntityEntry<Infrastructure.Data.Entities.Storage> storageEntity =
            await _context.Storages.AddAsync(new Infrastructure.Data.Entities.Storage 
                { Name = storageName, CategoryId = (await _context.StorageCategories.FirstAsync()).Id });
        await _context.SaveChangesAsync();

        // Act
        int id = await _repository.AddAsync(itemStock);

        // Assert
        Infrastructure.Data.Entities.ItemStock? saved = await _context.ItemStocks
            .Include(s => s.ItemType)
            .Include(s => s.Storage)
            .FirstOrDefaultAsync(s => s.Id == id);
        Assert.NotNull(saved);
        Assert.Equal(quantity, saved.Quantity);
        Assert.Equal(typeEntity.Entity.Id, saved.ItemTypeId);
        Assert.Equal(storageEntity.Entity.Id, saved.StorageId);
    }

    [Fact]
    public async Task Add_ItemTypeNotExists_ThrowsException()
    {
        // Arrange
        string itemTypeName = "test type";
        string storageName = "test storage";
        int quantity = 10;

        ItemType itemType = new ItemType { Name = itemTypeName };
        Storage storage = new Storage { Name = storageName };
        DomainItemStock itemStock = new DomainItemStock
        {
            ItemType = itemType,
            Storage = storage,
            Quantity = quantity
        };

        await _context.Storages.AddAsync(new Infrastructure.Data.Entities.Storage { Name = storageName });
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.AddAsync(itemStock));
    }

    [Fact]
    public async Task Add_StorageNotFound_ThrowsException()
    {
        // Arrange
        string itemTypeName = "test type";
        string storageName = "test storage";
        int quantity = 10;

        ItemType itemType = new ItemType { Name = itemTypeName };
        Storage storage = new Storage { Name = storageName };
        DomainItemStock itemStock = new DomainItemStock
        {
            ItemType = itemType,
            Storage = storage,
            Quantity = quantity
        };

        await _context.ItemTypes.AddAsync(new Infrastructure.Data.Entities.ItemType { Name = itemTypeName });
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.AddAsync(itemStock));
    }

    [Fact]
    public async Task Update_WithoutCategories_Success()
    {
        // Arrange
        string itemTypeName1 = "test type1";
        string itemTypeName2 = "test type2";
        string storageName1 = "test storage1";
        string storageName2 = "test storage2";
        int quantity1 = 10;
        int quantity2 = 20;

        EntityEntry<Infrastructure.Data.Entities.ItemType> type1 =
            await _context.ItemTypes.AddAsync(new Infrastructure.Data.Entities.ItemType { Name = itemTypeName1 });
        EntityEntry<Infrastructure.Data.Entities.ItemType> type2 =
            await _context.ItemTypes.AddAsync(new Infrastructure.Data.Entities.ItemType { Name = itemTypeName2 });
        EntityEntry<Infrastructure.Data.Entities.Storage> storage1 =
            await _context.Storages.AddAsync(new Infrastructure.Data.Entities.Storage { Name = storageName1 });
        EntityEntry<Infrastructure.Data.Entities.Storage> storage2 =
            await _context.Storages.AddAsync(new Infrastructure.Data.Entities.Storage { Name = storageName2 });
        await _context.SaveChangesAsync();

        EntityEntry<Infrastructure.Data.Entities.ItemStock> stock =
            await _context.ItemStocks.AddAsync(new Infrastructure.Data.Entities.ItemStock
            {
                ItemTypeId = type1.Entity.Id,
                StorageId = storage1.Entity.Id,
                Quantity = quantity1
            });
        await _context.SaveChangesAsync();

        ItemType newItemType = new ItemType { Name = itemTypeName2 };
        Storage newStorage = new Storage { Name = storageName2 };
        DomainItemStock updatedStock = new DomainItemStock
        {
            Id = stock.Entity.Id,
            ItemType = newItemType,
            Storage = newStorage,
            Quantity = quantity2
        };

        // Act
        await _repository.UpdateAsync(updatedStock);

        // Assert
        Infrastructure.Data.Entities.ItemStock? saved = await _context.ItemStocks.FindAsync(stock.Entity.Id);
        Assert.NotNull(saved);
        Assert.Equal(quantity2, saved.Quantity);
        Assert.Equal(type2.Entity.Id, saved.ItemTypeId);
        Assert.Equal(storage2.Entity.Id, saved.StorageId);
    }

    [Fact]
    public async Task Update_WithCategories_Success()
    {
        // Arrange
        string itemTypeName = "test type";
        string storageName = "test storage";
        string categoryPath1 = "c1/c2/c3";
        string categoryPath2 = "c1/c2/c4";
        int quantity1 = 10;
        int quantity2 = 20;

        EntityEntry<ItemTypeCategory> category1 =
            await _context.ItemTypeCategories.AddAsync(new ItemTypeCategory { Path = categoryPath1 });
        EntityEntry<ItemTypeCategory> category2 =
            await _context.ItemTypeCategories.AddAsync(new ItemTypeCategory { Path = categoryPath2 });
        EntityEntry<StorageCategory> storageCategory1 =
            await _context.StorageCategories.AddAsync(new StorageCategory { Path = categoryPath1 });
        EntityEntry<StorageCategory> storageCategory2 =
            await _context.StorageCategories.AddAsync(new StorageCategory { Path = categoryPath2 });
        await _context.SaveChangesAsync();

        EntityEntry<Infrastructure.Data.Entities.ItemType> type1 =
            await _context.ItemTypes.AddAsync(new Infrastructure.Data.Entities.ItemType 
                { Name = itemTypeName, CategoryId = category1.Entity.Id });
        EntityEntry<Infrastructure.Data.Entities.ItemType> type2 =
            await _context.ItemTypes.AddAsync(new Infrastructure.Data.Entities.ItemType 
                { Name = itemTypeName, CategoryId = category2.Entity.Id });
        EntityEntry<Infrastructure.Data.Entities.Storage> storage1 =
            await _context.Storages.AddAsync(new Infrastructure.Data.Entities.Storage 
                { Name = storageName, CategoryId = storageCategory1.Entity.Id });
        EntityEntry<Infrastructure.Data.Entities.Storage> storage2 =
            await _context.Storages.AddAsync(new Infrastructure.Data.Entities.Storage 
                { Name = storageName, CategoryId = storageCategory2.Entity.Id });
        await _context.SaveChangesAsync();

        EntityEntry<Infrastructure.Data.Entities.ItemStock> stock =
            await _context.ItemStocks.AddAsync(new Infrastructure.Data.Entities.ItemStock
            {
                ItemTypeId = type1.Entity.Id,
                StorageId = storage1.Entity.Id,
                Quantity = quantity1
            });
        await _context.SaveChangesAsync();

        Category<ItemType> newItemTypeCategory = new Category<ItemType> { Path = categoryPath2 };
        Category<Storage> newStorageCategory = new Category<Storage> { Path = categoryPath2 };
        ItemType newItemType = new ItemType { Name = itemTypeName, Category = newItemTypeCategory };
        Storage newStorage = new Storage { Name = storageName, Category = newStorageCategory };
        DomainItemStock updatedStock = new DomainItemStock
        {
            Id = stock.Entity.Id,
            ItemType = newItemType,
            Storage = newStorage,
            Quantity = quantity2
        };

        // Act
        await _repository.UpdateAsync(updatedStock);

        // Assert
        Infrastructure.Data.Entities.ItemStock? saved = await _context.ItemStocks.FindAsync(stock.Entity.Id);
        Assert.NotNull(saved);
        Assert.Equal(quantity2, saved.Quantity);
        Assert.Equal(type2.Entity.Id, saved.ItemTypeId);
        Assert.Equal(storage2.Entity.Id, saved.StorageId);
    }

    [Fact]
    public async Task Update_ItemTypeNotFound_ThrowsException()
    {
        // Arrange
        string itemTypeName1 = "test type1";
        string itemTypeName2 = "test type2";
        string storageName = "test storage";
        int quantity = 10;

        EntityEntry<Infrastructure.Data.Entities.ItemType> type1 =
            await _context.ItemTypes.AddAsync(new Infrastructure.Data.Entities.ItemType { Name = itemTypeName1 });
        EntityEntry<Infrastructure.Data.Entities.Storage> storage =
            await _context.Storages.AddAsync(new Infrastructure.Data.Entities.Storage { Name = storageName });
        await _context.SaveChangesAsync();

        EntityEntry<Infrastructure.Data.Entities.ItemStock> stock =
            await _context.ItemStocks.AddAsync(new Infrastructure.Data.Entities.ItemStock
            {
                ItemTypeId = type1.Entity.Id,
                StorageId = storage.Entity.Id,
                Quantity = quantity
            });
        await _context.SaveChangesAsync();

        ItemType newItemType = new ItemType { Name = itemTypeName2 };
        Storage newStorage = new Storage { Name = storageName };
        DomainItemStock updatedStock = new DomainItemStock
        {
            Id = stock.Entity.Id,
            ItemType = newItemType,
            Storage = newStorage,
            Quantity = quantity
        };

        // Act & Assert
        Exception exception = await Assert.ThrowsAsync<Exception>(() => _repository.UpdateAsync(updatedStock));
        Assert.Equal("New itemType not found", exception.Message);
    }

    [Fact]
    public async Task Update_StorageNotFound_ThrowsException()
    {
        // Arrange
        string itemTypeName = "test type";
        string storageName1 = "test storage1";
        string storageName2 = "test storage2";
        int quantity = 10;

        EntityEntry<Infrastructure.Data.Entities.ItemType> type =
            await _context.ItemTypes.AddAsync(new Infrastructure.Data.Entities.ItemType { Name = itemTypeName });
        EntityEntry<Infrastructure.Data.Entities.Storage> storage1 =
            await _context.Storages.AddAsync(new Infrastructure.Data.Entities.Storage { Name = storageName1 });
        await _context.SaveChangesAsync();

        EntityEntry<Infrastructure.Data.Entities.ItemStock> stock =
            await _context.ItemStocks.AddAsync(new Infrastructure.Data.Entities.ItemStock
            {
                ItemTypeId = type.Entity.Id,
                StorageId = storage1.Entity.Id,
                Quantity = quantity
            });
        await _context.SaveChangesAsync();

        ItemType newItemType = new ItemType { Name = itemTypeName };
        Storage newStorage = new Storage { Name = storageName2 };
        DomainItemStock updatedStock = new DomainItemStock
        {
            Id = stock.Entity.Id,
            ItemType = newItemType,
            Storage = newStorage,
            Quantity = quantity
        };

        // Act & Assert
        Exception exception = await Assert.ThrowsAsync<Exception>(() => _repository.UpdateAsync(updatedStock));
        Assert.Equal("New storage not found", exception.Message);
    }

    [Fact]
    public async Task Delete_Success()
    {
        // Arrange
        string itemTypeName = "test type";
        string storageName = "test storage";
        int quantity = 10;

        EntityEntry<Infrastructure.Data.Entities.ItemType> type =
            await _context.ItemTypes.AddAsync(new Infrastructure.Data.Entities.ItemType { Name = itemTypeName });
        EntityEntry<Infrastructure.Data.Entities.Storage> storage =
            await _context.Storages.AddAsync(new Infrastructure.Data.Entities.Storage { Name = storageName });
        await _context.SaveChangesAsync();

        EntityEntry<Infrastructure.Data.Entities.ItemStock> stock =
            await _context.ItemStocks.AddAsync(new Infrastructure.Data.Entities.ItemStock
            {
                ItemTypeId = type.Entity.Id,
                StorageId = storage.Entity.Id,
                Quantity = quantity
            });
        await _context.SaveChangesAsync();

        ItemType itemType = new ItemType { Name = itemTypeName };
        Storage domainStorage = new Storage { Name = storageName };
        DomainItemStock itemStock = new DomainItemStock
        {
            Id = stock.Entity.Id,
            ItemType = itemType,
            Storage = domainStorage,
            Quantity = quantity
        };

        // Act
        await _repository.DeleteAsync(itemStock);

        // Assert
        Infrastructure.Data.Entities.ItemStock? deleted = await _context.ItemStocks.FindAsync(stock.Entity.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task GetById_DoesNotExist_ReturnsNull()
    {
        // Act
        DomainItemStock? result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllByStorage_HasWrong_ReturnsCorrect()
    {
        // Arrange
        string itemTypeName1 = "type1";
        string itemTypeName2 = "type2";
        string storageName = "test storage";
        string wrongStorageName = "wrong storage";
        int quantity1 = 10;
        int quantity2 = 20;

        EntityEntry<Infrastructure.Data.Entities.ItemType> type1 =
            await _context.ItemTypes.AddAsync(new Infrastructure.Data.Entities.ItemType { Name = itemTypeName1 });
        EntityEntry<Infrastructure.Data.Entities.ItemType> type2 =
            await _context.ItemTypes.AddAsync(new Infrastructure.Data.Entities.ItemType { Name = itemTypeName2 });
        EntityEntry<Infrastructure.Data.Entities.Storage> storage =
            await _context.Storages.AddAsync(new Infrastructure.Data.Entities.Storage { Name = storageName });
        EntityEntry<Infrastructure.Data.Entities.Storage> wrongStorage =
            await _context.Storages.AddAsync(new Infrastructure.Data.Entities.Storage { Name = wrongStorageName });
        await _context.SaveChangesAsync();

        await _context.ItemStocks.AddRangeAsync(
            new Infrastructure.Data.Entities.ItemStock
            {
                ItemTypeId = type1.Entity.Id,
                StorageId = storage.Entity.Id,
                Quantity = quantity1
            },
            new Infrastructure.Data.Entities.ItemStock
            {
                ItemTypeId = type2.Entity.Id,
                StorageId = storage.Entity.Id,
                Quantity = quantity2
            },
            new Infrastructure.Data.Entities.ItemStock
            {
                ItemTypeId = type1.Entity.Id,
                StorageId = wrongStorage.Entity.Id,
                Quantity = quantity1
            });
        await _context.SaveChangesAsync();

        Storage domainStorage = new Storage { Name = storageName };

        // Act
        IAsyncEnumerable<DomainItemStock> result = _repository.GetAllByStorageAsync(domainStorage);

        // Assert
        List<DomainItemStock> items = await result.ToListAsync();
        Assert.Equal(2, items.Count);
        Assert.Contains(items, i => i.ItemType.Name == itemTypeName1 && i.Quantity == quantity1);
        Assert.Contains(items, i => i.ItemType.Name == itemTypeName2 && i.Quantity == quantity2);
        Assert.All(items, i => Assert.Equal(storageName, i.Storage.Name));
    }

    [Fact]
    public async Task GetAllByStorage_StorageNotFound_ThrowsException()
    {
        // Arrange
        Storage domainStorage = new Storage { Name = "nonexistent" };

        // Act & Assert
        Exception exception = await Assert.ThrowsAsync<Exception>(
            async () => await _repository.GetAllByStorageAsync(domainStorage).ToListAsync());
        Assert.Equal("Storage not found", exception.Message);
    }

    [Fact]
    public async Task GetAllByType_HasWrong_ReturnsCorrect()
    {
        // Arrange
        string itemTypeName = "test type";
        string storageName1 = "storage1";
        string storageName2 = "storage2";
        string wrongTypeName = "wrong type";
        int quantity1 = 10;
        int quantity2 = 20;

        EntityEntry<Infrastructure.Data.Entities.ItemType> type =
            await _context.ItemTypes.AddAsync(new Infrastructure.Data.Entities.ItemType { Name = itemTypeName });
        EntityEntry<Infrastructure.Data.Entities.ItemType> wrongType =
            await _context.ItemTypes.AddAsync(new Infrastructure.Data.Entities.ItemType { Name = wrongTypeName });
        EntityEntry<Infrastructure.Data.Entities.Storage> storage1 =
            await _context.Storages.AddAsync(new Infrastructure.Data.Entities.Storage { Name = storageName1 });
        EntityEntry<Infrastructure.Data.Entities.Storage> storage2 =
            await _context.Storages.AddAsync(new Infrastructure.Data.Entities.Storage { Name = storageName2 });
        await _context.SaveChangesAsync();

        await _context.ItemStocks.AddRangeAsync(
            new Infrastructure.Data.Entities.ItemStock
            {
                ItemTypeId = type.Entity.Id,
                StorageId = storage1.Entity.Id,
                Quantity = quantity1
            },
            new Infrastructure.Data.Entities.ItemStock
            {
                ItemTypeId = type.Entity.Id,
                StorageId = storage2.Entity.Id,
                Quantity = quantity2
            },
            new Infrastructure.Data.Entities.ItemStock
            {
                ItemTypeId = wrongType.Entity.Id,
                StorageId = storage1.Entity.Id,
                Quantity = quantity1
            });
        await _context.SaveChangesAsync();

        ItemType domainItemType = new ItemType { Name = itemTypeName };

        // Act
        IAsyncEnumerable<DomainItemStock> result = _repository.GetAllByTypeAsync(domainItemType);

        // Assert
        List<DomainItemStock> items = await result.ToListAsync();
        Assert.Equal(2, items.Count);
        Assert.Contains(items, i => i.Storage.Name == storageName1 && i.Quantity == quantity1);
        Assert.Contains(items, i => i.Storage.Name == storageName2 && i.Quantity == quantity2);
        Assert.All(items, i => Assert.Equal(itemTypeName, i.ItemType.Name));
    }
    
    [Fact]
    public async Task GetAllByType_TypeNotFound_ThrowsException()
    {
        // Arrange
        ItemType domainItemType = new ItemType { Name = "nonexistent" };

        // Act & Assert
        Exception exception = await Assert.ThrowsAsync<Exception>(
            async () => await _repository.GetAllByTypeAsync(domainItemType).ToListAsync());
        Assert.Equal("Type not found", exception.Message);
    }
}