using ArtifactStore.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtifactStore.Infrastructure.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("currencies");
        
        builder.HasKey(p => p.Id);
        builder.Property(b => b.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");
        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(128)")
            .IsRequired();
        builder.HasIndex(p => p.Name)
            .IsUnique();
        builder.Property(p => p.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("timestamp with time zone");
        builder.HasQueryFilter(a => a.DeletedAt == null);
    }
}