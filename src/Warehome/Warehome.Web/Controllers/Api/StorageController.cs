using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;
using Warehome.Application.Services;
using Warehome.Web.DTO.Input;
using Warehome.Web.DTO.Output;

namespace Warehome.Web.Controllers.Api;

[ApiController]
[Route("api/storages")]
public class StorageController(IStorageService storageService) : ControllerBase
{
    private readonly IStorageService _storageService = storageService;

    [HttpPost]
    public async Task<ActionResult<CreateStorageResponse>> Post(
        [FromBody] CreateStorageRequest request)
    {
        CreateStorageStatus status =
            await _storageService.CreateStorageAsync(new CreateStorageCommand
            {
                Name = request.Name,
                CategoryPath = request.CategoryPath
            });

        return status switch
        {
            CreateStorageStatus.Success => Ok(CreateStorageResponse.Success),
            CreateStorageStatus.AlreadyExists => Conflict(CreateStorageResponse.AlreadyExists),
            CreateStorageStatus.CategoryNotFound => NotFound(CreateStorageResponse.CategoryNotFound),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    [HttpDelete]
    public async Task<ActionResult<DeleteStorageResponse>> Delete(
        [FromBody] DeleteStorageRequest request)
    {
        DeleteStorageStatus status =
            await _storageService.DeleteStorageAsync(new DeleteStorageCommand
            {
                Name = request.Name,
                CategoryPath = request.CategoryPath
            });

        return status switch
        {
            DeleteStorageStatus.Success => Ok(DeleteStorageResponse.Success),
            DeleteStorageStatus.NotEmpty => Conflict(DeleteStorageResponse.NotEmpty),
            DeleteStorageStatus.NotFound => NotFound(DeleteStorageResponse.NotFound),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}