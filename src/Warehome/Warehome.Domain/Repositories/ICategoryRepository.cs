using Warehome.Domain.Entities;

namespace Warehome.Application.Repositories;

public interface ICategoryRepository<T>
{
    Task<bool> CheckExistsAsync(Category<T> category);
    IAsyncEnumerable<Category<T>> GetAllByParentAsync(Category<T> parent, bool recursive);
    Task<bool> TryAddAsync(Category<T> category, Category<T>? parent);
    Task<bool> DeleteAsync(Category<T> category);
}