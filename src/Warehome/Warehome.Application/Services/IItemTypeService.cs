using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;

namespace Warehome.Application.Services;

public interface IItemTypeService
{
    Task<CreateItemTypeStatus> CreateItemTypeAsync(CreateItemTypeCommand command);
    Task<DeleteItemTypeStatus> DeleteItemTypeAsync(DeleteItemTypeCommand command);
}