
using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;

namespace Warehome.Application.Services;

public interface IStorageService
{
    Task<CreateStorageStatus> CreateStorageAsync(CreateStorageDto dto);
    Task<DeleteStorageStatus> DeleteStorageAsync(DeleteStorageDto dto);
    
}