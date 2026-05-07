using CSharpFunctionalExtensions;
using Itv.Enums;
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
    /// Busca todas las citas que se encuetren entre dos rangos de su fecha de matriculacion.
    /// </summary>
    /// <param name="inicio">Inicio del rango.</param>
    /// <param name="fin">Fin del rango.</param>
    /// <returns>Enumerable con todas las citas que cumplan la condicion.</returns>
    Result<IEnumerable<Cita>, DomainError> GetByDateMatricula(DateTime inicio, DateTime? fin, bool isDeleteInclude = true);
    
    /// <summary>
    /// Bsuca todas las citas que tenga un tipo de motor especifico.
    /// </summary>
    /// <param name="motor">Tipo de motor.</param>
    /// <returns>Enumerable con todas las citas que cumplan la condicion.</returns>
    Result<IEnumerable<Cita>, DomainError> GetByTipoMotor(Motor motor, bool isDeleteInclude = true);
    
    /// <summary>
    /// Elimina todas las citas del almacen;
    /// </summary>
    /// <returns>Verdadero al eliminarlos.</returns>
    bool DeleteAll();
}