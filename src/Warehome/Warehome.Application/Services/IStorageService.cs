
using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;

namespace Warehome.Application.Services;

public interface IStorageService
{
    Task<CreateStorageStatus> CreateStorageAsync(CreateStorageDto dto);
    Task<DeleteStorageStatus> DeleteStorageAsync(string path);
    Task<IEnumerable<GetStorageResponse>> GetAllAsync();
    
}