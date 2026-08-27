using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SB.BACKEND.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSupportPlatform : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(name: "SolicitudCodeSequence");

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table =>
                {
                    return new
                    {
                        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        Nombre = table.Column<string>(
                            type: "nvarchar(120)",
                            maxLength: 120,
                            nullable: false
                        ),
                        NombreNormalizado = table.Column<string>(
                            type: "nvarchar(120)",
                            maxLength: 120,
                            nullable: false
                        ),
                        Descripcion = table.Column<string>(
                            type: "nvarchar(500)",
                            maxLength: 500,
                            nullable: true
                        ),
                        CreatedAt = table.Column<DateTimeOffset>(
                            type: "datetimeoffset",
                            nullable: false
                        ),
                        UpdatedAt = table.Column<DateTimeOffset>(
                            type: "datetimeoffset",
                            nullable: true
                        ),
                        IsActive = table.Column<bool>(type: "bit", nullable: false),
                        IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                        DeletedAt = table.Column<DateTimeOffset>(
                            type: "datetimeoffset",
                            nullable: true
                        ),
                        CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                        UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                        DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                        RowVersion = table.Column<byte[]>(
                            type: "rowversion",
                            rowVersion: true,
                            nullable: false
                        ),
                    };
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "SolicitudesSoporte",
                columns: table =>
                {
                    return new
                    {
                        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        Codigo = table.Column<string>(
                            type: "varchar(30)",
                            unicode: false,
                            maxLength: 30,
                            nullable: false
                        ),
                        Titulo = table.Column<string>(
                            type: "nvarchar(200)",
                            maxLength: 200,
                            nullable: false
                        ),
                        Descripcion = table.Column<string>(
                            type: "nvarchar(4000)",
                            maxLength: 4000,
                            nullable: false
                        ),
                        Tipo = table.Column<int>(type: "int", nullable: false),
                        Prioridad = table.Column<int>(type: "int", nullable: false),
                        Estado = table.Column<int>(type: "int", nullable: false),
                        AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        SolicitanteId = table.Column<Guid>(
                            type: "uniqueidentifier",
                            nullable: false
                        ),
                        ResponsableId = table.Column<Guid>(
                            type: "uniqueidentifier",
                            nullable: true
                        ),
                        ReferenciaEvidencia = table.Column<string>(
                            type: "nvarchar(1000)",
                            maxLength: 1000,
                            nullable: true
                        ),
                        FechaCompromiso = table.Column<DateTimeOffset>(
                            type: "datetimeoffset",
                            nullable: true
                        ),
                        FechaAsignacion = table.Column<DateTimeOffset>(
                            type: "datetimeoffset",
                            nullable: true
                        ),
                        FechaInicioAtencion = table.Column<DateTimeOffset>(
                            type: "datetimeoffset",
                            nullable: true
                        ),
                        FechaResolucion = table.Column<DateTimeOffset>(
                            type: "datetimeoffset",
                            nullable: true
                        ),
                        FechaCierre = table.Column<DateTimeOffset>(
                            type: "datetimeoffset",
                            nullable: true
                        ),
                        ComentarioResolucion = table.Column<string>(
                            type: "nvarchar(2000)",
                            maxLength: 2000,
                            nullable: true
                        ),
                        CreatedAt = table.Column<DateTimeOffset>(
                            type: "datetimeoffset",
                            nullable: false
                        ),
                        UpdatedAt = table.Column<DateTimeOffset>(
                            type: "datetimeoffset",
                            nullable: true
                        ),
                        IsActive = table.Column<bool>(type: "bit", nullable: false),
                        IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                        DeletedAt = table.Column<DateTimeOffset>(
                            type: "datetimeoffset",
                            nullable: true
                        ),
                        CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                        UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                        DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                        RowVersion = table.Column<byte[]>(
                            type: "rowversion",
                            rowVersion: true,
                            nullable: false
                        ),
                    };
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesSoporte", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesSoporte_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_SolicitudesSoporte_Usuarios_ResponsableId",
                        column: x => x.ResponsableId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_SolicitudesSoporte_Usuarios_SolicitanteId",
                        column: x => x.SolicitanteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ComentariosSolicitud",
                columns: table =>
                {
                    return new
                    {
                        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        SolicitudId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        Contenido = table.Column<string>(
                            type: "nvarchar(2000)",
                            maxLength: 2000,
                            nullable: false
                        ),
                        EsInterno = table.Column<bool>(type: "bit", nullable: false),
                        CreatedAt = table.Column<DateTimeOffset>(
                            type: "datetimeoffset",
                            nullable: false
                        ),
                        UpdatedAt = table.Column<DateTimeOffset>(
                            type: "datetimeoffset",
                            nullable: true
                        ),
                        IsActive = table.Column<bool>(type: "bit", nullable: false),
                        IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                        DeletedAt = table.Column<DateTimeOffset>(
                            type: "datetimeoffset",
                            nullable: true
                        ),
                        CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                        UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                        DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                        RowVersion = table.Column<byte[]>(
                            type: "rowversion",
                            rowVersion: true,
                            nullable: false
                        ),
                    };
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComentariosSolicitud", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComentariosSolicitud_SolicitudesSoporte_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "SolicitudesSoporte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "HistorialSolicitudes",
                columns: table =>
                {
                    return new
                    {
                        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        SolicitudId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        EstadoAnterior = table.Column<int>(type: "int", nullable: false),
                        EstadoNuevo = table.Column<int>(type: "int", nullable: false),
                        Comentario = table.Column<string>(
                            type: "nvarchar(1000)",
                            maxLength: 1000,
                            nullable: false
                        ),
                        UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        CreatedAt = table.Column<DateTimeOffset>(
                            type: "datetimeoffset",
                            nullable: false
                        ),
                        UpdatedAt = table.Column<DateTimeOffset>(
                            type: "datetimeoffset",
                            nullable: true
                        ),
                        IsActive = table.Column<bool>(type: "bit", nullable: false),
                    };
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialSolicitudes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialSolicitudes_SolicitudesSoporte_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "SolicitudesSoporte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table =>
                {
                    return new
                    {
                        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        SolicitudId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        Tipo = table.Column<int>(type: "int", nullable: false),
                        Titulo = table.Column<string>(
                            type: "nvarchar(160)",
                            maxLength: 160,
                            nullable: false
                        ),
                        Mensaje = table.Column<string>(
                            type: "nvarchar(500)",
                            maxLength: 500,
                            nullable: false
                        ),
                        Leida = table.Column<bool>(type: "bit", nullable: false),
                        FechaLectura = table.Column<DateTimeOffset>(
                            type: "datetimeoffset",
                            nullable: true
                        ),
                        CreatedAt = table.Column<DateTimeOffset>(
                            type: "datetimeoffset",
                            nullable: false
                        ),
                        UpdatedAt = table.Column<DateTimeOffset>(
                            type: "datetimeoffset",
                            nullable: true
                        ),
                        IsActive = table.Column<bool>(type: "bit", nullable: false),
                    };
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificaciones_SolicitudesSoporte_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "SolicitudesSoporte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Areas_IsDeleted_IsActive",
                table: "Areas",
                columns: ["IsDeleted", "IsActive"]
            );

            migrationBuilder.CreateIndex(
                name: "IX_Areas_NombreNormalizado",
                table: "Areas",
                column: "NombreNormalizado",
                unique: true,
                filter: "[IsDeleted] = 0"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ComentariosSolicitud_SolicitudId_CreatedAt",
                table: "ComentariosSolicitud",
                columns: ["SolicitudId", "CreatedAt"]
            );

            migrationBuilder.CreateIndex(
                name: "IX_HistorialSolicitudes_SolicitudId_CreatedAt",
                table: "HistorialSolicitudes",
                columns: ["SolicitudId", "CreatedAt"]
            );

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_SolicitudId",
                table: "Notificaciones",
                column: "SolicitudId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioId_Leida_CreatedAt",
                table: "Notificaciones",
                columns: ["UsuarioId", "Leida", "CreatedAt"]
            );

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesSoporte_AreaId",
                table: "SolicitudesSoporte",
                column: "AreaId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesSoporte_Codigo",
                table: "SolicitudesSoporte",
                column: "Codigo",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesSoporte_IsDeleted_Estado_Prioridad",
                table: "SolicitudesSoporte",
                columns: ["IsDeleted", "Estado", "Prioridad"]
            );

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesSoporte_ResponsableId",
                table: "SolicitudesSoporte",
                column: "ResponsableId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesSoporte_SolicitanteId",
                table: "SolicitudesSoporte",
                column: "SolicitanteId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ComentariosSolicitud");

            migrationBuilder.DropTable(name: "HistorialSolicitudes");

            migrationBuilder.DropTable(name: "Notificaciones");

            migrationBuilder.DropTable(name: "SolicitudesSoporte");

            migrationBuilder.DropTable(name: "Areas");

            migrationBuilder.DropSequence(name: "SolicitudCodeSequence");
        }
    }
}
