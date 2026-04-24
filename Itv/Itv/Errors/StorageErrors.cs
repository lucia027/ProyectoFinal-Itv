using Itv.Errors.Common;

namespace Itv.Errors;

/// <summary>
///  Clase especifica para los errores de vehiculos en el storage.
/// </summary>
public abstract record StorageError(string Message) : DomainError(Message) {
    public sealed record FileNotFound(string FilePath)
        : StorageError($"No se ha encontrado el archivo en la ruta: {FilePath}");
    public sealed record InvalidFormat(string Details)
        : StorageError($"El archivo con el formato: {Details} no es compatible.");
    public sealed record WriteError(string Details)
        : StorageError($"Error al escribir en el almacenamiento: {Details}");
    public sealed record ReadError(string Details)
        : StorageError($"Error al leer en el almacenamiento: {Details}");
    public sealed record AccessError(string Details)
        : StorageError($"Error de acceso al almacenamiento: {Details}");
}

/// <summary>
///  Clase estatica para crear los errores del storage de vehiculos.
/// </summary>
public static class StorageErrors {
    public static DomainError FileNotFound(string FilePaths) {
        return new StorageError.FileNotFound(FilePaths);
    }
    public static DomainError InvalidFormat(string Details) {
        return new StorageError.InvalidFormat(Details);
    }
    public static DomainError WriteError(string Details) {
        return new StorageError.WriteError(Details);
    }
    public static DomainError ReadError(string Details) {
        return new StorageError.ReadError(Details);
    }
    public static DomainError AccessError(string Details) {
        return new StorageError.AccessError(Details);
    }
}