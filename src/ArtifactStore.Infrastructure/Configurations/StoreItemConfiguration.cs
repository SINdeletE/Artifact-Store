using ArtifactStore.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtifactStore.Infrastructure.Configurations;

public class StoreItemConfiguration : IEntityTypeConfiguration<StoreItem>
{
    public void Configure(EntityTypeBuilder<StoreItem> builder)
    {
        builder.ToTable("store_items");
        
        builder.HasKey(p => p.Id);
        builder.Property(b => b.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");
        builder.Property(p => p.CurrencyId)
            .HasColumnName("currency_id");
        builder.HasOne(p => p.Currency)
            .WithMany()
            .HasForeignKey(p => p.CurrencyId)
            .IsRequired();
        builder.Property(p => p.ArtifactId)
            .HasColumnName("artifact_id");
        builder.HasOne(p => p.Artifact)
            .WithMany()
            .HasForeignKey(p => p.ArtifactId)
            .IsRequired();
        builder.Property(p => p.Price)
            .HasColumnName("price")
            .HasColumnType("numeric(19, 4)")
            .IsRequired()
            .HasDefaultValue(0);
        builder.Property(p => p.Version)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .IsRowVersion();

        builder.Property(p => p.DiscountId)
            .HasColumnName("discount_id")
            .IsRequired(false);
        builder.HasOne(p => p.Discount)
            .WithMany()
            .HasForeignKey(p => p.DiscountId);
    }
}