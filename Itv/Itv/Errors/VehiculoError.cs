namespace Itv.Errors;

/// <summary>
///  Clase especifica para los errores de vehiculos.
/// </summary>
public abstract record VehiculoError(string Message) : DomainError(Message) {
    public sealed record Validation(IEnumerable<string> Errores)
        : Errors.DomainError($"Han surgido errores en el intento de validación de la nueva entidad:{Environment.NewLine}• {string.Join($"{Environment.NewLine}• ", Errores)}");
}

/// <summary>
///  Clase estatica para crear los errores de los vehiculos.
/// </summary>
public static class VehiculoErrors {
    public static DomainError Validation(IEnumerable<string> errores) {
        return new VehiculoError.Validation(errores);
    }
}