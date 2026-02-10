using Warehome.Domain.Entities;

namespace Warehome.Application.Repositories;

public interface IStorageRepository
{
    Task<Storage?> GetByPathAsync(string path);
    Task<IEnumerable<Storage>> GetAllAsync();
    Task<bool> TryAddAsync(Storage storage);
    Task<bool> DeleteAsync(Storage storage);
    
}