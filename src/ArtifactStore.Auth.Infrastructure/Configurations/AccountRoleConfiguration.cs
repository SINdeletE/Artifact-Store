using ArtifactStore.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtifactStore.Auth.Infrastructure.Configurations;

public class AccountRoleConfiguration : IEntityTypeConfiguration<AccountRole>
{
    public void Configure(EntityTypeBuilder<AccountRole> builder)
    {
        builder.ToTable("account_roles");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasColumnName("id");
        builder.Property(r => r.Name)
            .HasColumnName("name")
            .HasMaxLength(32)
            .IsRequired();
    }
}