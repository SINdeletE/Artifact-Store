using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using ArtifactStore.Infrastructure.Repositories;

namespace ArtifactStore.Infrastructure.Tests.Repositories;

[Collection("Database")]
public class CharacterRepositoryTests(DatabaseFixture fixture)
{
    private Character NewCharacter(Guid? userId = null) =>
        new(Guid.NewGuid(), userId ?? fixture.SeedAccountId, "Hero", DateOnly.FromDateTime(DateTime.UtcNow), null);

    [Fact]
    public async Task Insert_Character()
    {
        await using var ctx = fixture.CreateContext();
        var repo = new CharacterRepository(ctx);
        var character = NewCharacter();

        var result = await repo.InsertAsync(character);

        await using var verifyCtx = fixture.CreateContext();
        var found = await new CharacterRepository(verifyCtx).GetByIdAsync(result.Id);
        Assert.NotNull(found);
        Assert.Equal(character.Name, found.Name);
    }

    [Fact]
    public async Task GetById_ReturnsException_WhenNotFound()
    {
        await using var ctx = fixture.CreateContext();
        await Assert.ThrowsAsync<NotFoundException>(                                                                                                                                       
            () => new CharacterRepository(ctx).GetByIdAsync(Guid.NewGuid())                                                                                                            
        );
    }

    [Fact]
    public async Task Update_ChangesName()
    {
        await using var ctx = fixture.CreateContext();
        var repo = new CharacterRepository(ctx);
        var character = await repo.InsertAsync(NewCharacter());

        character.Name = "Updated";
        await repo.UpdateAsync(character);
        
        await using var verifyCtx = fixture.CreateContext();
        var found = await new CharacterRepository(verifyCtx).GetByIdAsync(character.Id);
        Assert.Equal("Updated", found.Name);
    }

    [Fact]
    public async Task Delete_RemovesCharacter()
    {
        await using var ctx = fixture.CreateContext();
        var repo = new CharacterRepository(ctx);
        var character = await repo.InsertAsync(NewCharacter());

        await repo.DeleteAsync(character);
        
        await using var verifyCtx = fixture.CreateContext();
        await Assert.ThrowsAsync<NotFoundException>(                                                                                                                                       
            () => new CharacterRepository(verifyCtx).GetByIdAsync(character.Id)                                                                                                            
        );
    }

    [Fact]
    public async Task FindAsync_FiltersByUserId()
    {
        await using var ctx = fixture.CreateContext();
        var repo = new CharacterRepository(ctx);
        var targetUserId = fixture.SeedAccountId;
        var inserted = await repo.InsertAsync(NewCharacter(targetUserId));

        var results = await repo.FindAsync(new UserIdFilter(targetUserId));

        Assert.Contains(results, c => c.Id == inserted.Id);
    }

    private sealed class UserIdFilter(Guid userId) : BaseEntityFilter<Character>
    {
        public override IQueryable<Character> Filter(IQueryable<Character> entities) =>
            entities.Where(c => c.UserId == userId);
    }
}
