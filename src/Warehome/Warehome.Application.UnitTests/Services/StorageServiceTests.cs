using Moq;
using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;
using Warehome.Application.Repositories;
using Warehome.Application.Services.Implementations;
using Warehome.Domain.Entities;

namespace Warehome.Application.UnitTests.Services;

public class StorageServiceTests
{
    [Fact]
    public async Task Create_WithoutCategory_Success()
    {
        // Arrange
        string storageName = "test";
        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        mockStorageRepo.Setup(x =>
                x.AddAsync(It.Is<Storage>(storage => storage.Name == storageName
                                                     && storage.Category == null)))
            .Verifiable(Times.Once);
        mockStorageRepo.Setup(x =>
                x.GetAsync(storageName, null))
            .Verifiable(Times.Once);
        Mock<ICategoryRepository<Storage>> mockCategoryRepo = new Mock<ICategoryRepository<Storage>>();
        mockCategoryRepo.Setup(x =>
                x.CheckExistsAsync(It.IsAny<Category<Storage>>()))
            .Verifiable(Times.Never);
        StorageService service = new StorageService(mockStorageRepo.Object, mockCategoryRepo.Object);

        // Act
        CreateStorageStatus status =
            await service.CreateStorageAsync(new CreateStorageDto { Name = storageName });

        // Assert
        mockStorageRepo.Verify();
        mockCategoryRepo.Verify();
        Assert.Equal(CreateStorageStatus.Success, status);
    }

    [Fact]
    public async Task Create_WithCategory_Success()
    {
        // Arrange
        string storageName = "test";
        string categoryPath = "testCategory/t1/t2/t3";
        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        mockStorageRepo.Setup(x =>
                x.AddAsync(It.Is<Storage>(
                    storage => storage.Name == storageName && storage.Category!.Path == categoryPath)))
            .Verifiable(Times.Once);
        mockStorageRepo.Setup(x =>
                x.GetAsync(storageName, It.Is<Category<Storage>>(category => category.Path == categoryPath)))
            .Verifiable(Times.Once);
        Mock<ICategoryRepository<Storage>> mockCategoryRepo = new Mock<ICategoryRepository<Storage>>();
        mockCategoryRepo.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<Storage>>(category => category.Path == categoryPath)))
            .ReturnsAsync(true)
            .Verifiable(Times.Once);

        StorageService service = new StorageService(mockStorageRepo.Object, mockCategoryRepo.Object);

        // Act
        CreateStorageStatus status =
            await service.CreateStorageAsync(new CreateStorageDto
                { Name = storageName, CategoryPath = categoryPath });

        // Assert
        mockStorageRepo.Verify();
        mockCategoryRepo.Verify();
        Assert.Equal(CreateStorageStatus.Success, status);
    }

    [Fact]
    public async Task Create_AlreadyExists_ReturnsAlreadyExists()
    {
        // Arrange
        string storageName = "test";
        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        mockStorageRepo.Setup(x =>
                x.AddAsync(It.IsAny<Storage>()))
            .Verifiable(Times.Never);
        mockStorageRepo.Setup(x =>
                x.GetAsync(storageName, null))
            .ReturnsAsync(new Storage { Name = storageName })
            .Verifiable(Times.Once);
        Mock<ICategoryRepository<Storage>> mockCategoryRepo = new Mock<ICategoryRepository<Storage>>();
        StorageService service = new StorageService(mockStorageRepo.Object, mockCategoryRepo.Object);

        // Act
        CreateStorageStatus status =
            await service.CreateStorageAsync(new CreateStorageDto { Name = storageName });

        // Assert
        mockStorageRepo.Verify();
        Assert.Equal(CreateStorageStatus.AlreadyExists, status);
    }

    [Fact]
    public async Task Create_CategoryNotExists_ReturnsCategoryNotFound()
    {
        // Arrange
        string storageName = "test";
        string categoryPath = "testCategory/t1/t2/t3";
        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        mockStorageRepo.Setup(x =>
            x.AddAsync(It.IsAny<Storage>()))
            .Verifiable(Times.Never);
        Mock<ICategoryRepository<Storage>> mockCategoryRepo = new Mock<ICategoryRepository<Storage>>();
        mockCategoryRepo.Setup(x =>
                x.CheckExistsAsync(It.Is<Category<Storage>>(category => category.Path == categoryPath)))
            .ReturnsAsync(false);

        StorageService service = new StorageService(mockStorageRepo.Object, mockCategoryRepo.Object);

        // Act
        CreateStorageStatus status =
            await service.CreateStorageAsync(new CreateStorageDto { Name = storageName, CategoryPath = categoryPath });

        // Assert
        Assert.Equal(CreateStorageStatus.CategoryNotFound, status);
    }

    [Fact]
    public async Task Delete_Success()
    {
        // Arrange
        string storageName = "test";
        Mock<IStorageRepository> mockStorageRepo = new Mock<IStorageRepository>();
        mockStorageRepo.Setup(x => x.DeleteAsync(It.Is<Storage>(storage => storage.Name == storageName)))
            .Verifiable(Times.Once);
        mockStorageRepo.Setup(x => x.GetAsync(It.Is<string>(path => path == storageName), null))
            .ReturnsAsync(new Storage { Name = storageName })
            .Verifiable(Times.Once);
        Mock<ICategoryRepository<Storage>> mockCategoryRepo = new Mock<ICategoryRepository<Storage>>();

        StorageService service = new StorageService(mockStorageRepo.Object, mockCategoryRepo.Object);

        // Act
        DeleteStorageStatus status = await service.DeleteStorageAsync(new DeleteStorageDto { Name = storageName });

        // Assert
        mockStorageRepo.Verify();
        Assert.Equal(DeleteStorageStatus.Success, status);
    }

    [Fact]
    public async Task Delete_NotExists_ReturnsNotFound()
    {
        // Arrange
        string storageName = "test";
        Mock<IStorageRepository> mockRepo = new Mock<IStorageRepository>();
        mockRepo.Setup(x => x.DeleteAsync(It.IsAny<Storage>()))
            .Verifiable(Times.Never);
        mockRepo.Setup(x => x.GetAsync(It.Is<string>(path => path == storageName), null))
            .ReturnsAsync(value: null)
            .Verifiable(Times.Once);

        Mock<ICategoryRepository<Storage>> mockCategoryRepo = new Mock<ICategoryRepository<Storage>>();

        StorageService service = new StorageService(mockRepo.Object, mockCategoryRepo.Object);

        // Act
        DeleteStorageStatus status = await service.DeleteStorageAsync(new DeleteStorageDto { Name = storageName });

        // Assert
        mockRepo.Verify();
        Assert.Equal(DeleteStorageStatus.NotFound, status);
    }
}