using Moq;
using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;
using Warehome.Application.Repositories;
using Warehome.Application.Services.Implementations;
using Warehome.Domain.Entities;

namespace Warehome.Application.UnitTests.Services;

public class ItemStockServiceTests
{
    [Fact]
    public async Task Create_WithoutCategories_SuccessAndIdNotNull()
    {
        // Arrange
        string storageName = "storage";
        string itemTypeName = "item";
        int quantity = 10;
        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        mockStorageRepo.Setup(x =>
                x.CheckExistsAsync(It.Is<Storage>(s => s.Name == storageName && s.Category == null)))
            .ReturnsAsync(true)
            .Verifiable(Times.Once);
        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        mockItemTypeRepo.Setup(x =>
                x.CheckExistsAsync(It.Is<ItemType>(i => i.Name == itemTypeName && i.Category == null)))
            .ReturnsAsync(true)
            .Verifiable(Times.Once);
        Mock<IItemStockRepository> mockStockRepo = new Mock<IItemStockRepository>();
        mockStockRepo.Setup(x =>
                x.AddAsync(It.Is<ItemStock>(
                    s => s.Storage.Name == storageName
                         && s.ItemType.Name == itemTypeName
                         && s.Quantity == quantity)))
            .Verifiable(Times.Once);

        ItemStockService service =
            new ItemStockService(mockStockRepo.Object, mockItemTypeRepo.Object, mockStorageRepo.Object);

        // Act
        CreateItemStockResult result = await service.CreateItemStockAsync(new CreateItemStockCommand
        {
            ItemTypeName = itemTypeName,
            StorageName = storageName,
            Quantity = quantity
        });

        // Assert
        mockStorageRepo.Verify();
        mockItemTypeRepo.Verify();
        mockStockRepo.Verify();
        Assert.Equal(CreateItemStockStatus.Success, result.Status);
        Assert.NotNull(result.Id);
    }

    [Fact]
    public async Task Create_WithCategories_Success()
    {
        // Arrange
        string storageName = "storage";
        string itemTypeName = "item";
        string storageCategoryPath = "storageCategory";
        string itemCategoryPath = "itemCategory";
        int quantity = 10;
        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        mockStorageRepo.Setup(x =>
                x.CheckExistsAsync(It.Is<Storage>(s => s.Name == storageName
                                                       && s.Category != null
                                                       && s.Category.Path == storageCategoryPath)))
            .ReturnsAsync(true)
            .Verifiable(Times.Once);
        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        mockItemTypeRepo.Setup(x =>
                x.CheckExistsAsync(It.Is<ItemType>(i => i.Name == itemTypeName
                                                        && i.Category != null
                                                        && i.Category.Path == itemCategoryPath)))
            .ReturnsAsync(true)
            .Verifiable(Times.Once);
        Mock<IItemStockRepository> mockStockRepo = new Mock<IItemStockRepository>();
        mockStockRepo.Setup(x =>
                x.AddAsync(It.Is<ItemStock>(
                    s => s.Storage.Name == storageName
                         && s.ItemType.Name == itemTypeName
                         && s.Quantity == quantity)))
            .Verifiable(Times.Once);

        ItemStockService service =
            new ItemStockService(mockStockRepo.Object, mockItemTypeRepo.Object, mockStorageRepo.Object);

        // Act
        CreateItemStockResult result = await service.CreateItemStockAsync(new CreateItemStockCommand
        {
            ItemTypeName = itemTypeName,
            StorageName = storageName,
            ItemTypeCategoryPath = itemCategoryPath,
            StorageCategoryPath = storageCategoryPath,
            Quantity = quantity
        });

        // Assert
        mockStorageRepo.Verify();
        mockItemTypeRepo.Verify();
        mockStockRepo.Verify();
        Assert.Equal(CreateItemStockStatus.Success, result.Status);
    }

