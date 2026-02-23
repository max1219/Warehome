
using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;

namespace Warehome.Application.Services;

public interface IStorageService
{
    Task<CreateStorageStatus> CreateStorageAsync(CreateStorageCommand command);
    Task<DeleteStorageStatus> DeleteStorageAsync(DeleteStorageCommand command);
    
}