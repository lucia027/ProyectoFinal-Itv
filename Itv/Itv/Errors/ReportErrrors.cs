using Itv.Errors.Common;

namespace Itv.Errors;

/// <summary>
///  Clase especifica para crear los errores de los informes.
/// </summary>
public abstract record ReportError(string Message) : DomainError(Message) {
    public sealed record GenerationError(string Errores)
        : DomainError($"Han surgido errores en el intento de generar el informe, errores: {Errores}");
    
    public sealed record SaveError(string Errores)
        : DomainError($"Han surgido errores en el intento de guardar el informe, errores: {Errores}");
}

/// <summary>
///  Clase estatica para crear los errores de los informes.
/// </summary>
public static class ReportErrors {
    public static DomainError GenerationErrors(string errores) {
        return new ReportError.GenerationError(errores);
    }
    
    public static DomainError SaveError(string errores) {
        return new ReportError.SaveError(errores);
    }
}