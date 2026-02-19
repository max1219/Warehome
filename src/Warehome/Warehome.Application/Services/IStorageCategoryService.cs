using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;

namespace Warehome.Application.Services;

public interface IStorageCategoryService
{
    Task<CreateStorageCategoryStatus> CreateStorageCategoryAsync(CreateStorageCategoryDto dto);
    Task<DeleteStorageCategoryStatus> DeleteStorageCategoryAsync(DeleteStorageCategoryDto dto);
}