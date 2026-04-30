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
        _logger.Debug("Intentando validar la entidad: {Entity}",
            entity);
        
        var regexMatricula = @"^[0-9]{4}[BCDFGHJKLMNPRSTVWXYZ]{3}$";
        var regexDni = @"^[0-9]{8}[TRWAGMYFPDXBNJZSQVHLCKE]$";
        
        var errores = new List<string>();

        if (!Regex.IsMatch(entity.Matricula, regexMatricula)) {
            errores.Add("La matricula proporcionada no cumple el formato");
        }
        if (string.IsNullOrEmpty(entity.Marca)) {
            errores.Add("La marca es nula o esta vacia");
        }
        if (string.IsNullOrEmpty(entity.Modelo)) {
            errores.Add("El modelo es nulo o esta vacio");
        }
        if (entity.Cilindrada < 0) {
            errores.Add("La cilindrada no puede ser negativa");
        }
        if (!Enum.IsDefined(typeof(Motor), entity.Motor)) {
            errores.Add("El tipo de motor no es valido");
        }
        if (!Regex.IsMatch(entity.DniDueño, regexDni) || !ComprobarDniValido(entity.DniDueño)) {
            errores.Add("El dni del dueño no cumple el formato");
        }
        if (entity.FechaMatriculacion > DateTime.Today) {
            errores.Add("La fecha de matriculacion no puede estar en futuro.");
        }
        if (entity.FechaInspeccion > DateTime.Today.AddDays(30)) {
            errores.Add("La fecha de inspeccion no puede ser superior a 30 dias.");
        }

        if (errores.Any()) {
            _logger.Warning("No se ha podido validar la entidad: {Entity}",
                entity);
            return Result.Failure<Cita, DomainError>(CitaErrors.Validation(errores));
        }
        return Result.Success<Cita, DomainError>(entity);
    }

    /// <summary>
    /// Válida que el dni proporcionado cumpla el cálculo de numeros y letra.
    /// </summary>
    /// <param name="dni">DNi proporcionado.</param>
    /// <returns>Verdadero en caso correcto y false en contrario.</returns>
    private bool ComprobarDniValido(string dni) {
        _logger.Debug("Intentando comprobar el dni: {Dni}.",
            dni);
        char[] letrasPermitidas = ['T', 'R', 'W', 'A', 'G', 'M', 'Y', 'F', 'P', 'D', 'X', 'B', 'N', 'J', 'Z', 'S', 'Q', 'V', 'H', 'L', 'C', 'K', 'E'];

        try {
            string numeros = dni.Substring(0, 8);
            int numerosDni = int.Parse(numeros);
            
            char letraProporcionada = char.ToUpper(dni[8]);

            int indiceLetra = numerosDni % 23;

            char letraCorrecta = letrasPermitidas[indiceLetra];

            return letraProporcionada == letraCorrecta;
        } catch (Exception) {
            _logger.Error("El dni: {Dni} no ha pasado el proceso de validacion.",
                dni);
            return false;
        }
    }
}