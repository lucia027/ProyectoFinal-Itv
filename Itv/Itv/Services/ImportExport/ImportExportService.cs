using CSharpFunctionalExtensions;
using Itv.Errors.Common;
using Itv.Models;
using Itv.Storage.Common;

namespace Itv.Services.ImportExport;

/// <summary>
/// Servicio que gestiona los storages.
/// </summary>
/// <param name="storage"></param>
public class ImportExportService(IStorage<Cita> storage) : IImportExportService {
    
    /// <inheritdoc cref="IImportExportService.ExportarDatos" />
    public Result<int, DomainError> ExportarDatos(IEnumerable<Cita> items, string path) {
        var lista = items.ToList();
        return storage.Salvar(lista, path).Map(_ => lista.Count);
    }
    
    /// <inheritdoc cref="IImportExportService.ImportarDatos" />
    public Result<IEnumerable<Cita>, DomainError> ImportarDatos(string path) {
        return storage.Cargar(path);
    }

    /// <inheritdoc cref="IImportExportService.ExportarDatosSistema" />
    public Result<int, DomainError> ExportarDatosSistema(IEnumerable<Cita> items) {
        return ExportarDatos(items, string.Empty);
    }

    /// <inheritdoc cref="IImportExportService.ImportarDatosSistema" />
    public Result<IEnumerable<Cita>, DomainError> ImportarDatosSistema(string path) {
        return ImportarDatos(path);
    }
}