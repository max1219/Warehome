using Microsoft.AspNetCore.Mvc;
using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;
using Warehome.Application.Services;
using Warehome.Web.DTO.Input;
using Warehome.Web.DTO.Output;

namespace Warehome.Web.Controllers.Api;

[ApiController]
[Route("api/storage-categories")]
public class StorageCategoryController : ControllerBase
{
    private readonly IStorageCategoryService _storageCategoryService;

    public StorageCategoryController(IStorageCategoryService storageCategoryService)
    {
        _storageCategoryService = storageCategoryService;
    }

    [HttpGet("tree")]
    public async Task<ActionResult<GetStorageCategoryTreeResponse>> GetTree()
    {
        GetStorageCategoryTreeResult applicationResult =
            await _storageCategoryService.GetTreeAsync();
        return MapNode(applicationResult);
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
    
    private static GetStorageCategoryTreeResponse MapNode(GetStorageCategoryTreeResult appNode)
    {
        return new GetStorageCategoryTreeResponse
        {
            Name = appNode.Name,
            StorageNames = appNode.StorageNames,
            StorageCount = appNode.StorageNames.Count,
            ChildCount = appNode.Children.Count,
            Children = appNode.Children.Select(MapNode).ToList()
        };
    }
}