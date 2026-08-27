using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SB.BACKEND.Domain.Security;

namespace SB.BACKEND.Infrastructure.Persistence.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
        builder.Property(x => x.NormalizedName).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(250);
        builder.HasIndex(x => x.NormalizedName).IsUnique();
    }
}
