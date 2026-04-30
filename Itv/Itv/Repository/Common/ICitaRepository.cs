using CSharpFunctionalExtensions;
using Itv.Errors.Common;
using Itv.Models;

namespace Itv.Repository.Common;

/// <summary>
/// <summary>
/// Contrato que contextualiza la interfaz de ICitaRepository para int y Cita y que añade un borrado fisico,
/// y un borrado total.
/// </summary>
public interface ICitaRepository : ICrud_Repository<int, Cita> {
    
    /// <summary>
    /// Elimina permanentemente una cita del almacen.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Result<Cita, DomainError> DeleteHard(int id);

    /// <summary>
    /// Elimina todas las citas del almacen;
    /// </summary>
    /// <returns>Verdadero al eliminarlos.</returns>
    bool DeleteAll();
}