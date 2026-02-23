using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;

namespace Warehome.Application.Services;

public interface IStorageCategoryService
{
    Task<StorageCategoryTreeResponse> GetTreeAsync();
    Task<CreateStorageCategoryStatus> CreateStorageCategoryAsync(CreateStorageCategoryDto dto);
    Task<DeleteStorageCategoryStatus> DeleteStorageCategoryAsync(DeleteStorageCategoryDto dto);
}