    [Fact]
    public async Task Create_StorageNotExists_ReturnsStorageNotFound()
    {
        // Arrange
        string storageName = "storage";
        string itemTypeName = "item";
        int quantity = 10;
        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        mockStorageRepo.Setup(x =>
                x.CheckExistsAsync(It.Is<Storage>(s => s.Name == storageName && s.Category == null)))
            .ReturnsAsync(false);
        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        mockItemTypeRepo.Setup(x =>
                x.CheckExistsAsync(It.Is<ItemType>(i => i.Name == itemTypeName && i.Category == null)))
            .ReturnsAsync(true);
        Mock<IItemStockRepository> mockStockRepo = new Mock<IItemStockRepository>();
        mockStockRepo.Setup(x =>
                x.AddAsync(It.IsAny<ItemStock>()))
            .Verifiable(Times.Never);

        ItemStockService service =
            new ItemStockService(mockStockRepo.Object, mockItemTypeRepo.Object, mockStorageRepo.Object);

        // Act
        CreateItemStockResult result = await service.CreateItemStockAsync(new CreateItemStockCommand
        {
            ItemTypeName = itemTypeName,
            StorageName = storageName,
            Quantity = quantity
        });

        // Assert
        mockStorageRepo.Verify();
        mockItemTypeRepo.Verify();
        mockStockRepo.Verify();
        Assert.Equal(CreateItemStockStatus.StorageNotFound, result.Status);
    }

    [Fact]
    public async Task Create_TypeNotExists_ReturnsTypeNotFound()
    {
        // Arrange
        string storageName = "storage";
        string itemTypeName = "item";
        int quantity = 10;
        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        mockStorageRepo.Setup(x =>
                x.CheckExistsAsync(It.Is<Storage>(s => s.Name == storageName && s.Category == null)))
            .ReturnsAsync(true);
        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        mockItemTypeRepo.Setup(x =>
                x.CheckExistsAsync(It.Is<ItemType>(i => i.Name == itemTypeName && i.Category == null)))
            .ReturnsAsync(false);
        Mock<IItemStockRepository> mockStockRepo = new Mock<IItemStockRepository>();
        mockStockRepo.Setup(x =>
                x.AddAsync(It.IsAny<ItemStock>()))
            .Verifiable(Times.Never);

        ItemStockService service =
            new ItemStockService(mockStockRepo.Object, mockItemTypeRepo.Object, mockStorageRepo.Object);

        // Act
        CreateItemStockResult result = await service.CreateItemStockAsync(new CreateItemStockCommand
        {
            ItemTypeName = itemTypeName,
            StorageName = storageName,
            Quantity = quantity
        });

        // Assert
        mockStorageRepo.Verify();
        mockItemTypeRepo.Verify();
        mockStockRepo.Verify();
        Assert.Equal(CreateItemStockStatus.ItemTypeNotFound, result.Status);
    }

    [Fact]
    public async Task ChangeQuantity_Success_ChangeUpdated()
    {
        // Arrange
        int initialQuantity = 10;
        int newQuantity = 30;
        int id = 1;
        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        Mock<IItemStockRepository> mockStockRepo = new Mock<IItemStockRepository>();
        mockStockRepo.Setup(x =>
                x.GetByIdAsync(1))
            .ReturnsAsync(new ItemStock { Id = id, Quantity = initialQuantity });
        mockStockRepo.Setup(x =>
                x.UpdateAsync(It.Is<ItemStock>(s => s.Id == id && s.Quantity == newQuantity)))
            .Verifiable(Times.Once);

        ItemStockService service =
            new ItemStockService(mockStockRepo.Object, mockItemTypeRepo.Object, mockStorageRepo.Object);

        // Act
        ChangeItemStockQuantityStatus status = await service.ChangeItemStockQuantityAsync(
            new ChangeItemStockQuantityCommand
            {
                Id = id,
                NewQuantity = newQuantity
            });

        // Assert
        mockStockRepo.Verify();
        Assert.Equal(ChangeItemStockQuantityStatus.Success, status);
    }

