using Microsoft.EntityFrameworkCore;
using Warehome.Application.Repositories;
using Warehome.Domain.Entities;
using Warehome.Infrastructure.Data.Entities;
using DomainItemType = Warehome.Domain.Entities.ItemType;
using InfrastructureItemType = Warehome.Infrastructure.Data.Entities.ItemType;

namespace Warehome.Infrastructure.Data.Repositories;

public class EfItemTypeRepository(AppDbContext context) : IItemTypeRepository
{
    private readonly AppDbContext _context = context;

    public async Task<DomainItemType?> GetAsync(string name, Category<DomainItemType>? category)
    {
        InfrastructureItemType? itemType = await _context.ItemTypes.Where(
                i => i.Name == name
                     && (i.CategoryId == null && category == null
                         || i.CategoryId != null && category != null
                                                 && i.Category!.Path == category.Path))
            .FirstOrDefaultAsync();

        return itemType == null ? null : new DomainItemType { Name = itemType.Name, Category = category };
    }

    public IAsyncEnumerable<DomainItemType> GetAllByCategoryAsync(Category<DomainItemType>? category)
    {
        return _context.ItemTypes
            .Where(i => i.CategoryId == null && category == null
                        || i.CategoryId != null && category != null
                                                && i.Category!.Path == category.Path)
            .Select(i => new DomainItemType { Name = i.Name, Category = category })
            .AsAsyncEnumerable();
    }

    public async Task AddAsync(DomainItemType itemType)
    {

        ItemTypeCategory? category = null;
        if (itemType.Category is not null)
        {
            category = await _context.ItemTypeCategories.Where(
                c => c.Path == itemType.Category.Path).FirstAsync();
        }

        await _context.ItemTypes.AddAsync(new InfrastructureItemType { Name = itemType.Name, Category = category });
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(DomainItemType itemType)
    {
        await _context.ItemTypes.Where(i => i.Name == itemType.Name).ExecuteDeleteAsync();
    }
}