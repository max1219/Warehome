using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Warehome.Application.Repositories;
using Warehome.Domain.Entities;
using Warehome.Infrastructure.Mappers;
using DomainItemStock = Warehome.Domain.Entities.ItemStock;
using InfrastructureItemStock = Warehome.Infrastructure.Data.Entities.ItemStock;

namespace Warehome.Infrastructure.Data.Repositories;

public class EfItemStockRepository(AppDbContext context) : IItemStockRepository
{
    private readonly AppDbContext _context = context;

    public async Task<int> AddAsync(DomainItemStock item)
    {
        int? typeId = await GetItemTypeIdAsync(item.ItemType);

        int? storageId = await GetStorageIdAsync(item.Storage);

        // Проверки в репо, так как без них ef core кидал бы неверные исключения (например, при передаче
        // id=null жаловался бы на null значение not null поля, а не на нарушение по внешнему ключу)
        if (storageId == null)
        {
            throw new Exception("Storage not found");
        }

        if (typeId == null)
        {
            throw new Exception("ItemType not found");
        }

        EntityEntry<InfrastructureItemStock> entity = await _context.ItemStocks.AddAsync(new InfrastructureItemStock
        {
            ItemTypeId = typeId.Value,
            StorageId = storageId.Value,
            Quantity = item.Quantity,
        });
        await _context.SaveChangesAsync();
        return entity.Entity.Id;
    }

    public async Task UpdateAsync(DomainItemStock item)
    {
        int? typeId = await GetItemTypeIdAsync(item.ItemType);

        int? storageId = await GetStorageIdAsync(item.Storage);

        if (storageId == null)
        {
            throw new Exception("New storage not found");
        }

        if (typeId == null)
        {
            throw new Exception("New itemType not found");
        }

        InfrastructureItemStock newStock = _context.ItemStocks.First(x => x.Id == item.Id);

        newStock.ItemTypeId = typeId.Value;
        newStock.StorageId = storageId.Value;
        newStock.Quantity = item.Quantity;
        _context.Update(newStock);
        await _context.SaveChangesAsync();
    }

    public Task DeleteAsync(DomainItemStock item)
    {
        return _context.ItemStocks.Where(s => s.Id == item.Id).ExecuteDeleteAsync();
    }

    public async Task<DomainItemStock?> GetByIdAsync(int id)
    {
        InfrastructureItemStock? infrastructureStock =
            await _context.ItemStocks
                .Include(s => s.ItemType).ThenInclude(itemType => itemType.Category)
                .Include(s => s.Storage).ThenInclude(storage => storage.Category)
                .FirstOrDefaultAsync(s => s.Id == id);

        return infrastructureStock?.ToDomain();
    }

    public async IAsyncEnumerable<DomainItemStock> GetAllByStorageAsync(Storage storage)
    {
        int? storageId = await GetStorageIdAsync(storage);

        if (storageId == null)
        {
            throw new Exception("Storage not found");
        }

        IAsyncEnumerable<DomainItemStock> query = _context.ItemStocks.Where(s => s.StorageId == storageId.Value)
            .Include(s => s.ItemType).ThenInclude(itemType => itemType.Category)
            .Include(s => s.Storage).ThenInclude(st => st.Category)
            .Select(s => s.ToDomain())
            .AsAsyncEnumerable();

        await foreach (DomainItemStock item in query)
        {
            yield return item;
        }
    }

    public async IAsyncEnumerable<DomainItemStock> GetAllByTypeAsync(ItemType type)
    {
        int? typeId = await GetItemTypeIdAsync(type);

        if (typeId == null)
        {
            throw new Exception("Type not found");
        }

        IAsyncEnumerable<DomainItemStock> query = _context.ItemStocks.Where(s => s.ItemTypeId == typeId.Value)
            .Include(s => s.ItemType).ThenInclude(itemType => itemType.Category)
            .Include(s => s.Storage).ThenInclude(st => st.Category)
            .Select(s => s.ToDomain())
            .AsAsyncEnumerable();

        await foreach (DomainItemStock item in query)
        {
            yield return item;
        }
    }

    private async Task<int?> GetStorageIdAsync(Storage storage)
    {
        return (await _context.Storages
            .Where(s => s.Name == storage.Name
                        && (s.Category == null && storage.Category == null
                            || s.Category != null && storage.Category != null
                                                  && s.Category.Path == storage.Category.Path))
            .FirstOrDefaultAsync())?.Id;
    }

    private async Task<int?> GetItemTypeIdAsync(ItemType itemType)
    {
        return (await _context.ItemTypes
            .Where(t => t.Name == itemType.Name
                        && (t.Category == null && itemType.Category == null
                            || t.Category != null && itemType.Category != null
                                                  && t.Category.Path == itemType.Category.Path))
            .FirstOrDefaultAsync())?.Id;
    }
}