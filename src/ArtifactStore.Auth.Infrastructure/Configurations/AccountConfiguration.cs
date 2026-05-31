using ArtifactStore.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtifactStore.Auth.Infrastructure.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("accounts");
        
        builder.HasKey(c => c.Id);
        builder.Property(b => b.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");
        builder.Property(p => p.AccountRoleId)
            .HasColumnName("role_id");
        builder.HasOne(c => c.AccountRole)
            .WithMany()
            .IsRequired()
            .HasForeignKey(c => c.AccountRoleId);
        builder.Property(p => p.Nickname)
            .HasColumnName("nickname")
            .HasMaxLength(32)
            .IsRequired();
        builder.HasIndex(p => p.Nickname)
            .IsUnique();
        builder.Property(p => p.Password)
            .HasColumnName("password")
            .HasMaxLength(256)
            .IsRequired();
        builder.Property(p => p.Email)
            .HasColumnName("email")
            .HasMaxLength(254)
            .IsRequired();
        builder.HasIndex(p => p.Email)
            .IsUnique();
        builder.Property(p => p.RegisterDate)
            .HasColumnName("register_date")
            .HasColumnType("date")
            .HasDefaultValueSql("now()")
            .IsRequired();
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