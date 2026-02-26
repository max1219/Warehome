using Moq;
using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;
using Warehome.Application.Repositories;
using Warehome.Application.Services.Implementations;
using Warehome.Domain.Entities;

namespace Warehome.Application.UnitTests.Services;

public class ItemTypeServiceTests
{
    [Fact]
    public async Task Create_WithoutCategory_Success()
    {
        // Arrange
        string itemTypeName = "test";
        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        mockItemTypeRepo.Setup(x =>
                x.AddAsync(It.Is<ItemType>(itemType => itemType.Name == itemTypeName
                                                     && itemType.Category == null)))
            .Verifiable(Times.Once);
        mockItemTypeRepo.Setup(x =>
                x.GetAsync(itemTypeName, null))
            .Verifiable(Times.Once);
        Mock<ICategoryRepository<ItemType>> mockCategoryRepo = new Mock<ICategoryRepository<ItemType>>();
        mockCategoryRepo.Setup(x =>
                x.CheckExistsAsync(It.IsAny<Category<ItemType>>()))
            .Verifiable(Times.Never);
        ItemTypeService service = new ItemTypeService(mockItemTypeRepo.Object, mockCategoryRepo.Object);

        // Act
        CreateItemTypeStatus status =
            await service.CreateItemTypeAsync(new CreateItemTypeCommand { Name = itemTypeName });

        // Assert
        mockItemTypeRepo.Verify();
        mockCategoryRepo.Verify();
        Assert.Equal(CreateItemTypeStatus.Success, status);
    }

    [Fact]
    public async Task Create_WithCategory_Success()
    {
        // Arrange
        string itemTypeName = "test";
        string categoryPath = "testCategory/t1/t2/t3";
        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        mockItemTypeRepo.Setup(x =>
                x.AddAsync(It.Is<ItemType>(
                    itemType => itemType.Name == itemTypeName && itemType.Category!.Path == categoryPath)))
            .Verifiable(Times.Once);
        mockItemTypeRepo.Setup(x =>
                x.GetAsync(itemTypeName, It.Is<Category<ItemType>>(category => category.Path == categoryPath)))
            .Verifiable(Times.Once);
        Mock<ICategoryRepository<ItemType>> mockCategoryRepo = new Mock<ICategoryRepository<ItemType>>();
        mockCategoryRepo.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<ItemType>>(category => category.Path == categoryPath)))
            .ReturnsAsync(true)
            .Verifiable(Times.Once);

        ItemTypeService service = new ItemTypeService(mockItemTypeRepo.Object, mockCategoryRepo.Object);

        // Act
        CreateItemTypeStatus status =
            await service.CreateItemTypeAsync(new CreateItemTypeCommand
                { Name = itemTypeName, CategoryPath = categoryPath });

        // Assert
        mockItemTypeRepo.Verify();
        mockCategoryRepo.Verify();
        Assert.Equal(CreateItemTypeStatus.Success, status);
    }

    [Fact]
    public async Task Create_AlreadyExists_ReturnsAlreadyExists()
    {
        // Arrange
        string itemTypeName = "test";
        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        mockItemTypeRepo.Setup(x =>
                x.AddAsync(It.IsAny<ItemType>()))
            .Verifiable(Times.Never);
        mockItemTypeRepo.Setup(x =>
                x.GetAsync(itemTypeName, null))
            .ReturnsAsync(new ItemType { Name = itemTypeName })
            .Verifiable(Times.Once);
        Mock<ICategoryRepository<ItemType>> mockCategoryRepo = new Mock<ICategoryRepository<ItemType>>();
        ItemTypeService service = new ItemTypeService(mockItemTypeRepo.Object, mockCategoryRepo.Object);

        // Act
        CreateItemTypeStatus status =
            await service.CreateItemTypeAsync(new CreateItemTypeCommand { Name = itemTypeName });

        // Assert
        mockItemTypeRepo.Verify();
        Assert.Equal(CreateItemTypeStatus.AlreadyExists, status);
    }

    [Fact]
    public async Task Create_CategoryNotExists_ReturnsCategoryNotFound()
    {
        // Arrange
        string itemTypeName = "test";
        string categoryPath = "testCategory/t1/t2/t3";
        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        mockItemTypeRepo.Setup(x =>
            x.AddAsync(It.IsAny<ItemType>()))
            .Verifiable(Times.Never);
        Mock<ICategoryRepository<ItemType>> mockCategoryRepo = new Mock<ICategoryRepository<ItemType>>();
        mockCategoryRepo.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<ItemType>>(category => category.Path == categoryPath)))
            .ReturnsAsync(false);

        ItemTypeService service = new ItemTypeService(mockItemTypeRepo.Object, mockCategoryRepo.Object);

        // Act
        CreateItemTypeStatus status =
            await service.CreateItemTypeAsync(new CreateItemTypeCommand { Name = itemTypeName, CategoryPath = categoryPath });

        // Assert
        Assert.Equal(CreateItemTypeStatus.CategoryNotFound, status);
    }

    [Fact]
    public async Task Delete_Success()
    {
        // Arrange
        string itemTypeName = "test";
        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        mockItemTypeRepo.Setup(x => x.DeleteAsync(It.Is<ItemType>(itemType => itemType.Name == itemTypeName)))
            .Verifiable(Times.Once);
        mockItemTypeRepo.Setup(x => x.GetAsync(It.Is<string>(path => path == itemTypeName), null))
            .ReturnsAsync(new ItemType { Name = itemTypeName })
            .Verifiable(Times.Once);
        Mock<ICategoryRepository<ItemType>> mockCategoryRepo = new Mock<ICategoryRepository<ItemType>>();

        ItemTypeService service = new ItemTypeService(mockItemTypeRepo.Object, mockCategoryRepo.Object);

        // Act
        DeleteItemTypeStatus status = await service.DeleteItemTypeAsync(new DeleteItemTypeCommand { Name = itemTypeName });

        // Assert
        mockItemTypeRepo.Verify();
        Assert.Equal(DeleteItemTypeStatus.Success, status);
    }

    [Fact]
    public async Task Delete_NotExists_ReturnsNotFound()
    {
        // Arrange
        string itemTypeName = "test";
        Mock<IItemTypeRepository> mockRepo = new Mock<IItemTypeRepository>();
        mockRepo.Setup(x => x.DeleteAsync(It.IsAny<ItemType>()))
            .Verifiable(Times.Never);
        mockRepo.Setup(x => x.GetAsync(It.Is<string>(path => path == itemTypeName), null))
            .ReturnsAsync(value: null)
            .Verifiable(Times.Once);

        Mock<ICategoryRepository<ItemType>> mockCategoryRepo = new Mock<ICategoryRepository<ItemType>>();

        ItemTypeService service = new ItemTypeService(mockRepo.Object, mockCategoryRepo.Object);

        // Act
        DeleteItemTypeStatus status = await service.DeleteItemTypeAsync(new DeleteItemTypeCommand { Name = itemTypeName });

        // Assert
        mockRepo.Verify();
        Assert.Equal(DeleteItemTypeStatus.NotFound, status);
    }
}