using Warehome.Domain.Entities;

namespace Warehome.Application.Repositories;

public interface IStorageRepository
{
    Task<Storage?> GetAsync(string name, Category<Storage>? category);
    IAsyncEnumerable<Storage> GetAllByCategoryAsync(Category<Storage> category);
    Task AddAsync(Storage storage);
    Task DeleteAsync(Storage storage);
    
}