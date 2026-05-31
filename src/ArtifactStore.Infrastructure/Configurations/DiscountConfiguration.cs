using ArtifactStore.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtifactStore.Infrastructure.Configurations;

public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.ToTable("discounts");
        
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");
        builder.Property(d => d.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(64)")
            .IsRequired();
        builder.Property(d => d.Description)
            .HasColumnName("description")
            .HasColumnType("varchar(512)")
            .IsRequired()
            .HasDefaultValue(string.Empty);
        builder.Property(d => d.Percent)
            .HasColumnName("percent")
            .HasColumnType("decimal")
            .IsRequired();
        builder.Property(d => d.StartsAt)
            .HasColumnName("starts_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired()
            .HasDefaultValueSql("now()");
        builder.Property(d => d.EndsAt)
            .HasColumnName("ends_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired()
            .HasDefaultValueSql("now()");
        builder.Property(p => p.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("timestamp with time zone");
        builder.Property(p => p.Version)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .IsRowVersion();

        builder.HasQueryFilter(d => d.DeletedAt == null &&
                                    d.StartsAt <= DateTimeOffset.UtcNow &&
                                    DateTimeOffset.UtcNow < d.EndsAt);
    }
}