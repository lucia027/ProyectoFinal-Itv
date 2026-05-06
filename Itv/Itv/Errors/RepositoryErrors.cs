using Itv.Errors.Common;
using Itv.Models;

namespace Itv.Errors;

/// <summary>
/// Clase específica para lso errores de citas en los repositorios.
/// </summary>
/// <param name="Message"></param>
public abstract record RepositoryError(string Message) : DomainError(Message) {
    public sealed record IdNotFound(int id)
        : RepositoryError($"No se puede encontrar el cita con el id: {id}");

    public sealed record InvalidMatricula(string matricula)
        : RepositoryError($"La matricula: {matricula} no es valida por que ya existe.");
    
    public sealed record DniDueñoError(Cita cita)
        : RepositoryError($"El cita no se puede crear, el dueño con el dni: {cita.DniDueño} ha superado el maximo de vehiculos.");
    
    public sealed record FechaMatriculacionError(Cita cita)
        : RepositoryError($"El cita no se puede crear, el vehiculo proporcionado ya tiene una fecha de matriculacion({cita.FechaMatriculacion}) el mismo dia.");
    
    public sealed record NotFoundCitasError()
        : RepositoryError($"No se han encotrado citas que cumplan la condicion.");

}

public static class RepositoryErrors {
    public static DomainError IdNotFound(int id) {
        return new RepositoryError.IdNotFound(id);
    }
    
    public static DomainError InvalidMatricula(string matricula) {
        return new RepositoryError.InvalidMatricula(matricula);
    }
    
    public static DomainError DniDueñoError(Cita cita) {
        return new RepositoryError.DniDueñoError(cita);
    }
    
    public static DomainError FechaMatriculacionError(Cita cita) {
        return new RepositoryError.FechaMatriculacionError(cita);
    }
    
    public static DomainError NotFoundCitasError() {
        return new RepositoryError.NotFoundCitasError();
    }
}