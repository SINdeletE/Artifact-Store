using ArtifactStore.Application.Interfaces.Repositories.Base;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Interfaces.Repositories.Store;

public interface IStoreRepository : IReadRepository<StoreItem>, 
    IWriteRepository<StoreItem>, IFilterRepository<StoreItem> { }