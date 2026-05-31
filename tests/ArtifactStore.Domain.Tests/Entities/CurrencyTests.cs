using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Domain.Tests.Entities;

public class CurrencyTests
{
    [Fact]
    public void Constructor_SetsPropertiesAndGeneratesId()
    {
        var currency = new Currency(Guid.NewGuid(), "Silver", null);

        Assert.Equal("Silver", currency.Name);
        Assert.NotEqual(Guid.Empty, currency.Id);
    }


}
