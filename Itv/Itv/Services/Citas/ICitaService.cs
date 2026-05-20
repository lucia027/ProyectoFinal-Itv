using CSharpFunctionalExtensions;
using Itv.Enums;
using Itv.Errors.Common;
using Itv.Models;

namespace Itv.Services.Citas;

public interface ICitaService {
    
    /// <summary>
    /// Obtiene un Enumerable de citas paginadas y con la posibilidad de filtrar por campo de busqueda.
    /// </summary>
    /// <param name="pagina">Número de página que mostrar.</param>
    /// <param name="tamPagina">Número de elementos a mostrar por páginas.</param>
    /// <param name="isDeleteInclude">True si incluye los registros eliminados, false si no.</param>
    /// <param name="campoBusqueda">Campo de busqueda en los registros.</param>
    /// <returns>Enumerable de cita.</returns>
    IEnumerable<Cita> GetAll(int pagina = 1, int tamPagina = 5, bool isDeleteInclude = true, string campoBusqueda = "");
    
    /// <summary>
    /// Obtiene una cita segun su id.
    /// </summary>
    /// <param name="id">Id de la cita a buscar.</param>
    /// <returns>Result con la cita encontrada o un error.</returns>
    Result<Cita, DomainError> GetById(int id);

    /// <summary>
    /// Obtiene un listado de las citas que cumplen el filtro del rango de su fecha de inspeccion.
    /// </summary>
    /// <param name="inicio">Inicio del rango de fecha.</param>
    /// <param name="fin">Fin del rango de fecha.</param>
    /// <param name="isDeleteInclude">True si incluye los registros eliminados, false si no.</param>
    /// <returns>Result con las citas encontradas o un error.</returns>
    Result<IEnumerable<Cita>, DomainError> GetByDateInspeccion(DateTime inicio, DateTime? fin, bool isDeleteInclude = true);

    /// <summary>
    /// Obtiene un listado de las citas que son del tipo de motor especificado.
    /// </summary>
    /// <param name="motor">Tipo de motor para filtrar.</param>
    /// <param name="isDeleteInclude">True si incluye los registros eliminados, false si no</param>
    /// <returns>Result con las citas encontradas o un error.</returns>
    Result<IEnumerable<Cita>, DomainError> GetByTipoMotor(Motor motor, bool isDeleteInclude = true);

    /// <summary>
    /// Registra una cita nueva en el sistema.
    /// </summary>
    /// <param name="cita">Cita a registrar-</param>
    /// <returns>Result con la cita creada o un error.</returns>
    Result<Cita, DomainError> Create(Cita cita);

    /// <summary>
    /// Acuatiza una cita ya creada en el sistema.
    /// </summary>
    /// <param name="id">Id de la cita a actualizar.</param>
    /// <param name="cita">Cita actualizada.</param>
    /// <returns>Result con la cita actualizada o un error.</returns>
    Result<Cita, DomainError> Update(int id, Cita cita);

    /// <summary>
    /// Elimina de forma logica una cita ya creada en el sistema.
    /// </summary>
    /// <param name="id">Id de la cita a eliminar.</param>
    /// <returns>Result con la cita eliminada o un error.</returns>
    Result<Cita, DomainError> Delete(int id);

    /// <summary>
    /// Elimina de forma permanente una cita ya creada en el sistema.
    /// </summary>
    /// <param name="id">Id de la cita a eliminar.</param>
    /// <returns>Result con la cita eliminada o un error.</returns>
    Result<Cita, DomainError> DeleteHard(int id);

    /// <summary>
    /// Elimina permanentemente todas las citas registradas en el sistema.
    /// </summary>
    /// <returns>True si la operacion ha sido exitosa false si no.</returns>
    bool DeleteAll();
}