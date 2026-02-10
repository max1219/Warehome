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
    public async Task Create_Success()
    {
        // Arrange
        string storageName = "test";
        Mock<IStorageRepository> mockRepo = new Mock<IStorageRepository>();
        mockRepo.Setup(x => x.TryAddAsync(It.Is<Storage>(storage => storage.Name == storageName)))
            .ReturnsAsync(true)
            .Verifiable(Times.Once);
        StorageService service = new StorageService(mockRepo.Object);

        // Act
        CreateStorageStatus status =
            await service.CreateStorageAsync(new CreateStorageDto { Name = storageName });

        // Assert
        mockRepo.Verify();
        Assert.Equal(CreateStorageStatus.Success, status);
    }   
    
    [Fact]
    public async Task Create_AlreadyExists_ReturnsAlreadyExists()
    {
        // Arrange
        string storageName = "test";
        Mock<IStorageRepository> mockRepo = new Mock<IStorageRepository>();
        mockRepo.Setup(x => x.TryAddAsync(It.Is<Storage>(storage => storage.Name == storageName)))
            .ReturnsAsync(false)
            .Verifiable(Times.Once);
        StorageService service = new StorageService(mockRepo.Object);

        // Act
        CreateStorageStatus status =
            await service.CreateStorageAsync(new CreateStorageDto { Name = storageName });

        // Assert
        mockRepo.Verify();
        Assert.Equal(CreateStorageStatus.AlreadyExists, status);
    }

    [Fact]
    public async Task Delete_Success()
    {
        // Arrange
        string storageName = "test";
        Mock<IStorageRepository> mockRepo = new Mock<IStorageRepository>();
        mockRepo.Setup(x => x.DeleteAsync(It.Is<Storage>(storage => storage.Name == storageName)))
            .ReturnsAsync(true)
            .Verifiable(Times.Once);
        mockRepo.Setup(x => x.GetByPathAsync(It.Is<string>(path => path == storageName)))
            .ReturnsAsync(new Storage { Name = storageName })
            .Verifiable(Times.Once);
        
        StorageService service = new StorageService(mockRepo.Object);
        
        // Act
        DeleteStorageStatus status = await service.DeleteStorageAsync(storageName);
        
        // Assert
        mockRepo.Verify();
        Assert.Equal(DeleteStorageStatus.Success, status);
    }
    
    
}