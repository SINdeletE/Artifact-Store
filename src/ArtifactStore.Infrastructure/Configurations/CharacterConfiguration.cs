using ArtifactStore.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtifactStore.Infrastructure.Configurations;

public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.ToTable("characters");
        
        builder.HasKey(c => c.Id);
        builder.Property(b => b.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");
        builder.Property(c => c.UserId)
            .HasColumnName("account_id")
            .IsRequired();
        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(32)")
            .IsRequired();
        builder.Property(c => c.CreationDate)
            .HasColumnName("creation_date")
            .HasColumnType("date")
            .IsRequired()
            .HasDefaultValueSql("now()");
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