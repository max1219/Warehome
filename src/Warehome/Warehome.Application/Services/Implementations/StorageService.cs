using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;
using Warehome.Application.Repositories;
using Warehome.Domain.Entities;

namespace Warehome.Application.Services.Implementations;

public class StorageService(
    IStorageRepository storageRepository,
    ICategoryRepository<Storage> storageCategoryRepository)
    : IStorageService
{
    private readonly IStorageRepository _storageRepository = storageRepository;
    private readonly ICategoryRepository<Storage> _storageCategoryRepository = storageCategoryRepository;

    public async Task<CreateStorageStatus> CreateStorageAsync(CreateStorageCommand command)
    {
        Category<Storage>? category = null;
        if (command.CategoryPath is not null)
        {
            category = new Category<Storage> { Path = command.CategoryPath };
            if (! await _storageCategoryRepository.CheckExistsAsync(category))
            {
                return CreateStorageStatus.CategoryNotFound;
            }
        }
        
        bool isExists = await _storageRepository.GetAsync(command.Name, category) != null;
        if (isExists)
        {
            return CreateStorageStatus.AlreadyExists;
        }

        await _storageRepository.AddAsync(new Storage { Name = command.Name, Category = category });

        return CreateStorageStatus.Success;
    }

    public async Task<DeleteStorageStatus> DeleteStorageAsync(DeleteStorageCommand command)
    {
        Category<Storage>? category = null;
        if (command.CategoryPath is not null)
        {
            category = new Category<Storage> { Path = command.CategoryPath };
            if (! await _storageCategoryRepository.CheckExistsAsync(category))
            {
                return DeleteStorageStatus.NotFound;
            }
        }
        
        Storage? storage = await _storageRepository.GetAsync(command.Name, category);
        if (storage == null)
        {
            return DeleteStorageStatus.NotFound;
        }
        await _storageRepository.DeleteAsync(storage);
        return DeleteStorageStatus.Success;
    }
}