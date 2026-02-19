using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;
using Warehome.Application.Repositories;
using Warehome.Domain.Entities;

namespace Warehome.Application.Services.Implementations;

public class StorageCategoryService(
    ICategoryRepository<Storage> storageCategoryRepository,
    IStorageRepository storageRepository)
    : IStorageCategoryService
{
    private readonly IStorageRepository _storageRepository = storageRepository;
    private readonly ICategoryRepository<Storage> _storageCategoryRepository = storageCategoryRepository;

    public async Task<CreateStorageCategoryStatus> CreateStorageCategoryAsync(CreateStorageCategoryDto dto)
    {
        Category<Storage> category;
        Category<Storage>? parentCategory = null;

        if (dto.ParentPath is not null)
        {
            parentCategory = new Category<Storage> {Path = dto.ParentPath};
            if (! await _storageCategoryRepository.CheckExistsAsync(parentCategory))
            {
                return CreateStorageCategoryStatus.ParentNotFound;
            }

            category = Category<Storage>.FromNameAndParent(dto.Name, dto.ParentPath);
        }
        else
        {
            category = new Category<Storage> { Path = dto.Name };
        }

        bool isSuccess = await _storageCategoryRepository.TryAddAsync(category, parentCategory);

        return isSuccess ? CreateStorageCategoryStatus.Success : CreateStorageCategoryStatus.AlreadyExists;
    }

    public async Task<DeleteStorageCategoryStatus> DeleteStorageCategoryAsync(DeleteStorageCategoryDto dto)
    {
        Category<Storage> category = new Category<Storage> { Path = dto.Path };

        IAsyncEnumerable<Category<Storage>> childCategories =
            _storageCategoryRepository.GetAllByParentAsync(category, false);
        if (await childCategories.AnyAsync())
        {
            return DeleteStorageCategoryStatus.NotEmpty;
        }

        IAsyncEnumerable<Storage> storages = _storageRepository.GetAllByCategoryAsync(category);
        if (await storages.AnyAsync())
        {
            return DeleteStorageCategoryStatus.NotEmpty;
        }

        await _storageCategoryRepository.DeleteAsync(category);
        return DeleteStorageCategoryStatus.Success;
    }
}