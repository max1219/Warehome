using Microsoft.AspNetCore.Mvc;
using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;
using Warehome.Application.Services;
using Warehome.Web.DTO.Input;
using Warehome.Web.DTO.Output;

namespace Warehome.Web.Controllers.Api;

[ApiController]
[Route("api/item-type-categories")]
public class ItemTypeCategoryController : ControllerBase
{
    private readonly IItemTypeCategoryService _itemTypeCategoryService;

    public ItemTypeCategoryController(IItemTypeCategoryService itemTypeCategoryService)
    {
        _itemTypeCategoryService = itemTypeCategoryService;
    }

    [HttpGet("tree")]
    public async Task<ActionResult<GetItemTypeCategoryTreeResponse>> GetTree()
    {
        GetItemTypeCategoryTreeResult applicationResult = await _itemTypeCategoryService.GetTreeAsync();
        return MapNode(applicationResult);
    }

    [HttpPost]
    public async Task<ActionResult<CreateItemTypeCategoryResponse>> Post(
        [FromBody] CreateItemTypeCategoryRequest request)
    {
        CreateItemTypeCategoryStatus status =
            await _itemTypeCategoryService.CreateItemTypeCategoryAsync(new CreateItemTypeCategoryCommand
            {
                Name = request.Name,
                ParentPath = request.ParentPath
            });
        return status switch
        {
            CreateItemTypeCategoryStatus.Success => Ok(CreateItemTypeCategoryResponse.Success),
            CreateItemTypeCategoryStatus.AlreadyExists => Conflict(CreateItemTypeCategoryResponse.AlreadyExists),
            CreateItemTypeCategoryStatus.ParentNotFound => NotFound(CreateItemTypeCategoryResponse.ParentNotFound),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    [HttpDelete]
    public async Task<ActionResult<DeleteItemTypeCategoryResponse>> Delete(
        [FromBody] DeleteItemTypeCategoryRequest request)
    {
        DeleteItemTypeCategoryStatus status =
            await _itemTypeCategoryService.DeleteItemTypeCategoryAsync(new DeleteItemTypeCategoryCommand
            {
                Path = request.Path
            });
        return status switch
        {
            DeleteItemTypeCategoryStatus.Success => Ok(DeleteItemTypeCategoryResponse.Success),
            DeleteItemTypeCategoryStatus.NotEmpty => Conflict(DeleteItemTypeCategoryResponse.NotEmpty),
            DeleteItemTypeCategoryStatus.NotFound => NotFound(DeleteItemTypeCategoryResponse.NotFound),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    private static GetItemTypeCategoryTreeResponse MapNode(GetItemTypeCategoryTreeResult appNode)
    {
        return new GetItemTypeCategoryTreeResponse
        {
            Name = appNode.Name,
            ItemNames = appNode.ItemNames,
            ItemTypeCount = appNode.ItemNames.Count,
            ChildCount = appNode.Children.Count,
            Children = appNode.Children.Select(MapNode).ToList()
        };
    }

}