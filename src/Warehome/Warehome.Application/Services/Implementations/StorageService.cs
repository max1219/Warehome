using Warehome.Application.DTO.Input;
using Warehome.Application.DTO.Output;
using Warehome.Application.Repositories;
using Warehome.Domain.Entities;

namespace Warehome.Application.Services.Implementations;

public class StorageService(IStorageRepository storageRepository) : IStorageService
{
    private readonly IStorageRepository _storageRepository = storageRepository;

    public async Task<CreateStorageStatus> CreateStorageAsync(CreateStorageDto dto)
    {
        bool isSuccess = await _storageRepository.TryAddAsync(new Storage { Name = dto.Name });

        return isSuccess ? CreateStorageStatus.Success : CreateStorageStatus.AlreadyExists;
    }

    public async Task<DeleteStorageStatus> DeleteStorageAsync(string path)
    {
        Storage storage = await _storageRepository.GetByPathAsync(path);
        await _storageRepository.DeleteAsync(storage);
        return DeleteStorageStatus.Success;
    }

    public async Task<IEnumerable<GetStorageResponse>> GetAllAsync()
    {
         IEnumerable<Storage> storages = await _storageRepository.GetAllAsync();
         return storages.Select(storage => new GetStorageResponse { Name = storage.Name });
    }
}