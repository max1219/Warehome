using Microsoft.EntityFrameworkCore;
using Warehome.Application.Repositories;
using Warehome.Domain.Entities;
using Warehome.Infrastructure.Data.Entities;
using Storage = Warehome.Domain.Entities.Storage;

namespace Warehome.Infrastructure.Data.Repositories;

public class EfStorageCategoryRepository(AppDbContext context) : ICategoryRepository<Storage>
{
    private readonly AppDbContext _context = context;

    public Task<bool> CheckExistsAsync(Category<Storage> category)
    {
        return _context.StorageCategories
            .Where(c => c.Path == category.Path)
            .AnyAsync();
    }

    public async IAsyncEnumerable<Category<Storage>> GetAllByParentAsync(
        Category<Storage> parent,
        bool recursive)
    {
        StorageCategory? parentData = await _context.StorageCategories
            .FirstOrDefaultAsync(c => c.Path == parent.Path);

        if (parentData == null)
            yield break;

        IAsyncEnumerable<StorageCategory> categories = _context.StorageCategories
            .Where(c => c.ParentId == parentData.Id)
            .AsAsyncEnumerable();

        await foreach (StorageCategory category in categories)
        {
            Category<Storage> result = new Category<Storage> { Path = category.Path };
            yield return result;
            if (recursive)
            {
                await foreach (Category<Storage> categoryChild in GetAllByParentAsync(result, true))
                {
                    yield return categoryChild;
                }
            }
        }
    }

    public async Task AddAsync(Category<Storage> category, Category<Storage>? parent)
    {

        int? parentId = null;
        if (parent is not null)
        {
            parentId = (await _context.StorageCategories
                .FirstAsync(c => c.Path == parent.Path)).Id;
        }

        _context.StorageCategories.Add(new StorageCategory { Path = category.Path, ParentId = parentId });
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Category<Storage> category)
    {
        await _context.StorageCategories.Where(c => c.Path == category.Path).ExecuteDeleteAsync();
    }
}