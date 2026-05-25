using CSharpFunctionalExtensions;
using Itv.Errors.Common;

namespace Itv.Validator.Common;

/// <summary>
/// Contrato generico para los validadores.
/// </summary>
/// <typeparam name="T">Tipo a validar.</typeparam>
public interface IValidator<T> {

    Result<T, DomainError> Validate(T entity);
}