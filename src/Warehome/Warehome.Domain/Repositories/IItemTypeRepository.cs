using Warehome.Domain.Entities;

namespace Warehome.Application.Repositories;

public interface IItemTypeRepository
{
    Task<ItemType?> GetAsync(string name, Category<ItemType>? category);
    IAsyncEnumerable<ItemType> GetAllByCategoryAsync(Category<ItemType>? category);
    Task AddAsync(ItemType storage);
    Task DeleteAsync(ItemType storage);
    
}