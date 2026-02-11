using Microsoft.EntityFrameworkCore;
using Warehome.Application.Repositories;
using DomainStorage = Warehome.Domain.Entities.Storage;
using InfrastructureStorage = Warehome.Infrastructure.Data.Entities.Storage;

namespace Warehome.Infrastructure.Data.Repositories;

public class EfStorageRepository(AppDbContext context) : IStorageRepository
{
    private readonly AppDbContext _context = context;

    public async Task<DomainStorage?> GetByPathAsync(string path)
    {
        InfrastructureStorage? storage =
            await _context.Storages.FirstOrDefaultAsync(storage => storage.Name == path);
        return storage is null ? null : new DomainStorage { Name = storage.Name };
    }

    public async Task<IEnumerable<DomainStorage>> GetAllAsync()
    {
        return await _context.Storages.Select(s => new DomainStorage { Name = s.Name }).ToListAsync();
    }

    public async Task<bool> TryAddAsync(DomainStorage storage)
    {
        bool isExist = await _context.Storages.AnyAsync(s => s.Name == storage.Name);
        if (isExist)
        {
            return false;
        }
        await _context.Storages.AddAsync(new InfrastructureStorage { Name = storage.Name });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(DomainStorage storage)
    {
        int deleted = await _context.Storages.Where(s => s.Name == storage.Name).ExecuteDeleteAsync();
        return deleted > 0;
    }
}