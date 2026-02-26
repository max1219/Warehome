using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;

namespace Warehome.Application.Services;

public interface IItemTypeCategoryService
{
    Task<GetItemTypeCategoryTreeResult> GetTreeAsync();
    Task<CreateItemTypeCategoryStatus> CreateItemTypeCategoryAsync(CreateItemTypeCategoryCommand command);
    Task<DeleteItemTypeCategoryStatus> DeleteItemTypeCategoryAsync(DeleteItemTypeCategoryCommand command);
}