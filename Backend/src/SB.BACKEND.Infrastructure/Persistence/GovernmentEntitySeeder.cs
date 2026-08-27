using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using SB.BACKEND.Application.GovernmentEntities;
using SB.BACKEND.Application.Security;
using SB.BACKEND.Domain.GovernmentEntities;

namespace SB.BACKEND.Infrastructure.Persistence;

public interface IGovernmentEntitySeeder
{
    Task<SeedResult> SeedAsync(CancellationToken ct = default);
}

public sealed record SeedResult(int Read, int Inserted, int Skipped, int Rejected);

internal sealed class GovernmentEntitySeeder(
    IGovernmentEntityRepository repository,
    IUnitOfWork unitOfWork,
    ILogger<GovernmentEntitySeeder> logger
) : IGovernmentEntitySeeder
{
    public async Task<SeedResult> SeedAsync(CancellationToken ct = default)
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Data",
            "Seed",
            "ListaEntidadesGubernamentales.xlsx"
        );
        if (!File.Exists(path))
            throw new FileNotFoundException(
                "No se encontró el Excel de entidades gubernamentales.",
                path
            );
        using var book = new XLWorkbook(path);
        var sheet = book.Worksheet(1);
        var expected = new[] { "Nombre", "Categoría", "Poder del Estado", "Sector" };
        var actual = Enumerable
            .Range(1, 4)
            .Select(c =>
            {
                return sheet.Cell(1, c).GetString().Trim();
            })
            .ToArray();
        if (!actual.SequenceEqual(expected, StringComparer.OrdinalIgnoreCase))
            throw new InvalidDataException("El Excel no contiene las cuatro columnas requeridas.");
        var known = await repository.GetAllNormalizedNamesAsync(ct);
        var read = 0;
        var inserted = 0;
        var skipped = 0;
        var rejected = 0;
        foreach (var row in sheet.RowsUsed().Skip(1))
        {
            ct.ThrowIfCancellationRequested();
            var v = Enumerable
                .Range(1, 4)
                .Select(c =>
                {
                    return row.Cell(c).GetString().Trim();
                })
                .ToArray();
            if (v.All(string.IsNullOrWhiteSpace))
                continue;
            read++;
            if (v.Any(string.IsNullOrWhiteSpace))
            {
                rejected++;
                logger.LogWarning("Fila {Row} rechazada: campos incompletos.", row.RowNumber());
                continue;
            }
            var normalized = Normalize(v[0]);
            if (!known.Add(normalized))
            {
                skipped++;
                continue;
            }
            repository.Add(new EntidadGubernamental(v[0], normalized, v[1], v[2], v[3]));
            inserted++;
        }
        await unitOfWork.SaveChangesAsync(ct);
        logger.LogInformation(
            "Seed entidades: {Read} leídas, {Inserted} insertadas, {Skipped} omitidas, {Rejected} rechazadas.",
            read,
            inserted,
            skipped,
            rejected
        );
        return new(read, inserted, skipped, rejected);
    }

    private static string Normalize(string value)
    {
        return string.Join(' ', value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .ToUpperInvariant();
    }
}
