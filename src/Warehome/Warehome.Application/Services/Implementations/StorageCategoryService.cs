using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;
using Warehome.Application.Repositories;
using Warehome.Domain.Entities;

namespace Warehome.Application.Services.Implementations;

public class StorageCategoryService(
    ICategoryRepository<Storage> storageCategoryRepository,
    IStorageRepository storageRepository)
    : IStorageCategoryService
{
    private readonly IStorageRepository _storageRepository = storageRepository;
    private readonly ICategoryRepository<Storage> _storageCategoryRepository = storageCategoryRepository;

    public async Task<GetStorageCategoryTreeResult> GetTreeAsync()
    {
        GetStorageCategoryTreeResult root = new GetStorageCategoryTreeResult { Name = string.Empty };
        Stack<GetStorageCategoryTreeResult> stack = new Stack<GetStorageCategoryTreeResult>([root]);
        Stack<string> paths = new Stack<string>([""]);

        while (stack.Count > 0)
        {
            GetStorageCategoryTreeResult current = stack.Pop();
            string path = paths.Pop();
            Category<Storage>? category = null;
            if (path != "")
            {
                category = new Category<Storage> {Path = path};
            }

            IAsyncEnumerable<Storage> storagesAsync = _storageRepository.GetAllByCategoryAsync(category);
            List<Storage> storages = await storagesAsync.ToListAsync();
            current.StorageNames = storages.Select(x => x.Name).ToList();

            IAsyncEnumerable<Category<Storage>> categoriesAsync =
                _storageCategoryRepository.GetAllByParentAsync(category, false);
            List<Category<Storage>> categories = await categoriesAsync.ToListAsync();
            List<GetStorageCategoryTreeResult> childrenNodes = new List<GetStorageCategoryTreeResult>(categories.Count);
            current.Children = childrenNodes;
            foreach (Category<Storage> child in categories)
            {
                childrenNodes.Add(new GetStorageCategoryTreeResult { Name = child.Name });
                stack.Push(childrenNodes.Last());
                paths.Push(path == "" ? child.Name : $"{path}/{child.Name}");
            }
        }
        
        return root;
    }

    public async Task<CreateStorageCategoryStatus> CreateStorageCategoryAsync(CreateStorageCategoryCommand command)
    {
        Category<Storage> category;
        Category<Storage>? parentCategory = null;

        if (command.ParentPath is not null)
        {
            parentCategory = new Category<Storage> { Path = command.ParentPath };
            if (!await _storageCategoryRepository.CheckExistsAsync(parentCategory))
            {
                return CreateStorageCategoryStatus.ParentNotFound;
            }

            category = Category<Storage>.FromNameAndParent(command.Name, command.ParentPath);
        }
        else
        {
            category = new Category<Storage> { Path = command.Name };
        }

        bool isExists = await _storageCategoryRepository.CheckExistsAsync(category);
        if (isExists)
        {
            return CreateStorageCategoryStatus.AlreadyExists;
        }

        await _storageCategoryRepository.AddAsync(category, parentCategory);

        return CreateStorageCategoryStatus.Success;
    }

    public async Task<DeleteStorageCategoryStatus> DeleteStorageCategoryAsync(DeleteStorageCategoryCommand command)
    {
        Category<Storage> category = new Category<Storage> { Path = command.Path };

        if (!await _storageCategoryRepository.CheckExistsAsync(category))
        {
            return DeleteStorageCategoryStatus.NotFound;
        }

        IAsyncEnumerable<Category<Storage>> childCategories =
            _storageCategoryRepository.GetAllByParentAsync(category, false);
        if (await childCategories.AnyAsync())
        {
            return DeleteStorageCategoryStatus.NotEmpty;
        }

        IAsyncEnumerable<Storage> storages = _storageRepository.GetAllByCategoryAsync(category);
        if (await storages.AnyAsync())
        {
            return DeleteStorageCategoryStatus.NotEmpty;
        }

        await _storageCategoryRepository.DeleteAsync(category);
        return DeleteStorageCategoryStatus.Success;
    }
}