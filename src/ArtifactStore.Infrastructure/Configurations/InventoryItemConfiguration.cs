using ArtifactStore.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtifactStore.Infrastructure.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("inventory_items");
        
        builder.HasKey(p => p.Id);
        builder.Property(b => b.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");
        builder.Property(p => p.CharacterId)
            .HasColumnName("character_id");
        builder.HasOne(p => p.Character)
            .WithMany()
            .HasForeignKey(p => p.CharacterId)
            .IsRequired();
        builder.Property(p => p.ArtifactId)
            .HasColumnName("artifact_id");
        builder.HasOne(p => p.Artifact)
            .WithMany()
            .HasForeignKey(p => p.ArtifactId)
            .IsRequired();
        builder.Property(p => p.Version)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .IsRowVersion();
    }
}