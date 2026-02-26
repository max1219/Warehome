using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;
using Warehome.Application.Services;
using Warehome.Web.DTO.Input;
using Warehome.Web.DTO.Output;

namespace Warehome.Web.Controllers.Api;

[ApiController]
[Route("api/item-types")]
public class ItemTypeController(IItemTypeService itemTypeService) : ControllerBase
{
    private readonly IItemTypeService _itemTypeService = itemTypeService;

    [HttpPost]
    public async Task<ActionResult<CreateItemTypeResponse>> Post(
        [FromBody] CreateItemTypeRequest request)
    {
        CreateItemTypeStatus status =
            await _itemTypeService.CreateItemTypeAsync(new CreateItemTypeCommand
            {
                Name = request.Name,
                CategoryPath = request.CategoryPath
            });

        return status switch
        {
            CreateItemTypeStatus.Success => Ok(CreateItemTypeResponse.Success),
            CreateItemTypeStatus.AlreadyExists => Conflict(CreateItemTypeResponse.AlreadyExists),
            CreateItemTypeStatus.CategoryNotFound => NotFound(CreateItemTypeResponse.CategoryNotFound),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    [HttpDelete]
    public async Task<ActionResult<DeleteItemTypeResponse>> Delete(
        [FromBody] DeleteItemTypeRequest request)
    {
        DeleteItemTypeStatus status =
            await _itemTypeService.DeleteItemTypeAsync(new DeleteItemTypeCommand
            {
                Name = request.Name,
                CategoryPath = request.CategoryPath
            });

        return status switch
        {
            DeleteItemTypeStatus.Success => Ok(DeleteItemTypeResponse.Success),
            DeleteItemTypeStatus.NotFound => NotFound(DeleteItemTypeResponse.NotFound),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}