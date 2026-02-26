using Microsoft.EntityFrameworkCore;
using Warehome.Application.Repositories;
using Warehome.Domain.Entities;
using Warehome.Infrastructure.Data.Entities;
using ItemType = Warehome.Domain.Entities.ItemType;

namespace Warehome.Infrastructure.Data.Repositories;

public class EfItemTypeCategoryRepository(AppDbContext context) : ICategoryRepository<ItemType>
{
    private readonly AppDbContext _context = context;

    public Task<bool> CheckExistsAsync(Category<ItemType> category)
    {
        return _context.ItemTypeCategories
            .Where(c => c.Path == category.Path)
            .AnyAsync();
    }

    public async IAsyncEnumerable<Category<ItemType>> GetAllByParentAsync(
        Category<ItemType>? parent,
        bool recursive)
    {
        int? parentId = null;
        if (parent is not null)
        {
            ItemTypeCategory? parentData = await _context.ItemTypeCategories
                .FirstOrDefaultAsync(c => c.Path == parent.Path);

            if (parentData == null)
                yield break;
            
            parentId = parentData.Id;
        }

        IAsyncEnumerable<ItemTypeCategory> categories = _context.ItemTypeCategories
            .Where(c => c.ParentId == parentId)
            .AsAsyncEnumerable();

        await foreach (ItemTypeCategory category in categories)
        {
            Category<ItemType> result = new Category<ItemType> { Path = category.Path };
            yield return result;
            if (recursive)
            {
                await foreach (Category<ItemType> categoryChild in GetAllByParentAsync(result, true))
                {
                    yield return categoryChild;
                }
            }
        }
    }

    public async Task AddAsync(Category<ItemType> category, Category<ItemType>? parent)
    {

        int? parentId = null;
        if (parent is not null)
        {
            parentId = (await _context.ItemTypeCategories
                .FirstAsync(c => c.Path == parent.Path)).Id;
        }

        _context.ItemTypeCategories.Add(new ItemTypeCategory { Path = category.Path, ParentId = parentId });
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Category<ItemType> category)
    {
        await _context.ItemTypeCategories.Where(c => c.Path == category.Path).ExecuteDeleteAsync();
    }
}