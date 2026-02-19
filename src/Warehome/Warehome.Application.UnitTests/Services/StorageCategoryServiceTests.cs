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
                x.CheckExistsAsync(It.IsAny<Category<Storage>>()))
            .Verifiable(Times.Never);
        mockCategoryRepository.Setup(x =>
                x.TryAddAsync(It.Is<Category<Storage>>(
                    category => category.Path == name), null))
            .ReturnsAsync(true)
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
            .ReturnsAsync(true);

        mockCategoryRepository.Setup(x =>
                x.TryAddAsync(It.Is<Category<Storage>>(category => category.Path == path)
                    , It.Is<Category<Storage>>(category => category.Path == parentPath)))
            .ReturnsAsync(true)
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
                x.TryAddAsync(It.Is<Category<Storage>>(
                    category => category.Path == name), null))
            .ReturnsAsync(false)
            .Verifiable(Times.Once);

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
            .ReturnsAsync(true)
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
}