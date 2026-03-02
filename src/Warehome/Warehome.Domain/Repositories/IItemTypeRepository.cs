using Warehome.Domain.Entities;

namespace Warehome.Application.Repositories;

public interface IItemTypeRepository
{
    Task<bool> CheckExistsAsync(ItemType itemType);
    Task<ItemType?> GetAsync(string name, Category<ItemType>? category);
    IAsyncEnumerable<ItemType> GetAllByCategoryAsync(Category<ItemType>? category);
    Task AddAsync(ItemType itemType);
    Task DeleteAsync(ItemType itemType);
    
}