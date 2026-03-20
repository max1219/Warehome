using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;

namespace Warehome.Application.Services;

public interface IItemStockService
{
    Task<CreateItemStockResult> CreateItemStockAsync(CreateItemStockCommand command);
    Task<DeleteItemStockStatus> DeleteItemStockAsync(int id);
    Task<ChangeItemStockQuantityStatus> ChangeItemStockQuantityAsync(ChangeItemStockQuantityCommand command);
    Task<GetAllStocksByTypeResult> GetAllByTypeAsync(GetItemStocksByTypeCommand command);
    Task<GetAllStocksByStorageResult> GetAllByStorageAsync(GetItemStocksByStorageCommand command);
}