using Microsoft.AspNetCore.Mvc;
using Warehome.Application.Services;
using Warehome.Web.DTO.Input;
using Warehome.Web.DTO.Output;
using AppDto = Warehome.Application.DTO;

namespace Warehome.Web.Controllers.Api;

[ApiController]
[Route("api/item-stocks")]
public class ItemStockController(IItemStockService itemStockService) : ControllerBase
{
    private readonly IItemStockService _itemStockService = itemStockService;

    [HttpPost]
    public async Task<ActionResult<CreateItemStockResponse>> Post([FromBody] CreateItemStockRequest request)
    {
        AppDto.Output.CreateItemStockResult result =
            await _itemStockService.CreateItemStockAsync(new AppDto.Input.CreateItemStockCommand
            {
                Quantity = request.Quantity,
                ItemTypeName = request.ItemTypeName,
                ItemTypeCategoryPath = request.ItemTypeCategoryPath,
                StorageName = request.StorageName,
                StorageCategoryPath = request.StorageCategoryPath
            });

        return result.Status switch
        {
            AppDto.Output.CreateItemStockStatus.Success => Ok(new CreateItemStockResponse
            {
                Id = result.Id,
                Status = CreateItemStockStatus.Success
            }),
            AppDto.Output.CreateItemStockStatus.StorageNotFound => NotFound(new CreateItemStockResponse
            {
                Status = CreateItemStockStatus.StorageNotFound
            }),
            AppDto.Output.CreateItemStockStatus.ItemTypeNotFound => NotFound(new CreateItemStockResponse
            {
                Status = CreateItemStockStatus.ItemTypeNotFound
            }),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    [HttpDelete]
    public async Task<ActionResult<DeleteItemStockStatus>> Delete([FromBody] int id)
    {
        AppDto.Output.DeleteItemStockStatus status =
            await _itemStockService.DeleteItemStockAsync(id);

        return status switch
        {
            AppDto.Output.DeleteItemStockStatus.Success => Ok(DeleteItemStockStatus.Success),
            AppDto.Output.DeleteItemStockStatus.NotFound => NotFound(DeleteItemStockStatus.NotFound),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    [HttpPatch("quantity")]
    public async Task<ActionResult<ChangeItemStockQuantityStatus>> ChangeQuantity(
        [FromBody] ChangeItemStockQuantityRequest request)
    {
        AppDto.Output.ChangeItemStockQuantityStatus status =
            await _itemStockService.ChangeItemStockQuantityAsync(new AppDto.Input.ChangeItemStockQuantityCommand
            {
                Id = request.Id,
                NewQuantity = request.NewQuantity
            });

        return status switch
        {
            AppDto.Output.ChangeItemStockQuantityStatus.Success => Ok(ChangeItemStockQuantityStatus.Success),
            AppDto.Output.ChangeItemStockQuantityStatus.NotFound => NotFound(ChangeItemStockQuantityStatus.NotFound),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    [HttpPost("by-storage")]
    public async Task<ActionResult<GetAllStocksByStorageResponse>> GetByStorage(
        [FromBody] GetItemStocksByStorageRequest request)
    {
        AppDto.Output.GetAllStocksByStorageResult result =
            await _itemStockService.GetAllByStorageAsync(new AppDto.Input.GetItemStocksByStorageCommand
            {
                StorageName = request.StorageName,
                StorageCategoryPath = request.StorageCategoryPath
            });

        return result.Status switch
        {
            AppDto.Output.GetAllStocksByStorageStatus.Success => Ok(new GetAllStocksByStorageResponse
            {
                Result = result.Result!.Select(r => new GetItemStockResponse()
                {
                    Id = r.Id,
                    ItemTypeName = r.ItemTypeName,
                    ItemTypeCategoryPath = r.ItemTypeCategoryPath,
                    StorageName = r.StorageName,
                    StorageCategoryPath = r.StorageCategoryPath,
                    Quantity = r.Quantity
                }),
                Status = GetAllStocksByStorageStatus.Success
            }),
            AppDto.Output.GetAllStocksByStorageStatus.StorageNotFound => NotFound(new GetAllStocksByStorageResponse
            {
                Status = GetAllStocksByStorageStatus.StorageNotFound
            }),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    [HttpPost("by-type")]
    public async Task<ActionResult<GetAllStocksByTypeResponse>> GetByType([FromBody] GetItemStocksByTypeRequest request)
    {
        AppDto.Output.GetAllStocksByTypeResult result =
            await _itemStockService.GetAllByTypeAsync(new AppDto.Input.GetItemStocksByTypeCommand
            {
                ItemTypeName = request.ItemTypeName,
                ItemTypeCategoryPath = request.ItemTypeCategoryPath
            });

        return result.Status switch
        {
            AppDto.Output.GetAllStocksByTypeStatus.Success => Ok(new GetAllStocksByTypeResponse
            {
                Result = result.Result!.Select(r => new GetItemStockResponse()
                {
                    Id = r.Id,
                    ItemTypeName = r.ItemTypeName,
                    ItemTypeCategoryPath = r.ItemTypeCategoryPath,
                    StorageName = r.StorageName,
                    StorageCategoryPath = r.StorageCategoryPath,
                    Quantity = r.Quantity
                }),
                Status = GetAllStocksByTypeStatus.Success
            }),
            AppDto.Output.GetAllStocksByTypeStatus.TypeNotFound => NotFound(new GetAllStocksByTypeResponse
            {
                Status = GetAllStocksByTypeStatus.TypeNotFound
            }),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}