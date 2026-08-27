using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SB.BACKEND.Application.Common;
using SB.BACKEND.Application.GovernmentEntities;
using SB.BACKEND.Infrastructure.Persistence;
using SB.BACKEND.Infrastructure.Persistence.Repositories;
using SB.BACKEND.Services.GovernmentEntities;
using Xunit;

namespace SB.BACKEND.Tests;

public sealed class GovernmentEntityTests
{
    private static (
        SecurityDbContext Db,
        GovernmentEntityRepository Repo,
        GovernmentEntityService Service
    ) CreateSystem()
    {
        var current = new TestCurrentUser();
        var options = new DbContextOptionsBuilder<SecurityDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new SecurityDbContext(options, current);
        var repo = new GovernmentEntityRepository(db);
        return (db, repo, new GovernmentEntityService(repo, db, current));
    }

    [Fact]
    public async Task Seed_inserts_181_rows_is_idempotent_and_preserves_unicode()
    {
        var (db, repo, _) = CreateSystem();
        var seeder = new GovernmentEntitySeeder(
            repo,
            db,
            NullLogger<GovernmentEntitySeeder>.Instance
        );
        var first = await seeder.SeedAsync();
        var second = await seeder.SeedAsync();
        Assert.Equal(181, first.Read);
        Assert.Equal(181, first.Inserted);
        Assert.Equal(0, first.Rejected);
        Assert.Equal(0, second.Inserted);
        Assert.Equal(181, second.Skipped);
        Assert.Equal(181, await db.EntidadesGubernamentales.CountAsync());
        Assert.Contains(
            await db.EntidadesGubernamentales.ToListAsync(),
            x =>
            {
                return x.Nombre.Contains('“') && x.Nombre.Contains("Rodríguez");
            }
        );
    }

    [Fact]
    public async Task Create_trims_values_sets_audit_and_prevents_duplicates()
    {
        var (_, _, service) = CreateSystem();
        var request = Request("  Entidad Única  ");
        var created = await service.CreateAsync(request, default);
        Assert.Equal("Entidad Única", created.Nombre);
        Assert.True(created.Activo);
        Assert.False(created.IsDeleted);
        Assert.Equal(TimeSpan.Zero, created.CreatedAt.Offset);
        Assert.NotNull(created.CreatedBy);
        await Assert.ThrowsAsync<ConflictException>(() =>
        {
            return service.CreateAsync(Request("entidad única"), default);
        });
    }

    [Fact]
    public async Task Get_and_paged_search_filters_return_expected_results()
    {
        var (_, _, service) = CreateSystem();
        var first = await service.CreateAsync(
            Request("Ministerio Ámbar", "Ministerio", "Poder Ejecutivo", "Cultura"),
            default
        );
        await service.CreateAsync(
            Request("Instituto Azul", "Instituto", "Poder Ejecutivo", "Educación"),
            default
        );
        Assert.Equal(first.Id, (await service.GetByIdAsync(first.Id, default)).Id);
        var page = await service.GetAllAsync(
            new()
            {
                Search = "Ámbar",
                Categoria = "Ministerio",
                PoderDelEstado = "Poder Ejecutivo",
                Sector = "Cultura",
                Activo = true,
                PageSize = 1,
            },
            default
        );
        Assert.Single(page.Items);
        Assert.Equal(1, page.TotalRecords);
        Assert.Equal(1, page.TotalPages);
    }

    [Fact]
    public async Task Update_and_status_set_updated_audit_and_validate_state()
    {
        var (_, _, service) = CreateSystem();
        var item = await service.CreateAsync(Request("Entidad A"), default);
        var updated = await service.UpdateAsync(
            item.Id,
            new()
            {
                Nombre = "Entidad B",
                Categoria = "Categoría",
                PoderDelEstado = "Poder Ejecutivo",
                Sector = "Sector",
                RowVersion = item.RowVersion,
            },
            default
        );
        Assert.Equal("Entidad B", updated.Nombre);
        Assert.NotNull(updated.UpdatedAt);
        Assert.NotNull(updated.UpdatedBy);
        await service.ChangeStatusAsync(
            item.Id,
            new() { Activo = false, RowVersion = updated.RowVersion },
            default
        );
        Assert.False((await service.GetByIdAsync(item.Id, default)).Activo);
        await Assert.ThrowsAsync<ValidationException>(() =>
        {
            return service.ChangeStatusAsync(
                item.Id,
                new() { Activo = false, RowVersion = updated.RowVersion },
                default
            );
        });
    }

