using Microsoft.EntityFrameworkCore;
using Warehome.Application.Repositories;
using Warehome.Domain.Entities;
using Warehome.Infrastructure.Data.Entities;
using DomainStorage = Warehome.Domain.Entities.Storage;
using InfrastructureStorage = Warehome.Infrastructure.Data.Entities.Storage;

namespace Warehome.Infrastructure.Data.Repositories;

public class EfStorageRepository(AppDbContext context) : IStorageRepository
{
    private readonly AppDbContext _context = context;

    public async Task<DomainStorage?> GetAsync(string name, Category<DomainStorage>? category)
    {
        InfrastructureStorage? storage = await _context.Storages.Where(
                s => s.Name == name
                     && (s.CategoryId == null && category == null
                         || s.CategoryId != null && category != null
                                                 && s.Category!.Path == category.Path))
            .FirstOrDefaultAsync();

        return storage == null ? null : new DomainStorage { Name = storage.Name, Category = category };
    }

    public IAsyncEnumerable<DomainStorage> GetAllByCategoryAsync(Category<DomainStorage>? category)
    {
        return _context.Storages
            .Where(s => s.CategoryId == null && category == null
                        || s.CategoryId != null && category != null
                                                && s.Category!.Path == category.Path)
            .Select(s => new DomainStorage { Name = s.Name, Category = category })
            .AsAsyncEnumerable();
    }

    public async Task<bool> TryAddAsync(DomainStorage storage)
    {
        bool isExist = await _context.Storages.AnyAsync(
            s => s.Name == storage.Name
                 && (s.CategoryId == null && storage.Category == null
                     || s.CategoryId != null && storage.Category != null
                                             && s.Category!.Path == storage.Category.Path));
        if (isExist)
        {
            return false;
        }

        StorageCategory? category = null;
        if (storage.Category is not null)
        {
            category = await _context.StorageCategories.Where(
                c => c.Path == storage.Category.Path).FirstAsync();
        }

        await _context.Storages.AddAsync(new InfrastructureStorage { Name = storage.Name, Category = category });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(DomainStorage storage)
    {
        int deleted = await _context.Storages.Where(s => s.Name == storage.Name).ExecuteDeleteAsync();
        return deleted > 0;
    }
}