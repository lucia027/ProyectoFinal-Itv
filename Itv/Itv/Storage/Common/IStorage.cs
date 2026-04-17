using CSharpFunctionalExtensions;
using Itv.Errors.Common;

namespace Itv.Storage.Common;

public interface IStorage<T> {
    /// <summary>
    /// Carga los datos del archivo con la ruta especificada.
    /// </summary>
    /// <param name="path">Ruta del archivo de datos.</param>
    /// <returns>Enumerable de los datos cargados.</returns>
    Result<IEnumerable<T>, DomainError> Cargar(string path);
    
    /// <summary>
    /// Salva los datos en un archivo con la ruta especificada.
    /// </summary>
    /// <param name="items">Datos a salvar.</param>
    /// <param name="path">Ruta donde estara el archivo.</param>
    Result<bool, DomainError> Salvar(IEnumerable<T> items, string path);
}