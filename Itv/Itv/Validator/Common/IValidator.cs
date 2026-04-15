using System.Net.Http.Headers;
using CSharpFunctionalExtensions;
using Itv.Errors;
using Itv.Errors.Common;

namespace Itv.Validator.Common;

/// <summary>
/// Contrato generico para los validadores.
/// </summary>
/// <typeparam name="T">Tipo a validar.</typeparam>
public interface IValidator<T> {
    /// <summary>
    /// Comprueba si el dato introducido cumple los requisitos.
    /// </summary>
    /// <param name="entity">Entidad a validar</param>
    /// <returns>Coleccion de los errores que han surgido en la validación.</returns>
    Result<T, DomainError> Validate(T entity);
}