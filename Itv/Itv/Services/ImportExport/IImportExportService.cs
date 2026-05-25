using CSharpFunctionalExtensions;
using Itv.Errors.Common;
using Itv.Models;

namespace Itv.Services.ImportExport;

public interface IImportExportService {

    /// <summary>
    /// Exporta las citas registradas a un archivo de un formato específico.
    /// </summary>
    /// <param name="items">Citas registradas a exportar.</param>
    /// <param name="path">Ruta donde estara el archivo.</param>
    /// <returns>Result con el número de citas exportadas o un error</returns>
    Result<int, DomainError> ExportarDatos(IEnumerable<Cita> items, string path);
    
    /// <summary>
    /// Importa las citas registradas en un archivo de un formato específico.
    /// </summary>
    /// <param name="path">Ruta donde estara el archivo.</param>
    /// <returns>Result con un Enumerable de las citas importadas o un error</returns>
    Result<IEnumerable<Cita>, DomainError> ImportarDatos(string path);
    
    /// <summary>
    /// Exporta las citas registradas en el sistema utilizando un directorio configurado en el sistema.
    /// </summary>
    /// <param name="items">Citas registradas a exportar.</param>
    /// <returns>Result con el número de citas exportadas o un error</returns>
    Result<int, DomainError> ExportarDatosSistema(IEnumerable<Cita> items);

    /// <summary>
    /// Importa las citas encontradas en el directorio específico del sistema.
    /// </summary>
    /// <param name="path">Ruta donde estara el archivo.</param>
    /// <returns>Result con un Enumerable de las citas importadas o un error</returns>
    Result<IEnumerable<Cita>, DomainError> ImportarDatosSistema(string path);
}