    [Fact]
    public async Task GetAllByType_WithoutCategory_ReturnsCorrect()
    {
        // Arrange
        string typeName = "type";
        string storage1Name = "storage1";
        string storage2Name = "storage1";
        ItemType type = new ItemType { Name = typeName };
        ItemStock stock1 = new ItemStock
        {
            Id = 1,
            ItemType = type,
            Storage = new Storage { Name = storage1Name },
            Quantity = 1
        };
        ItemStock stock2 = new ItemStock
        {
            Id = 2,
            ItemType = type,
            Storage = new Storage { Name = storage2Name },
            Quantity = 2
        };

        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        Mock<IItemStockRepository> mockStockRepo = new Mock<IItemStockRepository>();
        mockItemTypeRepo.Setup(x =>
                x.CheckExistsAsync(It.Is<ItemType>(i => i.Name == typeName && i.Category == null)))
            .ReturnsAsync(true)
            .Verifiable(Times.Once);
        mockStockRepo.Setup(x =>
                x.GetAllByTypeAsync(It.Is<ItemType>(i => i.Name == typeName && i.Category == null)))
            .Returns(new[] { stock1, stock2 }.ToAsyncEnumerable());

        ItemStockService service =
            new ItemStockService(mockStockRepo.Object, mockItemTypeRepo.Object, mockStorageRepo.Object);

        // Act
        GetAllStocksByTypeResult result =
            await service.GetAllByTypeAsync(new GetItemStocksByTypeCommand { ItemTypeName = typeName });

        // Assert
        Assert.Equal(GetAllStocksByTypeStatus.Success, result.Status);
        Assert.NotNull(result.Result);
        List<GetItemStockResult> list = result.Result.ToList();
        Assert.Equal(2, list.Count);
        Assert.True(list[0].Id == stock1.Id || list[0].Id == stock2.Id);
        Assert.True(list[1].Id == stock1.Id || list[1].Id == stock2.Id);
    }

    [Fact]
    public async Task GetAllByType_WithCategory_ReturnsCorrect()
    {
        // Arrange
        string typeName = "type";
        string typeCategoryPath = "typeCategory";
        string storage1Name = "storage1";
        string storage2Name = "storage1";
        ItemType type = new ItemType { Name = typeName, Category = new Category<ItemType> { Path = typeCategoryPath } };
        ItemStock stock1 = new ItemStock
        {
            Id = 1,
            ItemType = type,
            Storage = new Storage { Name = storage1Name },
            Quantity = 1
        };
        ItemStock stock2 = new ItemStock
        {
            Id = 2,
            ItemType = type,
            Storage = new Storage { Name = storage2Name },
            Quantity = 2
        };

        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        Mock<IItemStockRepository> mockStockRepo = new Mock<IItemStockRepository>();
        mockItemTypeRepo.Setup(x =>
                x.CheckExistsAsync(It.Is<ItemType>(
                    i => i.Name == typeName && i.Category != null && i.Category.Path == typeCategoryPath)))
            .ReturnsAsync(true)
            .Verifiable(Times.Once);
        mockStockRepo.Setup(x =>
                x.GetAllByTypeAsync(It.Is<ItemType>(
                    i => i.Name == typeName && i.Category != null && i.Category.Path == typeCategoryPath)))
            .Returns(new[] { stock1, stock2 }.ToAsyncEnumerable());

        ItemStockService service =
            new ItemStockService(mockStockRepo.Object, mockItemTypeRepo.Object, mockStorageRepo.Object);

        // Act
        GetAllStocksByTypeResult result =
            await service.GetAllByTypeAsync(new GetItemStocksByTypeCommand { ItemTypeName = typeName });

        // Assert
        Assert.Equal(GetAllStocksByTypeStatus.Success, result.Status);
        Assert.NotNull(result.Result);
        List<GetItemStockResult> list = result.Result.ToList();
        Assert.Equal(2, list.Count);
        Assert.True(list[0].Id == stock1.Id || list[0].Id == stock2.Id);
        Assert.True(list[1].Id == stock1.Id || list[1].Id == stock2.Id);
    }

    
    [Fact]
    public async Task GetAllByStorage_WithoutCategory_ReturnsCorrect()
    {
        // Arrange
        string type1Name = "type1";
        string type2Name = "type2";
        string storageName = "storage";
        Storage storage = new Storage { Name = storageName };
        ItemStock stock1 = new ItemStock
        {
            Id = 1,
            ItemType = new ItemType {Name = type1Name},
            Storage = storage,
            Quantity = 1
        };
        ItemStock stock2 = new ItemStock
        {
            Id = 2,
            ItemType = new ItemType {Name = type2Name},
            Storage = storage,
            Quantity = 2
        };

        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        Mock<IItemStockRepository> mockStockRepo = new Mock<IItemStockRepository>();
        mockStorageRepo.Setup(x =>
                x.CheckExistsAsync(It.Is<Storage>(i => i.Name == storageName && i.Category == null)))
            .ReturnsAsync(true)
            .Verifiable(Times.Once);
        mockStockRepo.Setup(x =>
                x.GetAllByStorageAsync(It.Is<Storage>(i => i.Name == storageName && i.Category == null)))
            .Returns(new[] { stock1, stock2 }.ToAsyncEnumerable());

        ItemStockService service =
            new ItemStockService(mockStockRepo.Object, mockItemTypeRepo.Object, mockStorageRepo.Object);

        // Act
        GetAllStocksByStorageResult result =
            await service.GetAllByStorageAsync(new GetItemStocksByStorageCommand { StorageName = storageName });

        // Assert
        Assert.Equal(GetAllStocksByStorageStatus.Success, result.Status);
        Assert.NotNull(result.Result);
        List<GetItemStockResult> list = result.Result.ToList();
        Assert.Equal(2, list.Count);
        Assert.True(list[0].Id == stock1.Id || list[0].Id == stock2.Id);
        Assert.True(list[1].Id == stock1.Id || list[1].Id == stock2.Id);
    }

