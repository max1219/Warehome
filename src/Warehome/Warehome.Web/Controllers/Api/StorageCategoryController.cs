using Microsoft.AspNetCore.Mvc;
using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;
using Warehome.Application.Services;
using Warehome.Web.DTO.Input;
using Warehome.Web.DTO.Output;

namespace Warehome.Web.Controllers.Api;

[ApiController]
[Route("api/storage-categories")]
public class StorageCategoryController(IStorageCategoryService storageCategoryService) : ControllerBase
{
    private readonly IStorageCategoryService _storageCategoryService = storageCategoryService;

    [HttpGet("tree")]
    public async Task<ActionResult<GetStorageCategoryTreeResponse>> GetTree()
    {
        GetStorageCategoryTreeResult applicationResult =
            await _storageCategoryService.GetTreeAsync();
        GetStorageCategoryTreeResponse response = new GetStorageCategoryTreeResponse();
        Stack<GetStorageCategoryTreeResult> appResultStack =
            new Stack<GetStorageCategoryTreeResult>([applicationResult]);
        Stack<GetStorageCategoryTreeResponse> responseStack = 
            new Stack<GetStorageCategoryTreeResponse>([response]);

        while (appResultStack.Any())
        {
            GetStorageCategoryTreeResult appResultNode = appResultStack.Pop();
            GetStorageCategoryTreeResponse responseNode = responseStack.Pop();
            responseNode.Name = appResultNode.Name;
            responseNode.StorageNames = appResultNode.StorageNames;
            responseNode.StorageCount = appResultNode.StorageNames.Count;
            responseNode.ChildCount = appResultNode.Children.Count;
            List<GetStorageCategoryTreeResponse> children = new (responseNode.ChildCount);
            foreach (GetStorageCategoryTreeResult child in appResultNode.Children)
            {
                children.Add(new GetStorageCategoryTreeResponse());
                responseStack.Push(children.Last());
                appResultStack.Push(child);
            }
            responseNode.Children = children;
        }
        
        return response;
    }

    [HttpPost]
    public async Task<ActionResult<CreateStorageCategoryResponse>> Post(
        [FromBody] CreateStorageCategoryRequest request)
    {
        CreateStorageCategoryStatus status =
            await _storageCategoryService.CreateStorageCategoryAsync(new CreateStorageCategoryCommand
            {
                Name = request.Name,
                ParentPath = request.ParentPath
            });
        return status switch
        {
            CreateStorageCategoryStatus.Success => Ok(CreateStorageCategoryResponse.Success),
            CreateStorageCategoryStatus.AlreadyExists => Conflict(CreateStorageCategoryResponse.AlreadyExists),
            CreateStorageCategoryStatus.ParentNotFound => NotFound(CreateStorageCategoryResponse.ParentNotFound),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    [HttpDelete]
    public async Task<ActionResult<DeleteStorageCategoryResponse>> Delete(
        [FromBody] DeleteStorageCategoryRequest request)
    {
        DeleteStorageCategoryStatus status =
            await _storageCategoryService.DeleteStorageCategoryAsync(new DeleteStorageCategoryCommand
            {
                Path = request.Path
            });
        return status switch
        {
            DeleteStorageCategoryStatus.Success => Ok(DeleteStorageCategoryResponse.Success),
            DeleteStorageCategoryStatus.NotEmpty => Conflict(DeleteStorageCategoryResponse.NotEmpty),
            DeleteStorageCategoryStatus.NotFound => NotFound(DeleteStorageCategoryResponse.NotFound),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}