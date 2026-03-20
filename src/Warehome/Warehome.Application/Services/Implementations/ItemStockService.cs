using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;
using Warehome.Application.Repositories;
using Warehome.Domain.Entities;

namespace Warehome.Application.Services.Implementations;

public class ItemStockService : IItemStockService
{
    private readonly IItemStockRepository _stockRepository;
    private readonly IItemTypeRepository _itemTypeRepository;
    private readonly IStorageRepository _storageRepository;

    public ItemStockService(IItemStockRepository stockRepository,
        IItemTypeRepository itemTypeRepository,
        IStorageRepository storageRepository)
    {
        _stockRepository = stockRepository;
        _itemTypeRepository = itemTypeRepository;
        _storageRepository = storageRepository;
    }

    public async Task<CreateItemStockResult> CreateItemStockAsync(CreateItemStockCommand command)
    {
        ItemType itemType = new ItemType
        {
            Name = command.ItemTypeName,
            Category = command.ItemTypeCategoryPath == null
                ? null
                : new Category<ItemType> { Path = command.ItemTypeCategoryPath }
        };
        if (!await _itemTypeRepository.CheckExistsAsync(itemType))
        {
            return new CreateItemStockResult { Id = null, Status = CreateItemStockStatus.ItemTypeNotFound };
        }

        Storage storage = new Storage
        {
            Name = command.StorageName,
            Category = command.StorageCategoryPath == null
                ? null
                : new Category<Storage> { Path = command.StorageCategoryPath }
        };
        if (!await _storageRepository.CheckExistsAsync(storage))
        {
            return new CreateItemStockResult { Id = null, Status = CreateItemStockStatus.StorageNotFound };
        }

        ItemStock itemStock = new ItemStock
        {
            ItemType = itemType,
            Storage = storage,
            Quantity = command.Quantity
        };

        int id = await _stockRepository.AddAsync(itemStock);
        return new CreateItemStockResult { Id = id, Status = CreateItemStockStatus.Success };
    }

    public async Task<DeleteItemStockStatus> DeleteItemStockAsync(int id)
    {
        ItemStock? itemStock = await _stockRepository.GetByIdAsync(id);
        if (itemStock == null)
        {
            return DeleteItemStockStatus.NotFound;
        }

        await _stockRepository.DeleteAsync(itemStock);
        return DeleteItemStockStatus.Success;
    }

    public async Task<ChangeItemStockQuantityStatus> ChangeItemStockQuantityAsync(
        ChangeItemStockQuantityCommand command)
    {
        ItemStock? itemStock = await _stockRepository.GetByIdAsync(command.Id);
        if (itemStock == null)
        {
            return ChangeItemStockQuantityStatus.NotFound;
        }

        itemStock.Quantity = command.NewQuantity;
        await _stockRepository.UpdateAsync(itemStock);
        return ChangeItemStockQuantityStatus.Success;
    }

    public async Task<GetAllStocksByTypeResult> GetAllByTypeAsync(GetItemStocksByTypeCommand command)
    {
        ItemType itemType = new ItemType
        {
            Name = command.ItemTypeName,
            Category = command.ItemTypeCategoryPath == null
                ? null
                : new Category<ItemType> { Path = command.ItemTypeCategoryPath }
        };

        if (!await _itemTypeRepository.CheckExistsAsync(itemType))
        {
            return new GetAllStocksByTypeResult { Result = null, Status = GetAllStocksByTypeStatus.TypeNotFound};
        }
        
        IAsyncEnumerable<ItemStock> enumerable = _stockRepository.GetAllByTypeAsync(itemType);
        IEnumerable<GetItemStockResult> result = 
            await enumerable.Select(GetItemStockResult.FromItemStock).ToListAsync();
        return new GetAllStocksByTypeResult { Result = result, Status = GetAllStocksByTypeStatus.Success };
    }

    public async Task<GetAllStocksByStorageResult> GetAllByStorageAsync(GetItemStocksByStorageCommand command)
    {
        Storage storage = new Storage
        {
            Name = command.StorageName,
            Category = command.StorageCategoryPath == null
                ? null
                : new Category<Storage> { Path = command.StorageCategoryPath }
        };

        if (!await _storageRepository.CheckExistsAsync(storage))
        {
            return new GetAllStocksByStorageResult { Result = null, Status = GetAllStocksByStorageStatus.StorageNotFound};
        }
        
        IAsyncEnumerable<ItemStock> enumerable = _stockRepository.GetAllByStorageAsync(storage);
        IEnumerable<GetItemStockResult> result = 
            await enumerable.Select(GetItemStockResult.FromItemStock).ToListAsync();
        return new GetAllStocksByStorageResult { Result = result, Status = GetAllStocksByStorageStatus.Success };
    }
}