using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using Itv.Enums;
using Itv.Errors;
using Itv.Errors.Common;
using Itv.Models;
using Itv.Validator.Common;
using Serilog;

namespace Itv.Validator;

public class CitaValidator : IValidator<Cita> {
    private readonly ILogger _logger = Log.ForContext<CitaValidator>();
    
    /// <inheritdoc cref="IValidator.Validate" />
    public Result<Cita, DomainError> Validate(Cita entity) {
        
        _logger.Debug("Intentando validar la entidad: {Entity}", entity);
        var errores = new List<string>();

        if (!entity.Matricula.IsValidMatricula()) {
            errores.Add("La matricula proporcionada no cumple el formato");
        }
        if (!entity.Marca.IsValidMarca()) {
            errores.Add("La marca es nula o esta vacia");
        }
        if (!entity.Modelo.IsValidModelo()) {
            errores.Add("El modelo es nulo o esta vacio");
        }
        if (!entity.Cilindrada.IsValidCilindrada()) {
            errores.Add("La cilindrada no puede ser negativa");
        }
        if (!entity.Motor.IsValidMotor()) {
            errores.Add("El tipo de motor no es valido");
        }
        if (!entity.DniDueño.IsValidDniDueño()) {
            errores.Add("El dni del dueño no cumple el formato");
        }
        if (!entity.FechaMatriculacion.IsValidFechaMatriculacion()) {
            errores.Add("La fecha de matriculacion no puede estar en futuro.");
        }
        if (!entity.FechaInspeccion.IsValidFechaInpeccion()) {
            errores.Add("La fecha de inspeccion no puede ser superior a 30 dias.");
        }

        if (errores.Any()) {
            _logger.Warning("No se ha podido validar la entidad: {Entity}",
                entity);
            return Result.Failure<Cita, DomainError>(CitaErrors.Validation(errores));
        }
        return Result.Success<Cita, DomainError>(entity);
    }
}

public static class ValidadorCitaExtension {
    public static bool IsValidMatricula(this string matricula) {
        var regexMatricula = @"^[0-9]{4}[BCDFGHJKLMNPRSTVWXYZ]{3}$";
        return Regex.IsMatch(matricula, regexMatricula);
    }
    public static bool IsValidMarca(this string marca) {
        return !string.IsNullOrEmpty(marca);
    }
    public static bool IsValidModelo(this string modelo) {
        return !string.IsNullOrEmpty(modelo);
    }
    public static bool IsValidCilindrada(this int cilindrada) {
        return cilindrada > 0;
    }
    public static bool IsValidMotor(this Motor motor) {
        return Enum.IsDefined(typeof(Motor), motor);
    }
    public static bool IsValidDniDueño(this string dniDueño) {
        var regexDni = @"^[0-9]{8}[TRWAGMYFPDXBNJZSQVHLCKE]$";
        return Regex.IsMatch(dniDueño, regexDni) && ComprobarDniValido(dniDueño);
    }
    public static bool IsValidFechaMatriculacion(this DateTime fechaMatriculacion) {
        return fechaMatriculacion < DateTime.Today;
    }
    public static bool IsValidFechaInpeccion(this DateTime fechaInspeccion) {
        return fechaInspeccion < DateTime.Today.AddDays(30);
    }
    
    /// <summary>
    /// Válida que el dni proporcionado cumpla el cálculo de numeros y letra.
    /// </summary>
    /// <param name="dni">DNi proporcionado.</param>
    /// <returns>Verdadero en caso correcto y false en contrario.</returns>
    private static bool ComprobarDniValido(string dni) {
        char[] letrasPermitidas = ['T', 'R', 'W', 'A', 'G', 'M', 'Y', 'F', 'P', 'D', 'X', 'B', 'N', 'J', 'Z', 'S', 'Q', 'V', 'H', 'L', 'C', 'K', 'E'];

        try {
            string numeros = dni.Substring(0, 8);
            int numerosDni = int.Parse(numeros);
            
            char letraProporcionada = char.ToUpper(dni[8]);

            int indiceLetra = numerosDni % 23;

            char letraCorrecta = letrasPermitidas[indiceLetra];

            return letraProporcionada == letraCorrecta;
        } catch (Exception) {
            return false;
        }
    }
}