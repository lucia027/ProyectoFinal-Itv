using CSharpFunctionalExtensions;
using Itv.Errors.Common;

namespace Itv.Repository.Common;

/// <summary>
/// Contrato generico Crud para el sistema.
/// </summary>
public interface ICrud_Repository<in TKey, TEntity> where TEntity : class {
    
    /// <summary>
    /// Devuelve todas las entidades del almacen.
    /// </summary>
    /// <returns>Enumerable de las entidades.</returns>
    IEnumerable<TEntity> GetAll(int pagina = 1, int tamPagina = 5, bool isDeleteInclude = true, string campoBusqueda = "");
    
    /// <summary>
    /// Devuelve la entidad cuyo Id sea igual al proporcionado.
    /// </summary>
    /// <param name="id">Id de la entidad</param>
    /// <returns>En caso de existir la entidad y en caso contrario nulo.</returns>
    Result<TEntity, DomainError> GetById(TKey id);
    
    /// <summary>
    /// Crea y guarda en el almacen una nueva entidad.
    /// </summary>
    /// <param name="entity">Entidad nueva.</param>
    /// <returns>En caso de ser correcta la entidad y en caso contrario nulo.</returns>
    Result<TEntity, DomainError> Create(TEntity entity);
    
    /// <summary>
    /// Actualiza una entidad ya existente en el almacen.
    /// </summary>
    /// <param name="id">Id de la entidad existente.</param>
    /// <param name="entity">Entidad existente actualizada.</param>
    /// <returns>En caso de ser correcta la entidad y en caso contrario nulo.</returns>
    Result<TEntity, DomainError> Update(TKey id, TEntity entity);
    
    /// <summary>
    /// ELimina la entidad del almacen.
    /// </summary>
    /// <param name="id">Id de la entidad existente.</param>
    /// <returns>En caso de ser correcta la entidad eliminada y nulo en caso contrario.</returns>
    Result<TEntity, DomainError> Delete(TKey id);
}