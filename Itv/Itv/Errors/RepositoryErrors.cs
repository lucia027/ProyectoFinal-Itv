using Itv.Errors.Common;
using Itv.Models;

namespace Itv.Errors;

public abstract record RepositoryError(string Message) : DomainError(Message) {
    public sealed record MatriculaNotFound(Vehiculo vehiculo)
        : RepositoryError($"No se puede encontrar el vehiculo con la matricula: {vehiculo.Matricula}");

    public sealed record IdNotFound(int id)
        : RepositoryError($"No se puede encontrar el vehiculo con el id: {id}");

    public sealed record InvalidMatricula(int id)
        : RepositoryError($"No se actualizar la matricula del vehiculo con el id: {id}, por que ya existe.");
}

public static class RepositoryErrors {
    public static DomainError MatriculaNotFound(Vehiculo vehiculo) {
        return new RepositoryError.MatriculaNotFound(vehiculo);
    }
    public static DomainError IdNotFound(int id) {
        return new RepositoryError.IdNotFound(id);
    }
    
    public static DomainError InvalidMatricula(int id) {
        return new RepositoryError.InvalidMatricula(id);
    }
}