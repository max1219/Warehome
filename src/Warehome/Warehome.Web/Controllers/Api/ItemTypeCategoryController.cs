using Microsoft.AspNetCore.Mvc;
using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;
using Warehome.Application.Services;
using Warehome.Web.DTO.Input;
using Warehome.Web.DTO.Output;

namespace Warehome.Web.Controllers.Api;

[ApiController]
[Route("api/item-type-categories")]
public class ItemTypeCategoryController(IItemTypeCategoryService itemTypeCategoryService) : ControllerBase
{
    private readonly IItemTypeCategoryService _itemTypeCategoryService = itemTypeCategoryService;

    [HttpGet("tree")]
    public async Task<ActionResult<GetItemTypeCategoryTreeResponse>> GetTree()
    {
        GetItemTypeCategoryTreeResult applicationResult =
            await _itemTypeCategoryService.GetTreeAsync();
        GetItemTypeCategoryTreeResponse response = new GetItemTypeCategoryTreeResponse();
        Stack<GetItemTypeCategoryTreeResult> appResultStack =
            new Stack<GetItemTypeCategoryTreeResult>([applicationResult]);
        Stack<GetItemTypeCategoryTreeResponse> responseStack = 
            new Stack<GetItemTypeCategoryTreeResponse>([response]);

        while (appResultStack.Any())
        {
            GetItemTypeCategoryTreeResult appResultNode = appResultStack.Pop();
            GetItemTypeCategoryTreeResponse responseNode = responseStack.Pop();
            responseNode.Name = appResultNode.Name;
            responseNode.ItemNames = appResultNode.ItemNames;
            responseNode.ItemTypeCount = appResultNode.ItemNames.Count;
            responseNode.ChildCount = appResultNode.Children.Count;
            List<GetItemTypeCategoryTreeResponse> children = new (responseNode.ChildCount);
            foreach (GetItemTypeCategoryTreeResult child in appResultNode.Children)
            {
                children.Add(new GetItemTypeCategoryTreeResponse());
                responseStack.Push(children.Last());
                appResultStack.Push(child);
            }
            responseNode.Children = children;
        }
        
        return response;
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
}