    [Fact]
    public async Task GetAllByType_NotExists_ReturnsTypeNotFound()
    {
        // Arrange
        string typeName = "type";

        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        Mock<IItemStockRepository> mockStockRepo = new Mock<IItemStockRepository>();
        mockItemTypeRepo.Setup(x =>
                x.CheckExistsAsync(It.Is<ItemType>(i => i.Name == typeName && i.Category == null)))
            .ReturnsAsync(false)
            .Verifiable(Times.Once);
        mockStockRepo.Setup(x =>
                x.GetAllByTypeAsync(It.IsAny<ItemType>()))
            .Verifiable(Times.Never);


        ItemStockService service =
            new ItemStockService(mockStockRepo.Object, mockItemTypeRepo.Object, mockStorageRepo.Object);

        // Act
        GetAllStocksByTypeResult result =
            await service.GetAllByTypeAsync(new GetItemStocksByTypeCommand { ItemTypeName = typeName });

        // Assert
        Assert.Equal(GetAllStocksByTypeStatus.TypeNotFound, result.Status);
    }
    
    [Fact]
    public async Task GetAllByStorage_NotExists_ReturnsStorageNotFound()
    {
        // Arrange
        string storageName = "storage";

        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        Mock<IItemStockRepository> mockStockRepo = new Mock<IItemStockRepository>();
        mockStorageRepo.Setup(x =>
                x.CheckExistsAsync(It.Is<Storage>(i => i.Name == storageName && i.Category == null)))
            .ReturnsAsync(false)
            .Verifiable(Times.Once);
        mockStockRepo.Setup(x =>
                x.GetAllByStorageAsync(It.IsAny<Storage>()))
            .Verifiable(Times.Never);


        ItemStockService service =
            new ItemStockService(mockStockRepo.Object, mockItemTypeRepo.Object, mockStorageRepo.Object);

        // Act
        GetAllStocksByStorageResult result =
            await service.GetAllByStorageAsync(new GetItemStocksByStorageCommand { StorageName = storageName });

        // Assert
        Assert.Equal(GetAllStocksByStorageStatus.StorageNotFound, result.Status);
    }
}