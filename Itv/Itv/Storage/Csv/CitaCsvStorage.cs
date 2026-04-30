using System.Text;
using CSharpFunctionalExtensions;
using Itv.Config;
using Itv.Dto;
using Itv.Errors;
using Itv.Errors.Common;
using Itv.Mappers;
using Itv.Models;
using Serilog;

namespace Itv.Storage.Csv;

/// <summary>
/// Almacenamiento de los datos en csv para las citas.
/// </summary>
public class CitaCsvStorage : ICitaCsvStorage {
    
    private ILogger _logger = Log.Logger.ForContext<CitaCsvStorage>();

    public CitaCsvStorage() {
        _logger.Debug("Se esta iniciando el almacenamiento en csv.");
        InitStorage();
    }

    /// <inheritdoc cref="ICitaCsvStorage.Cargar" />
    public Result<IEnumerable<Cita>, DomainError> Cargar(string path) {
        _logger.Debug("Intentando cargar los datos en formato csv.");

        if (!Path.Exists(path)) {
            _logger.Warning("El fichero para los datos en formato csv no existe.");
            return Result.Failure<IEnumerable<Cita>, DomainError>(StorageErrors.FileNotFound(path));
        }

        try { 
            var citas = File.ReadLines(path, Encoding.UTF8)
                .Skip(1)
                .Select(l => l.Split(";"))
                .Select(campos => new CitaDto(
                        int.Parse(campos[0]),
                        campos[1],
                        campos[2],
                        campos[3],
                        int.Parse(campos[4]),
                        campos[5],
                        campos[6],
                        campos[7],
                        campos[8],
                        campos[9],
                        campos[10],
                        bool.TryParse(campos[11], out var d) && d
                        ).ToModel()
                );
            return Result.Success<IEnumerable<Cita>, DomainError>(citas);
        } catch (Exception e) {
            _logger.Error("Error al intentar cargar los datos en formato csv, mensaje error: {e.Message}",
                e.Message);
            return Result.Failure<IEnumerable<Cita>, DomainError>(StorageErrors.InvalidFormat(e.Message));
        }    
    }

    /// <inheritdoc cref="ICitaCsvStorage.Salvar" />
    public Result<bool, DomainError> Salvar(IEnumerable<Cita> items, string path) {
        _logger.Debug("Intentando salvar los datos en formato csv.");

        try {
            _logger.Debug("Intentando salvar los datos en formato csv.");
            using var writer = new StreamWriter(path, false, Encoding.UTF8);
            writer.WriteLine("Id;Matricula;Marca;Modelo;Cilindrada;Motor;DniDueño;FechaMatriculacion;FechaInspeccion;CreateAt;UpdateAt;IsDelete");

            foreach (var v in items) {
                var dto = v.ToDto();
                writer.WriteLine($"{dto.Id};{dto.Matricula};{dto.Marca};{dto.Modelo};{dto.Cilindrada};{dto.Motor};{dto.DniDueño};{dto.FechaMatriculacion};{dto.FechaInspeccion};{dto.CreateAt};{dto.UpdateAt};{dto.IsDelete}");
            }

            return Result.Success<bool, DomainError>(true);

        } catch (Exception e) {
            _logger.Error("Error al salvar los datos en formato csv, mensaje de error: {e.Message}",
                e.Message);
            return Result.Failure<bool, DomainError>(StorageErrors.WriteError(e.Message));
        }        
    }
    
    /// <summary>
    /// Comprueba que exista el directorio donde almacenar el archivo de datos,
    /// en caso de que no exista lo crea.
    /// </summary>
    private void InitStorage() {
        if (Directory.Exists(Configuracion.DataFolder)) {
            _logger.Warning("No existe el directorio data, creando..");
            return;
        }
        Directory.CreateDirectory(Configuracion.DataFolder);
    }
}