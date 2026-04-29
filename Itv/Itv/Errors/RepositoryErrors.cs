using Itv.Errors.Common;
using Itv.Models;

namespace Itv.Errors;

public abstract record RepositoryError(string Message) : DomainError(Message) {
    public sealed record MatriculaNotFound(Vehiculo vehiculo)
        : RepositoryError($"No se puede encontrar el vehiculo con la matricula: {vehiculo.Matricula}");

    public sealed record IdNotFound(int id)
        : RepositoryError($"No se puede encontrar el vehiculo con el id: {id}");

    public sealed record InvalidMatricula(string matricula)
        : RepositoryError($"La matricula: {matricula} no es valida por que ya existe.");
    
    public sealed record DniDueñoError(Vehiculo Vehiculo)
        : RepositoryError($"El vehiculo no se puede crear, el dueño con el dni: {Vehiculo.DniDueño} ha superado el maximo de vehiculos.");

}

public static class RepositoryErrors {
    public static DomainError MatriculaNotFound(Vehiculo vehiculo) {
        return new RepositoryError.MatriculaNotFound(vehiculo);
    }
    public static DomainError IdNotFound(int id) {
        return new RepositoryError.IdNotFound(id);
    }
    
    public static DomainError InvalidMatricula(string matricula) {
        return new RepositoryError.InvalidMatricula(matricula);
    }
    
    public static DomainError DniDueñoError(Vehiculo vehiculo) {
        return new RepositoryError.DniDueñoError(vehiculo);
    }
}