using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;
using Warehome.Application.Repositories;
using Warehome.Domain.Entities;

namespace Warehome.Application.Services.Implementations;

public class ItemTypeCategoryService(
    ICategoryRepository<ItemType> itemTypeCategoryRepository,
    IItemTypeRepository itemTypeRepository)
    : IItemTypeCategoryService
{
    private readonly IItemTypeRepository _itemTypeRepository = itemTypeRepository;
    private readonly ICategoryRepository<ItemType> _itemTypeCategoryRepository = itemTypeCategoryRepository;

    public async Task<GetItemTypeCategoryTreeResult> GetTreeAsync()
    {
        GetItemTypeCategoryTreeResult root = new GetItemTypeCategoryTreeResult { Name = string.Empty };
        Stack<GetItemTypeCategoryTreeResult> stack = new Stack<GetItemTypeCategoryTreeResult>([root]);
        Stack<string> paths = new Stack<string>([""]);

        while (stack.Count > 0)
        {
            GetItemTypeCategoryTreeResult current = stack.Pop();
            string path = paths.Pop();
            Category<ItemType>? category = null;
            if (path != "")
            {
                category = new Category<ItemType> {Path = path};
            }

            IAsyncEnumerable<ItemType> itemTypesAsync = _itemTypeRepository.GetAllByCategoryAsync(category);
            List<ItemType> itemTypes = await itemTypesAsync.ToListAsync();
            current.ItemNames = itemTypes.Select(x => x.Name).ToList();

            IAsyncEnumerable<Category<ItemType>> categoriesAsync =
                _itemTypeCategoryRepository.GetAllByParentAsync(category, false);
            List<Category<ItemType>> categories = await categoriesAsync.ToListAsync();
            List<GetItemTypeCategoryTreeResult> childrenNodes = new List<GetItemTypeCategoryTreeResult>(categories.Count);
            current.Children = childrenNodes;
            foreach (Category<ItemType> child in categories)
            {
                childrenNodes.Add(new GetItemTypeCategoryTreeResult { Name = child.Name });
                stack.Push(childrenNodes.Last());
                paths.Push(path == "" ? child.Name : $"{path}/{child.Name}");
            }
        }
        
        return root;
    }

    public async Task<CreateItemTypeCategoryStatus> CreateItemTypeCategoryAsync(CreateItemTypeCategoryCommand command)
    {
        Category<ItemType> category;
        Category<ItemType>? parentCategory = null;

        if (command.ParentPath is not null)
        {
            parentCategory = new Category<ItemType> { Path = command.ParentPath };
            if (!await _itemTypeCategoryRepository.CheckExistsAsync(parentCategory))
            {
                return CreateItemTypeCategoryStatus.ParentNotFound;
            }

            category = Category<ItemType>.FromNameAndParent(command.Name, command.ParentPath);
        }
        else
        {
            category = new Category<ItemType> { Path = command.Name };
        }

        bool isExists = await _itemTypeCategoryRepository.CheckExistsAsync(category);
        if (isExists)
        {
            return CreateItemTypeCategoryStatus.AlreadyExists;
        }

        await _itemTypeCategoryRepository.AddAsync(category, parentCategory);

        return CreateItemTypeCategoryStatus.Success;
    }

    public async Task<DeleteItemTypeCategoryStatus> DeleteItemTypeCategoryAsync(DeleteItemTypeCategoryCommand command)
    {
        Category<ItemType> category = new Category<ItemType> { Path = command.Path };

        if (!await _itemTypeCategoryRepository.CheckExistsAsync(category))
        {
            return DeleteItemTypeCategoryStatus.NotFound;
        }

        IAsyncEnumerable<Category<ItemType>> childCategories =
            _itemTypeCategoryRepository.GetAllByParentAsync(category, false);
        if (await childCategories.AnyAsync())
        {
            return DeleteItemTypeCategoryStatus.NotEmpty;
        }

        IAsyncEnumerable<ItemType> itemTypes = _itemTypeRepository.GetAllByCategoryAsync(category);
        if (await itemTypes.AnyAsync())
        {
            return DeleteItemTypeCategoryStatus.NotEmpty;
        }

        await _itemTypeCategoryRepository.DeleteAsync(category);
        return DeleteItemTypeCategoryStatus.Success;
    }
}