using ArtifactStore.Application.Interfaces.Repositories.Base;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Interfaces.Repositories;

public interface IDiscountRepository : IReadRepository<Discount>,
    IWriteRepository<Discount>
{
    
}