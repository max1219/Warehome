using Warehome.Domain.Entities;

namespace Warehome.Application.Repositories;

public interface IItemStockRepository
{
    Task<int> AddAsync(ItemStock item);
    Task UpdateAsync(ItemStock item);
    Task DeleteAsync(ItemStock item);
    IAsyncEnumerable<ItemStock> GetAllByStorageAsync(Storage storage);
    IAsyncEnumerable<ItemStock> GetAllByTypeAsync(ItemType type);
}