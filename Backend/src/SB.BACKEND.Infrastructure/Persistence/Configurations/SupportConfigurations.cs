using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SB.BACKEND.Domain.Support;

namespace SB.BACKEND.Infrastructure.Persistence.Configurations;

internal sealed class AreaConfiguration : IEntityTypeConfiguration<Area>
{
    public void Configure(EntityTypeBuilder<Area> b)
    {
        b.ToTable("Areas");
        b.HasKey(x => x.Id);
        b.Property(x => x.Nombre).HasMaxLength(120).IsRequired().IsUnicode();
        b.Property(x => x.NombreNormalizado).HasMaxLength(120).IsRequired().IsUnicode();
        b.Property(x => x.Descripcion).HasMaxLength(500).IsUnicode();
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => x.NombreNormalizado).IsUnique().HasFilter("[IsDeleted] = 0");
        b.HasIndex(x => new { x.IsDeleted, x.IsActive });
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

internal sealed class SolicitudConfiguration : IEntityTypeConfiguration<SolicitudSoporte>
{
    public void Configure(EntityTypeBuilder<SolicitudSoporte> b)
    {
        b.ToTable("SolicitudesSoporte");
        b.HasKey(x => x.Id);
        b.Property(x => x.Codigo).HasMaxLength(30).IsRequired().IsUnicode(false);
        b.Property(x => x.Titulo).HasMaxLength(200).IsRequired().IsUnicode();
        b.Property(x => x.Descripcion).HasMaxLength(4000).IsRequired().IsUnicode();
        b.Property(x => x.ReferenciaEvidencia).HasMaxLength(1000).IsUnicode();
        b.Property(x => x.ComentarioResolucion).HasMaxLength(2000).IsUnicode();
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => x.Codigo).IsUnique();
        b.HasIndex(x => new
        {
            x.IsDeleted,
            x.Estado,
            x.Prioridad,
        });
        b.HasIndex(x => x.AreaId);
        b.HasIndex(x => x.SolicitanteId);
        b.HasIndex(x => x.ResponsableId);
        b.HasOne(x => x.Area)
            .WithMany(x => x.Solicitudes)
            .HasForeignKey(x => x.AreaId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Solicitante)
            .WithMany()
            .HasForeignKey(x => x.SolicitanteId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Responsable)
            .WithMany()
            .HasForeignKey(x => x.ResponsableId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

internal sealed class HistorialSolicitudConfiguration : IEntityTypeConfiguration<HistorialSolicitud>
{
    public void Configure(EntityTypeBuilder<HistorialSolicitud> b)
    {
        b.ToTable("HistorialSolicitudes");
        b.HasKey(x => x.Id);
        b.Property(x => x.Comentario).HasMaxLength(1000).IsRequired().IsUnicode();
        b.HasIndex(x => new { x.SolicitudId, x.CreatedAt });
        b.HasOne(x => x.Solicitud)
            .WithMany(x => x.Historial)
            .HasForeignKey(x => x.SolicitudId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasQueryFilter(x => !x.Solicitud.IsDeleted);
    }
}

internal sealed class ComentarioSolicitudConfiguration
    : IEntityTypeConfiguration<ComentarioSolicitud>
{
    public void Configure(EntityTypeBuilder<ComentarioSolicitud> b)
    {
        b.ToTable("ComentariosSolicitud");
        b.HasKey(x => x.Id);
        b.Property(x => x.Contenido).HasMaxLength(2000).IsRequired().IsUnicode();
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => new { x.SolicitudId, x.CreatedAt });
        b.HasOne(x => x.Solicitud)
            .WithMany(x => x.Comentarios)
            .HasForeignKey(x => x.SolicitudId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasQueryFilter(x => !x.IsDeleted && !x.Solicitud.IsDeleted);
    }
}

internal sealed class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
{
    public void Configure(EntityTypeBuilder<Notificacion> b)
    {
        b.ToTable("Notificaciones");
        b.HasKey(x => x.Id);
        b.Property(x => x.Titulo).HasMaxLength(160).IsRequired().IsUnicode();
        b.Property(x => x.Mensaje).HasMaxLength(500).IsRequired().IsUnicode();
        b.HasIndex(x => new
        {
            x.UsuarioId,
            x.Leida,
            x.CreatedAt,
        });
        b.HasOne(x => x.Solicitud)
            .WithMany(x => x.Notificaciones)
            .HasForeignKey(x => x.SolicitudId)
            .OnDelete(DeleteBehavior.Restrict);
        b.HasQueryFilter(x => !x.Solicitud.IsDeleted);
    }
}
