using ArtifactStore.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtifactStore.Infrastructure.Configurations;

public class StoreTransactionStatusConfiguration : IEntityTypeConfiguration<StoreTransactionStatus>
{
    public void Configure(EntityTypeBuilder<StoreTransactionStatus> builder)
    {
        builder.ToTable("store_transaction_statuses");

        builder.HasKey(c => c.Id);
        builder.Property(b => b.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");
        builder.Property(b => b.Name)
            .HasColumnName("name")
            .HasMaxLength(32)
            .IsRequired();
        builder.HasIndex(b => b.Name)
            .IsUnique();
    }
}