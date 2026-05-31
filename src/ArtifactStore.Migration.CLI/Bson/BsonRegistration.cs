using ArtifactStore.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using AuthAccount = ArtifactStore.Auth.Domain.Entities.Account;

namespace ArtifactStore.Migration.CLI.Bson;

public static class BsonRegistration
{
    private static bool _registered;

    public static void Register()
    {
        if (_registered) return;
        _registered = true;

        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        var pack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
        ConventionRegistry.Register("MigrationConventions", pack, _ => true);

        BsonClassMap.TryRegisterClassMap<Balance>(cm =>
        {
            cm.AutoMap();
            cm.UnmapMember(b => b.Character);
            cm.UnmapMember(b => b.Currency);
        });

        BsonClassMap.TryRegisterClassMap<InventoryItem>(cm =>
        {
            cm.AutoMap();
            cm.UnmapMember(i => i.Character);
            cm.UnmapMember(i => i.Artifact);
        });

        BsonClassMap.TryRegisterClassMap<StoreItem>(cm =>
        {
            cm.AutoMap();
            cm.UnmapMember(s => s.Currency);
            cm.UnmapMember(s => s.Artifact);
            cm.UnmapMember(s => s.Discount);
        });

        BsonClassMap.TryRegisterClassMap<StoreTransaction>(cm =>
        {
            cm.AutoMap();
            cm.UnmapMember(s => s.Balance);
            cm.UnmapMember(s => s.Artifact);
            cm.UnmapMember(s => s.StoreTransactionType);
            cm.UnmapMember(s => s.StoreTransactionStatus);
        });

        BsonClassMap.TryRegisterClassMap<AuthAccount>(cm =>
        {
            cm.AutoMap();
            cm.UnmapMember(a => a.AccountRole);
        });
    }
}
