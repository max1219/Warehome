using Moq;
using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;
using Warehome.Application.Repositories;
using Warehome.Application.Services.Implementations;
using Warehome.Domain.Entities;

namespace Warehome.Application.UnitTests.Services;

public class ItemTypeCategoryServiceTests
{
    [Fact]
    public async Task Create_WithoutParent_Success()
    {
        // Arrange
        string name = "test";
        Mock<ICategoryRepository<ItemType>> mockCategoryRepository = new Mock<ICategoryRepository<ItemType>>();
        mockCategoryRepository.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<ItemType>>(category => category.Path == name)))
            .ReturnsAsync(false)
            .Verifiable(Times.Once);
        mockCategoryRepository.Setup(x =>
                x.AddAsync(It.Is<Category<ItemType>>(category => category.Path == name), null))
            .Verifiable(Times.Once);

        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        ItemTypeCategoryService service =
            new ItemTypeCategoryService(mockCategoryRepository.Object, mockItemTypeRepo.Object);

        // Act
        CreateItemTypeCategoryStatus status =
            await service.CreateItemTypeCategoryAsync(new CreateItemTypeCategoryCommand { Name = name });

        // Assert
        mockCategoryRepository.Verify();
        Assert.Equal(CreateItemTypeCategoryStatus.Success, status);
    }

    [Fact]
    public async Task Create_WithParent_Success()
    {
        // Arrange
        string name = "test";
        string parentPath = "parent";
        string path = $"{parentPath}/{name}";
        Mock<ICategoryRepository<ItemType>> mockCategoryRepository = new Mock<ICategoryRepository<ItemType>>();

        mockCategoryRepository.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<ItemType>>(category => category.Path == parentPath)))
            .ReturnsAsync(true)
            .Verifiable(Times.Once);

        mockCategoryRepository.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<ItemType>>(category => category.Path == path)))
            .ReturnsAsync(false)
            .Verifiable(Times.Once);
        mockCategoryRepository.Setup(x =>
                x.AddAsync(It.Is<Category<ItemType>>(category => category.Path == path)
                    , It.Is<Category<ItemType>>(category => category.Path == parentPath)))
            .Verifiable(Times.Once);

        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        ItemTypeCategoryService service =
            new ItemTypeCategoryService(mockCategoryRepository.Object, mockItemTypeRepo.Object);

        // Act
        CreateItemTypeCategoryStatus status =
            await service.CreateItemTypeCategoryAsync(new CreateItemTypeCategoryCommand
                { Name = name, ParentPath = parentPath });

        // Assert
        mockCategoryRepository.Verify();
        Assert.Equal(CreateItemTypeCategoryStatus.Success, status);
    }

    [Fact]
    public async Task Create_AlreadyExists_ReturnsAlreadyExists()
    {
        // Arrange
        string name = "test";
        Mock<ICategoryRepository<ItemType>> mockCategoryRepository = new Mock<ICategoryRepository<ItemType>>();
        mockCategoryRepository.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<ItemType>>(category => category.Path == name)))
            .ReturnsAsync(true)
            .Verifiable(Times.Once);
        mockCategoryRepository.Setup(x =>
                x.AddAsync(It.IsAny<Category<ItemType>>(), null))
            .Verifiable(Times.Never);

        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        ItemTypeCategoryService service =
            new ItemTypeCategoryService(mockCategoryRepository.Object, mockItemTypeRepo.Object);

        // Act
        CreateItemTypeCategoryStatus status =
            await service.CreateItemTypeCategoryAsync(new CreateItemTypeCategoryCommand { Name = name });

        // Assert
        mockCategoryRepository.Verify();
        Assert.Equal(CreateItemTypeCategoryStatus.AlreadyExists, status);
    }

    [Fact]
    public async Task Create_ParentNotExists_ReturnsParentNotFound()
    {
        // Arrange
        string name = "test";
        string parentPath = "parent";
        string path = $"{parentPath}/{name}";
        Mock<ICategoryRepository<ItemType>> mockCategoryRepository = new Mock<ICategoryRepository<ItemType>>();
        mockCategoryRepository.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<ItemType>>(category => category.Path == parentPath)))
            .ReturnsAsync(false)
            .Verifiable(Times.Once);

        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();

        ItemTypeCategoryService service =
            new ItemTypeCategoryService(mockCategoryRepository.Object, mockItemTypeRepo.Object);

        // Act
        CreateItemTypeCategoryStatus status = await service.CreateItemTypeCategoryAsync(
            new CreateItemTypeCategoryCommand { Name = path, ParentPath = parentPath });

        // Assert
        mockCategoryRepository.Verify();
        Assert.Equal(CreateItemTypeCategoryStatus.ParentNotFound, status);
    }

    [Fact]
    public async Task Delete_Success()
    {
        // Arrange
        string path = "test";
        Mock<ICategoryRepository<ItemType>> mockCategoryRepository = new Mock<ICategoryRepository<ItemType>>();
        mockCategoryRepository.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<ItemType>>(category => category.Path == path)))
            .ReturnsAsync(true);
        mockCategoryRepository.Setup(x =>
                x.GetAllByParentAsync(It.Is<Category<ItemType>>(category => category.Path == path), false))
            .Returns(Array.Empty<Category<ItemType>>().ToAsyncEnumerable());
        mockCategoryRepository.Setup(x =>
                x.DeleteAsync(It.Is<Category<ItemType>>(category => category.Path == path)))
            .Verifiable(Times.Once);

        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        mockItemTypeRepo.Setup(x =>
                x.GetAllByCategoryAsync(It.Is<Category<ItemType>>(category => category.Path == path)))
            .Returns(Array.Empty<ItemType>().ToAsyncEnumerable());
        ItemTypeCategoryService service =
            new ItemTypeCategoryService(mockCategoryRepository.Object, mockItemTypeRepo.Object);

        // Act
        DeleteItemTypeCategoryStatus status = await service.DeleteItemTypeCategoryAsync(
            new DeleteItemTypeCategoryCommand { Path = path });

        // Assert
        mockCategoryRepository.Verify();
        Assert.Equal(DeleteItemTypeCategoryStatus.Success, status);
    }

    [Fact]
    public async Task Delete_HasItemTypes_ReturnsNotEmpty()
    {
        // Arrange
        string path = "test";
        string itemTypeName = "itemType";
        Mock<ICategoryRepository<ItemType>> mockCategoryRepository = new Mock<ICategoryRepository<ItemType>>();
        mockCategoryRepository.Setup(x =>
                x.GetAllByParentAsync(It.Is<Category<ItemType>>(category => category.Path == path), false))
            .Returns(Array.Empty<Category<ItemType>>().ToAsyncEnumerable());

        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        mockItemTypeRepo.Setup(x =>
                x.GetAllByCategoryAsync(It.Is<Category<ItemType>>(category => category.Path == path)))
            .Returns(new[] { new ItemType { Name = itemTypeName } }.ToAsyncEnumerable());

        mockCategoryRepository.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<ItemType>>(category => category.Path == path)))
            .ReturnsAsync(true);
        mockCategoryRepository.Setup(x =>
                x.DeleteAsync(It.IsAny<Category<ItemType>>()))
            .Verifiable(Times.Never);

        ItemTypeCategoryService service =
            new ItemTypeCategoryService(mockCategoryRepository.Object, mockItemTypeRepo.Object);

        // Act
        DeleteItemTypeCategoryStatus status = await service.DeleteItemTypeCategoryAsync(
            new DeleteItemTypeCategoryCommand { Path = path });

        // Assert
        mockCategoryRepository.Verify();
        Assert.Equal(DeleteItemTypeCategoryStatus.NotEmpty, status);
    }

    [Fact]
    public async Task Delete_HasChild_ReturnsNotEmpty()
    {
        // Arrange
        string path = "test";
        string childPath = "child";
        Mock<ICategoryRepository<ItemType>> mockCategoryRepository = new Mock<ICategoryRepository<ItemType>>();
        mockCategoryRepository.Setup(x =>
                x.GetAllByParentAsync(
                    It.Is<Category<ItemType>>(category => category.Path == path), false))
            .Returns(new[] { new Category<ItemType> { Path = childPath } }.ToAsyncEnumerable())
            .Verifiable(Times.Once);
        mockCategoryRepository.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<ItemType>>(category => category.Path == path)))
            .ReturnsAsync(true);
        mockCategoryRepository.Setup(x =>
                x.DeleteAsync(It.IsAny<Category<ItemType>>()))
            .Verifiable(Times.Never);

        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        mockItemTypeRepo.Setup(x =>
                x.GetAllByCategoryAsync(It.Is<Category<ItemType>>(category => category.Path == path)))
            .Returns(Array.Empty<ItemType>().ToAsyncEnumerable());

        ItemTypeCategoryService service =
            new ItemTypeCategoryService(mockCategoryRepository.Object, mockItemTypeRepo.Object);

        // Act
        DeleteItemTypeCategoryStatus status = await service.DeleteItemTypeCategoryAsync(
            new DeleteItemTypeCategoryCommand { Path = path });

        // Assert
        mockCategoryRepository.Verify();
        Assert.Equal(DeleteItemTypeCategoryStatus.NotEmpty, status);
    }

    [Fact]
    public async Task Delete_NotExists_ReturnsNotFound()
    {
        // Arrange
        string path = "test";
        Mock<ICategoryRepository<ItemType>> mockCategoryRepository = new Mock<ICategoryRepository<ItemType>>();
        mockCategoryRepository.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<ItemType>>(category => category.Path == path)))
            .ReturnsAsync(false);
        mockCategoryRepository.Setup(x =>
                x.GetAllByParentAsync(It.IsAny<Category<ItemType>>(), false))
            .Verifiable(Times.Never);
        mockCategoryRepository.Setup(x =>
                x.DeleteAsync(It.IsAny<Category<ItemType>>()))
            .Verifiable(Times.Never);

        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        mockItemTypeRepo.Setup(x =>
                x.GetAllByCategoryAsync(It.IsAny<Category<ItemType>>()))
            .Verifiable(Times.Never);
        ItemTypeCategoryService service =
            new ItemTypeCategoryService(mockCategoryRepository.Object, mockItemTypeRepo.Object);

        // Act
        DeleteItemTypeCategoryStatus status = await service.DeleteItemTypeCategoryAsync(
            new DeleteItemTypeCategoryCommand { Path = path });

        // Assert
        mockCategoryRepository.Verify();
        mockItemTypeRepo.Verify();
        Assert.Equal(DeleteItemTypeCategoryStatus.NotFound, status);
    }

    [Fact]
    public async Task GetTree_ReturnsCorrectTree()
    {
        // Arrange

        /*
        c1 (категория)
        ├── c1c1 (пустая категория)
        ├── c1c2 (категория)
        │   └── [itemType] c1c2ItemType1
        │   └── [itemType] c1c2ItemType2
        │   └── [itemType] c1c2ItemType3
        └── [itemType] r1ItemType1

        c2 (категория)
        ├── c2c1 (пустая категория)

        [itemType] rootItemType1
        [itemType] rootItemType2
        */

        string[] rootCategoryPaths = ["c1", "c2"];
        string[] rootItemTypeNames = ["rootItemType1", "rootItemType2"];
        string[] cat1ChildNames = ["c1c1", "c1c2"];
        string[] cat1ItemTypeNames = ["r1ItemType1"];
        string[] cat2ChildNames = ["c2c1"];
        string[] cat1Cat2ItemTypeNames = ["c1c2ItemType1", "c1c2ItemType2", "c1c2ItemType3"];

        Mock<ICategoryRepository<ItemType>> mockCategoryRepository = new Mock<ICategoryRepository<ItemType>>();
        mockCategoryRepository.Setup(x =>
                x.GetAllByParentAsync(It.IsAny<Category<ItemType>?>(), false))
            .Returns(Array.Empty<Category<ItemType>>().ToAsyncEnumerable());
        mockCategoryRepository.Setup(x =>
                x.GetAllByParentAsync(null, false))
            .Returns(rootCategoryPaths.Select(path => new Category<ItemType> { Path = path }).ToAsyncEnumerable());
        mockCategoryRepository.Setup(x =>
                x.GetAllByParentAsync(It.Is<Category<ItemType>?>(c => c != null && c.Path == rootCategoryPaths[0]),
                    false))
            .Returns(cat1ChildNames.Select(path => new Category<ItemType> { Path = path }).ToAsyncEnumerable());
        mockCategoryRepository.Setup(x =>
                x.GetAllByParentAsync(It.Is<Category<ItemType>?>(c => c != null && c.Path == rootCategoryPaths[1]),
                    false))
            .Returns(cat2ChildNames.Select(path => new Category<ItemType> { Path = path }).ToAsyncEnumerable());


        Mock<IItemTypeRepository> mockItemTypeRepo = new Mock<IItemTypeRepository>();
        mockItemTypeRepo.Setup(x =>
            x.GetAllByCategoryAsync(It.IsAny<Category<ItemType>?>()))
            .Returns(Array.Empty<ItemType>().ToAsyncEnumerable());
        mockItemTypeRepo.Setup(x =>
                x.GetAllByCategoryAsync(null))
            .Returns(rootItemTypeNames.Select(name => new ItemType { Name = name }).ToAsyncEnumerable());
        mockItemTypeRepo.Setup(x =>
                x.GetAllByCategoryAsync(It.Is<Category<ItemType>?>(c => c != null && c.Path == rootCategoryPaths[0])))
            .Returns(cat1ItemTypeNames.Select(name => new ItemType { Name = name }).ToAsyncEnumerable());
        mockItemTypeRepo.Setup(x =>
                x.GetAllByCategoryAsync(It.Is<Category<ItemType>?>(
                    c => c != null 
                         && c.Path == $"{rootCategoryPaths[0]}/{cat1ChildNames[1]}")))
            .Returns(cat1Cat2ItemTypeNames.Select(name => new ItemType { Name = name }).ToAsyncEnumerable());

        ItemTypeCategoryService service =
            new ItemTypeCategoryService(mockCategoryRepository.Object, mockItemTypeRepo.Object);

        // Act
        GetItemTypeCategoryTreeResult tree = await service.GetTreeAsync();

        // Assert
        Assert.Equal(rootCategoryPaths.Length, tree.Children.Count);
        GetItemTypeCategoryTreeResult c1 = tree.Children.Single(c => c.Name == rootCategoryPaths[0]);
        Assert.Equal(cat1ItemTypeNames, c1.ItemNames);
        Assert.Equal(cat1ChildNames, c1.Children.Select(c => c.Name));
        GetItemTypeCategoryTreeResult c1C2 = c1.Children.Single(c => c.Name == cat1ChildNames[1]);
        Assert.Equal(cat1Cat2ItemTypeNames, c1C2.ItemNames);
        Assert.Empty(c1C2.Children);
        GetItemTypeCategoryTreeResult c1C1 = c1.Children.Single(c => c.Name == cat1ChildNames[0]);
        Assert.Empty(c1C1.ItemNames);
        Assert.Empty(c1C1.Children);
        GetItemTypeCategoryTreeResult c2 = tree.Children.Single(c => c.Name == rootCategoryPaths[1]);
        Assert.Empty(c2.ItemNames);
        Assert.Equal(cat2ChildNames, c2.Children.Select(c => c.Name));
        GetItemTypeCategoryTreeResult c2C1 = c2.Children.Single(c => c.Name == cat2ChildNames[0]);
        Assert.Empty(c2C1.ItemNames);
        Assert.Empty(c2C1.Children);
    }
}
