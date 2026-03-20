using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;
using Warehome.Application.Repositories;
using Warehome.Domain.Entities;

namespace Warehome.Application.Services.Implementations;

public class ItemTypeService(
    IItemTypeRepository itemTypeRepository,
    ICategoryRepository<ItemType> categoryRepository,
    IItemStockRepository stockRepository)
    : IItemTypeService
{
    private readonly IItemTypeRepository _itemTypeRepository = itemTypeRepository;
    private readonly ICategoryRepository<ItemType> _categoryRepository = categoryRepository;
    private readonly IItemStockRepository _stockRepository = stockRepository;

    public async Task<CreateItemTypeStatus> CreateItemTypeAsync(CreateItemTypeCommand command)
    {
        Category<ItemType>? category = null;
        if (command.CategoryPath is not null)
        {
            category = new Category<ItemType> { Path = command.CategoryPath };
            if (! await _categoryRepository.CheckExistsAsync(category))
            {
                return CreateItemTypeStatus.CategoryNotFound;
            }
        }
        
        bool isExists = await _itemTypeRepository.GetAsync(command.Name, category) != null;
        if (isExists)
        {
            return CreateItemTypeStatus.AlreadyExists;
        }

        await _itemTypeRepository.AddAsync(new ItemType { Name = command.Name, Category = category });

        return CreateItemTypeStatus.Success;
    }

    public async Task<DeleteItemTypeStatus> DeleteItemTypeAsync(DeleteItemTypeCommand command)
    {
        Category<ItemType>? category = null;
        if (command.CategoryPath is not null)
        {
            category = new Category<ItemType> { Path = command.CategoryPath };
            if (! await _categoryRepository.CheckExistsAsync(category))
            {
                return DeleteItemTypeStatus.NotFound;
            }
        }
        
        ItemType? itemType = await _itemTypeRepository.GetAsync(command.Name, category);
        if (itemType == null)
        {
            return DeleteItemTypeStatus.NotFound;
        }

        if (await _stockRepository.GetAllByTypeAsync(itemType).AnyAsync())
        {
            return DeleteItemTypeStatus.HasStock;
        }
        
        await _itemTypeRepository.DeleteAsync(itemType);
        return DeleteItemTypeStatus.Success;
    }
}