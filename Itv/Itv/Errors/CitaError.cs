using Itv.Errors.Common;

namespace Itv.Errors;

/// <summary>
///  Clase especifica para los errores de citas.
/// </summary>
public abstract record CitaError(string Message) : DomainError(Message) {
    public sealed record Validation(IEnumerable<string> Errores)
        : DomainError($"Han surgido errores en el intento de validación de la nueva entidad:{Environment.NewLine}• {string.Join($"{Environment.NewLine}• ", Errores)}");

    public sealed record NotFoundCitasError()
        : RepositoryError($"No se han encontrado citas.");

}

/// <summary>
///  Clase estatica para crear los errores de las citas.
/// </summary>
public static class CitaErrors {
    public static DomainError Validation(IEnumerable<string> errores) {
        return new CitaError.Validation(errores);
    }
    
    public static DomainError NotFoundCitasError() {
        return new RepositoryError.NotFoundCitasError();
    }
}