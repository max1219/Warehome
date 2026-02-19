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

    public async Task<CreateStorageStatus> CreateStorageAsync(CreateStorageDto dto)
    {
        Category<Storage>? category = null;
        if (dto.CategoryPath is not null)
        {
            category = new Category<Storage> { Path = dto.CategoryPath };
            if (! await _storageCategoryRepository.CheckExistsAsync(category))
            {
                return CreateStorageStatus.CategoryNotFound;
            }
        }

        bool isSuccess = await _storageRepository.TryAddAsync(new Storage { Name = dto.Name, Category = category });

        return isSuccess ? CreateStorageStatus.Success : CreateStorageStatus.AlreadyExists;
    }

    public async Task<DeleteStorageStatus> DeleteStorageAsync(DeleteStorageDto dto)
    {
        Category<Storage>? category = null;
        if (dto.CategoryPath is not null)
        {
            category = new Category<Storage> { Path = dto.CategoryPath };
            if (! await _storageCategoryRepository.CheckExistsAsync(category))
            {
                return DeleteStorageStatus.NotFound;
            }
        }
        
        Storage? storage = await _storageRepository.GetAsync(dto.Name, category);
        if (storage == null)
        {
            return DeleteStorageStatus.NotFound;
        }
        await _storageRepository.DeleteAsync(storage);
        return DeleteStorageStatus.Success;
    }
}