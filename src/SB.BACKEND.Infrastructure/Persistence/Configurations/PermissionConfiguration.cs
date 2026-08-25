using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SB.BACKEND.Application.Security;
using SB.BACKEND.Domain.Security;

namespace SB.BACKEND.Infrastructure.Persistence.Configurations;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permisos"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.NormalizedName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(250).IsRequired();
        builder.HasIndex(x => x.NormalizedName).IsUnique();

        var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var permissions = Permissions.All.Select((name, index) => new
        {
            Id = Guid.Parse($"10000000-0000-0000-0000-{index + 1:000000000000}"),
            Name = name, NormalizedName = name, Description = $"Allows {name}.",
            CreatedAt = createdAt, UpdatedAt = (DateTimeOffset?)null, IsActive = true
        });
        builder.HasData(permissions);
    }
}
