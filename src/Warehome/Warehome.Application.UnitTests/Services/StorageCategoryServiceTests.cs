using Moq;
using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;
using Warehome.Application.Repositories;
using Warehome.Application.Services.Implementations;
using Warehome.Domain.Entities;

namespace Warehome.Application.UnitTests.Services;

public class StorageCategoryServiceTests
{
    [Fact]
    public async Task Create_WithoutParent_Success()
    {
        // Arrange
        string name = "test";
        Mock<ICategoryRepository<Storage>> mockCategoryRepository = new Mock<ICategoryRepository<Storage>>();
        mockCategoryRepository.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<Storage>>(category => category.Path == name)))
            .ReturnsAsync(false)
            .Verifiable(Times.Once);
        mockCategoryRepository.Setup(x =>
                x.AddAsync(It.Is<Category<Storage>>(category => category.Path == name), null))
            .Verifiable(Times.Once);

        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        StorageCategoryService service =
            new StorageCategoryService(mockCategoryRepository.Object, mockStorageRepo.Object);

        // Act
        CreateStorageCategoryStatus status =
            await service.CreateStorageCategoryAsync(new CreateStorageCategoryDto { Name = name });

        // Assert
        mockCategoryRepository.Verify();
        Assert.Equal(CreateStorageCategoryStatus.Success, status);
    }

    [Fact]
    public async Task Create_WithParent_Success()
    {
        // Arrange
        string name = "test";
        string parentPath = "parent";
        string path = $"{parentPath}/{name}";
        Mock<ICategoryRepository<Storage>> mockCategoryRepository = new Mock<ICategoryRepository<Storage>>();

        mockCategoryRepository.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<Storage>>(category => category.Path == parentPath)))
            .ReturnsAsync(true)
            .Verifiable(Times.Once);

        mockCategoryRepository.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<Storage>>(category => category.Path == path)))
            .ReturnsAsync(false)
            .Verifiable(Times.Once);
        mockCategoryRepository.Setup(x =>
                x.AddAsync(It.Is<Category<Storage>>(category => category.Path == path)
                    , It.Is<Category<Storage>>(category => category.Path == parentPath)))
            .Verifiable(Times.Once);

        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        StorageCategoryService service =
            new StorageCategoryService(mockCategoryRepository.Object, mockStorageRepo.Object);

        // Act
        CreateStorageCategoryStatus status =
            await service.CreateStorageCategoryAsync(new CreateStorageCategoryDto
                { Name = name, ParentPath = parentPath });

        // Assert
        mockCategoryRepository.Verify();
        Assert.Equal(CreateStorageCategoryStatus.Success, status);
    }

    [Fact]
    public async Task Create_AlreadyExists_ReturnsAlreadyExists()
    {
        // Arrange
        string name = "test";
        Mock<ICategoryRepository<Storage>> mockCategoryRepository = new Mock<ICategoryRepository<Storage>>();
        mockCategoryRepository.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<Storage>>(category => category.Path == name)))
            .ReturnsAsync(true)
            .Verifiable(Times.Once);
        mockCategoryRepository.Setup(x =>
                x.AddAsync(It.IsAny<Category<Storage>>(), null))
            .Verifiable(Times.Never);

        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        StorageCategoryService service =
            new StorageCategoryService(mockCategoryRepository.Object, mockStorageRepo.Object);

        // Act
        CreateStorageCategoryStatus status =
            await service.CreateStorageCategoryAsync(new CreateStorageCategoryDto { Name = name });

        // Assert
        mockCategoryRepository.Verify();
        Assert.Equal(CreateStorageCategoryStatus.AlreadyExists, status);
    }

    [Fact]
    public async Task Create_ParentNotExists_ReturnsParentNotFound()
    {
        // Arrange
        string name = "test";
        string parentPath = "parent";
        string path = $"{parentPath}/{name}";
        Mock<ICategoryRepository<Storage>> mockCategoryRepository = new Mock<ICategoryRepository<Storage>>();
        mockCategoryRepository.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<Storage>>(category => category.Path == parentPath)))
            .ReturnsAsync(false)
            .Verifiable(Times.Once);

        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();

        StorageCategoryService service =
            new StorageCategoryService(mockCategoryRepository.Object, mockStorageRepo.Object);

        // Act
        CreateStorageCategoryStatus status = await service.CreateStorageCategoryAsync(
            new CreateStorageCategoryDto { Name = path, ParentPath = parentPath });

        // Assert
        mockCategoryRepository.Verify();
        Assert.Equal(CreateStorageCategoryStatus.ParentNotFound, status);
    }

    [Fact]
    public async Task Delete_Success()
    {
        // Arrange
        string path = "test";
        Mock<ICategoryRepository<Storage>> mockCategoryRepository = new Mock<ICategoryRepository<Storage>>();
        mockCategoryRepository.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<Storage>>(category => category.Path == path)))
            .ReturnsAsync(true);
        mockCategoryRepository.Setup(x =>
                x.GetAllByParentAsync(It.Is<Category<Storage>>(category => category.Path == path), false))
            .Returns(Array.Empty<Category<Storage>>().ToAsyncEnumerable());
        mockCategoryRepository.Setup(x =>
                x.DeleteAsync(It.Is<Category<Storage>>(category => category.Path == path)))
            .Verifiable(Times.Once);

        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        mockStorageRepo.Setup(x =>
                x.GetAllByCategoryAsync(It.Is<Category<Storage>>(category => category.Path == path)))
            .Returns(Array.Empty<Storage>().ToAsyncEnumerable());
        StorageCategoryService service =
            new StorageCategoryService(mockCategoryRepository.Object, mockStorageRepo.Object);

        // Act
        DeleteStorageCategoryStatus status = await service.DeleteStorageCategoryAsync(
            new DeleteStorageCategoryDto { Path = path });

        // Assert
        mockCategoryRepository.Verify();
        Assert.Equal(DeleteStorageCategoryStatus.Success, status);
    }

    [Fact]
    public async Task Delete_HasStorages_ReturnsNotEmpty()
    {
        // Arrange
        string path = "test";
        string storageName = "storage";
        Mock<ICategoryRepository<Storage>> mockCategoryRepository = new Mock<ICategoryRepository<Storage>>();
        mockCategoryRepository.Setup(x =>
                x.GetAllByParentAsync(It.Is<Category<Storage>>(category => category.Path == path), false))
            .Returns(Array.Empty<Category<Storage>>().ToAsyncEnumerable());

        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        mockStorageRepo.Setup(x =>
                x.GetAllByCategoryAsync(It.Is<Category<Storage>>(category => category.Path == path)))
            .Returns(new[] { new Storage { Name = storageName } }.ToAsyncEnumerable());

        mockCategoryRepository.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<Storage>>(category => category.Path == path)))
            .ReturnsAsync(true);
        mockCategoryRepository.Setup(x =>
                x.DeleteAsync(It.IsAny<Category<Storage>>()))
            .Verifiable(Times.Never);

        StorageCategoryService service =
            new StorageCategoryService(mockCategoryRepository.Object, mockStorageRepo.Object);

        // Act
        DeleteStorageCategoryStatus status = await service.DeleteStorageCategoryAsync(
            new DeleteStorageCategoryDto { Path = path });

        // Assert
        mockCategoryRepository.Verify();
        Assert.Equal(DeleteStorageCategoryStatus.NotEmpty, status);
    }

    [Fact]
    public async Task Delete_HasChild_ReturnsNotEmpty()
    {
        // Arrange
        string path = "test";
        string childPath = "child";
        Mock<ICategoryRepository<Storage>> mockCategoryRepository = new Mock<ICategoryRepository<Storage>>();
        mockCategoryRepository.Setup(x =>
                x.GetAllByParentAsync(
                    It.Is<Category<Storage>>(category => category.Path == path), false))
            .Returns(new[] { new Category<Storage> { Path = childPath } }.ToAsyncEnumerable())
            .Verifiable(Times.Once);
        mockCategoryRepository.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<Storage>>(category => category.Path == path)))
            .ReturnsAsync(true);
        mockCategoryRepository.Setup(x =>
                x.DeleteAsync(It.IsAny<Category<Storage>>()))
            .Verifiable(Times.Never);

        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        mockStorageRepo.Setup(x =>
                x.GetAllByCategoryAsync(It.Is<Category<Storage>>(category => category.Path == path)))
            .Returns(Array.Empty<Storage>().ToAsyncEnumerable());

        StorageCategoryService service =
            new StorageCategoryService(mockCategoryRepository.Object, mockStorageRepo.Object);

        // Act
        DeleteStorageCategoryStatus status = await service.DeleteStorageCategoryAsync(
            new DeleteStorageCategoryDto { Path = path });

        // Assert
        mockCategoryRepository.Verify();
        Assert.Equal(DeleteStorageCategoryStatus.NotEmpty, status);
    }

    [Fact]
    public async Task Delete_NotExists_ReturnsNotFound()
    {
        // Arrange
        string path = "test";
        Mock<ICategoryRepository<Storage>> mockCategoryRepository = new Mock<ICategoryRepository<Storage>>();
        mockCategoryRepository.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<Storage>>(category => category.Path == path)))
            .ReturnsAsync(false);
        mockCategoryRepository.Setup(x =>
                x.GetAllByParentAsync(It.IsAny<Category<Storage>>(), false))
            .Verifiable(Times.Never);
        mockCategoryRepository.Setup(x =>
                x.DeleteAsync(It.IsAny<Category<Storage>>()))
            .Verifiable(Times.Never);

        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        mockStorageRepo.Setup(x =>
                x.GetAllByCategoryAsync(It.IsAny<Category<Storage>>()))
            .Verifiable(Times.Never);
        StorageCategoryService service =
            new StorageCategoryService(mockCategoryRepository.Object, mockStorageRepo.Object);

        // Act
        DeleteStorageCategoryStatus status = await service.DeleteStorageCategoryAsync(
            new DeleteStorageCategoryDto { Path = path });

        // Assert
        mockCategoryRepository.Verify();
        mockStorageRepo.Verify();
        Assert.Equal(DeleteStorageCategoryStatus.NotFound, status);
    }

    [Fact]
    public async Task GetTree_ReturnsCorrectTree()
    {
        // Arrange

        /*
        c1 (категория)
        ├── c1c1 (пустая категория)
        ├── c1c2 (категория)
        │   └── [storage] c1c2Storage1
        │   └── [storage] c1c2Storage2
        │   └── [storage] c1c2Storage3
        └── [storage] r1Storage1

        c2 (категория)
        ├── c2c1 (пустая категория)

        [storage] rootStorage1
        [storage] rootStorage2
        */

        string[] rootCategoryPaths = ["c1", "c2"];
        string[] rootStorageNames = ["rootStorage1", "rootStorage2"];
        string[] cat1ChildNames = ["c1c1", "c1c2"];
        string[] cat1StorageNames = ["r1Storage1"];
        string[] cat2ChildNames = ["c2c1"];
        string[] cat1Cat2StorageNames = ["c1c2Storage1", "c1c2Storage2", "c1c2Storage3"];

        Mock<ICategoryRepository<Storage>> mockCategoryRepository = new Mock<ICategoryRepository<Storage>>();
        mockCategoryRepository.Setup(x =>
                x.GetAllByParentAsync(It.IsAny<Category<Storage>?>(), false))
            .Returns(Array.Empty<Category<Storage>>().ToAsyncEnumerable());
        mockCategoryRepository.Setup(x =>
                x.GetAllByParentAsync(null, false))
            .Returns(rootCategoryPaths.Select(path => new Category<Storage> { Path = path }).ToAsyncEnumerable());
        mockCategoryRepository.Setup(x =>
                x.GetAllByParentAsync(It.Is<Category<Storage>?>(c => c != null && c.Path == rootCategoryPaths[0]),
                    false))
            .Returns(cat1ChildNames.Select(path => new Category<Storage> { Path = path }).ToAsyncEnumerable());
        mockCategoryRepository.Setup(x =>
                x.GetAllByParentAsync(It.Is<Category<Storage>?>(c => c != null && c.Path == rootCategoryPaths[1]),
                    false))
            .Returns(cat2ChildNames.Select(path => new Category<Storage> { Path = path }).ToAsyncEnumerable());


        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        mockStorageRepo.Setup(x =>
            x.GetAllByCategoryAsync(It.IsAny<Category<Storage>?>()))
            .Returns(Array.Empty<Storage>().ToAsyncEnumerable());
        mockStorageRepo.Setup(x =>
                x.GetAllByCategoryAsync(null))
            .Returns(rootStorageNames.Select(name => new Storage { Name = name }).ToAsyncEnumerable());
        mockStorageRepo.Setup(x =>
                x.GetAllByCategoryAsync(It.Is<Category<Storage>?>(c => c != null && c.Path == rootCategoryPaths[0])))
            .Returns(cat1StorageNames.Select(name => new Storage { Name = name }).ToAsyncEnumerable());
        mockStorageRepo.Setup(x =>
                x.GetAllByCategoryAsync(It.Is<Category<Storage>?>(
                    c => c != null 
                         && c.Path == $"{rootCategoryPaths[0]}/{cat1ChildNames[1]}")))
            .Returns(cat1Cat2StorageNames.Select(name => new Storage { Name = name }).ToAsyncEnumerable());

        StorageCategoryService service =
            new StorageCategoryService(mockCategoryRepository.Object, mockStorageRepo.Object);

        // Act
        StorageCategoryTreeResponse tree = await service.GetTreeAsync();

        // Assert
        Assert.Equal(rootCategoryPaths.Length, tree.ChildrenCount);
        StorageCategoryTreeResponse c1 = tree.Children.Single(c => c.Name == rootCategoryPaths[0]);
        Assert.Equal(cat1StorageNames, c1.StorageNames);
        Assert.Equal(cat1ChildNames, c1.Children.Select(c => c.Name));
        StorageCategoryTreeResponse c1C2 = c1.Children.Single(c => c.Name == cat1ChildNames[1]);
        Assert.Equal(cat1Cat2StorageNames, c1C2.StorageNames);
        Assert.Empty(c1C2.Children);
        StorageCategoryTreeResponse c1C1 = c1.Children.Single(c => c.Name == cat1ChildNames[0]);
        Assert.Empty(c1C1.StorageNames);
        Assert.Empty(c1C1.Children);
        StorageCategoryTreeResponse c2 = tree.Children.Single(c => c.Name == rootCategoryPaths[1]);
        Assert.Empty(c2.StorageNames);
        Assert.Equal(cat2ChildNames, c2.Children.Select(c => c.Name));
        StorageCategoryTreeResponse c2C1 = c2.Children.Single(c => c.Name == cat2ChildNames[0]);
        Assert.Empty(c2C1.StorageNames);
        Assert.Empty(c2C1.Children);
    }
}