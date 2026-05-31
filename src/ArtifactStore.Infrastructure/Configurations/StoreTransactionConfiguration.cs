using ArtifactStore.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtifactStore.Infrastructure.Configurations;

public class StoreTransactionConfiguration : IEntityTypeConfiguration<StoreTransaction>
{
    public void Configure(EntityTypeBuilder<StoreTransaction> builder)
    {
        builder.ToTable("store_transactions");
        
        builder.HasKey(c => c.Id);
        builder.Property(b => b.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");
        builder.Property(p => p.BalanceId)
            .HasColumnName("balance_id");
        builder.HasOne(c => c.Balance)
            .WithMany()
            .IsRequired()
            .HasForeignKey(c => c.BalanceId);
        builder.Property(p => p.ArtifactId)
            .HasColumnName("artifact_id");
        builder.HasOne(c => c.Artifact)
            .WithMany()
            .IsRequired()
            .HasForeignKey(c => c.ArtifactId);
        builder.Property(p => p.StoreTransactionTypeId)
            .HasColumnName("type_id");
        builder.HasOne(c => c.StoreTransactionType)
            .WithMany()
            .IsRequired()
            .HasForeignKey(c => c.StoreTransactionTypeId);
        builder.Property(p => p.StoreTransactionStatusId)
            .HasColumnName("status_id");
        builder.HasOne(c => c.StoreTransactionStatus)
            .WithMany()
            .IsRequired()
            .HasForeignKey(c => c.StoreTransactionStatusId);
        builder.Property(c => c.Amount)
            .HasColumnName("amount")
            .HasColumnType("numeric(19, 4)")
            .IsRequired()
            .HasDefaultValue(0);
        builder.Property(c => c.TransactionDateTime)
            .HasColumnName("transaction_datetime")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()");
    }
}