using ArtifactStore.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtifactStore.Infrastructure.Configurations;

public class StoreTransactionTypeConfiguration : IEntityTypeConfiguration<StoreTransactionType>
{
    public void Configure(EntityTypeBuilder<StoreTransactionType> builder)
    {
        builder.ToTable("store_transaction_types");

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