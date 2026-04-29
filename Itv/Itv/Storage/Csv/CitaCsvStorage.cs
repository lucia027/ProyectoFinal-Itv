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
        InitStorage();
    }

    /// <inheritdoc cref="ICitaCsvStorage.Cargar" />
    public Result<IEnumerable<Cita>, DomainError> Cargar(string path) {

        if (!Path.Exists(path)) {
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
            return Result.Failure<IEnumerable<Cita>, DomainError>(StorageErrors.InvalidFormat(e.Message));
        }    
    }

    /// <inheritdoc cref="ICitaCsvStorage.Salvar" />
    public Result<bool, DomainError> Salvar(IEnumerable<Cita> items, string path) {
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
            return Result.Failure<bool, DomainError>(StorageErrors.WriteError(e.Message));
        }        
    }
    
    /// <summary>
    /// Comprueba que exista el directorio donde almacenar el archivo de datos,
    /// en caso de que no exista lo crea.
    /// </summary>
    private void InitStorage() {
        if (Directory.Exists(Configuracion.DataFolder)) return;
        Directory.CreateDirectory(Configuracion.DataFolder);
    }
}