    [Fact]
    public async Task Soft_delete_excludes_get_and_list_and_sets_deleted_audit()
    {
        var (_, repo, service) = CreateSystem();
        var item = await service.CreateAsync(Request("Entidad Eliminable"), default);
        await service.DeleteAsync(item.Id, item.RowVersion, default);
        Assert.Equal(0, (await service.GetAllAsync(new(), default)).TotalRecords);
        await Assert.ThrowsAsync<NotFoundException>(() =>
        {
            return service.GetByIdAsync(item.Id, default);
        });
        var deleted = await repo.GetByIdAsync(item.Id, true, default);
        Assert.NotNull(deleted);
        Assert.True(deleted.IsDeleted);
        Assert.False(deleted.IsActive);
        Assert.NotNull(deleted.DeletedAt);
        Assert.NotNull(deleted.DeletedBy);
    }

    [Fact]
    public async Task Restore_reactivates_and_duplicate_name_causes_conflict()
    {
        var (_, repo, service) = CreateSystem();
        var deleted = await service.CreateAsync(Request("Entidad Restaurable"), default);
        await service.DeleteAsync(deleted.Id, deleted.RowVersion, default);
        var stored = await repo.GetByIdAsync(deleted.Id, true, default);
        await service.RestoreAsync(deleted.Id, Convert.ToBase64String(stored!.RowVersion), default);
        Assert.True((await service.GetByIdAsync(deleted.Id, default)).Activo);
        var other = await service.CreateAsync(Request("Nombre Duplicable"), default);
        await service.DeleteAsync(other.Id, other.RowVersion, default);
        await service.CreateAsync(Request("nombre duplicable"), default);
        stored = await repo.GetByIdAsync(other.Id, true, default);
        await Assert.ThrowsAsync<ConflictException>(() =>
        {
            return service.RestoreAsync(
                other.Id,
                Convert.ToBase64String(stored!.RowVersion),
                default
            );
        });
    }

    [Fact]
    public async Task Validation_rejects_empty_fields_and_invalid_identifiers_or_versions()
    {
        var (_, _, service) = CreateSystem();
        await Assert.ThrowsAsync<ValidationException>(() =>
        {
            return service.CreateAsync(Request(""), default);
        });
        await Assert.ThrowsAsync<ValidationException>(() =>
        {
            return service.GetByIdAsync(Guid.Empty, default);
        });
        var item = await service.CreateAsync(Request("Entidad Versionada"), default);
        await Assert.ThrowsAsync<ValidationException>(() =>
        {
            return service.DeleteAsync(item.Id, "not-base64", default);
        });
    }

    [Fact]
    public async Task Stale_row_version_returns_conflict()
    {
        var (_, _, service) = CreateSystem();
        var item = await service.CreateAsync(Request("Entidad Concurrente"), default);
        var request = new UpdateGovernmentEntityRequest
        {
            Nombre = "Entidad Concurrente Editada",
            Categoria = "Categoría",
            PoderDelEstado = "Poder Ejecutivo",
            Sector = "Sector",
            RowVersion = Convert.ToBase64String([99]),
        };
        await Assert.ThrowsAsync<ConflictException>(() =>
        {
            return service.UpdateAsync(item.Id, request, default);
        });
    }

    private static CreateGovernmentEntityRequest Request(
        string name,
        string category = "Categoría",
        string power = "Poder Ejecutivo",
        string sector = "Sector"
    )
    {
        return new()
        {
            Nombre = name,
            Categoria = category,
            PoderDelEstado = power,
            Sector = sector,
        };
    }

    private sealed class TestCurrentUser : ICurrentUserService
    {
        public Guid? UserId { get; } = Guid.NewGuid();
        public IReadOnlyCollection<string> Roles { get; } = ["Admin"];

        public bool IsInRole(params string[] roles)
        {
            return roles.Contains("Admin");
        }
    }
}
