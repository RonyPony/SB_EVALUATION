using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SB.BACKEND.Application.GovernmentEntities;
using SB.BACKEND.Domain.GovernmentEntities;

namespace SB.BACKEND.Infrastructure.Persistence.Configurations;

internal sealed class GovernmentEntityConfiguration : IEntityTypeConfiguration<EntidadGubernamental>
{
    public void Configure(EntityTypeBuilder<EntidadGubernamental> b)
    {
        b.ToTable("EntidadesGubernamentales");
        b.HasKey(x => x.Id);
        b.Property(x => x.Nombre)
            .HasMaxLength(GovernmentEntityLengths.NAME)
            .IsUnicode()
            .IsRequired();
        b.Property(x => x.NombreNormalizado)
            .HasMaxLength(GovernmentEntityLengths.NAME)
            .IsUnicode()
            .IsRequired();
        b.Property(x => x.Categoria)
            .HasMaxLength(GovernmentEntityLengths.CATEGORY)
            .IsUnicode()
            .IsRequired();
        b.Property(x => x.PoderDelEstado)
            .HasMaxLength(GovernmentEntityLengths.STATE_POWER)
            .IsUnicode()
            .IsRequired();
        b.Property(x => x.Sector)
            .HasMaxLength(GovernmentEntityLengths.SECTOR)
            .IsUnicode()
            .IsRequired();
        b.Property(x => x.IsActive).HasColumnName("Activo").IsRequired();
        b.Property(x => x.IsDeleted).IsRequired();
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasQueryFilter(x => !x.IsDeleted);
        b.HasIndex(x => x.NombreNormalizado).IsUnique().HasFilter("[IsDeleted] = 0");
        b.HasIndex(x => new { x.IsDeleted, x.IsActive });
        b.HasIndex(x => x.Categoria);
        b.HasIndex(x => x.PoderDelEstado);
        b.HasIndex(x => x.Sector);
    }
}
