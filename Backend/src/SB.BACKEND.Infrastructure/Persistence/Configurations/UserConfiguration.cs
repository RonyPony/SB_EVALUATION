using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SB.BACKEND.Domain.Security;

namespace SB.BACKEND.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Usuarios");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Username).HasMaxLength(50).IsRequired();
        builder.Property(x => x.NormalizedUsername).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(254).IsRequired();
        builder.Property(x => x.NormalizedEmail).HasMaxLength(254).IsRequired();
        builder.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
        builder.HasIndex(x => x.NormalizedUsername).IsUnique();
        builder.HasIndex(x => x.NormalizedEmail).IsUnique();
    }
}
