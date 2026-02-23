using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;

namespace Warehome.Application.Services;

public interface IStorageCategoryService
{
    Task<GetStorageCategoryTreeResult> GetTreeAsync();
    Task<CreateStorageCategoryStatus> CreateStorageCategoryAsync(CreateStorageCategoryCommand command);
    Task<DeleteStorageCategoryStatus> DeleteStorageCategoryAsync(DeleteStorageCategoryCommand command);
}