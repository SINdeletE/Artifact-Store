using ArtifactStore.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtifactStore.Infrastructure.Configurations;

public class BalanceConfiguration : IEntityTypeConfiguration<Balance>
{
    public void Configure(EntityTypeBuilder<Balance> builder)
    {
        builder.ToTable("balances");
        
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");
        builder.Property(b => b.CharacterId)
            .HasColumnName("character_id");
        builder.HasOne(b => b.Character)
            .WithMany()
            .HasForeignKey(b => b.CharacterId)
            .IsRequired();
        builder.Property(b => b.CurrencyId)
            .HasColumnName("currency_id");
        builder.HasOne(b => b.Currency)
            .WithMany()
            .HasForeignKey(b => b.CurrencyId)
            .IsRequired();
        builder.Property(b => b.Amount)
            .HasColumnName("amount")
            .HasColumnType("numeric(19, 4)")
            .HasDefaultValue(0);
        builder.Property(p => p.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("timestamp with time zone");
        builder.HasQueryFilter(a => a.DeletedAt == null);
        builder.Property(p => p.Version)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .IsRowVersion();
    }
}