using System.Text;
using CSharpFunctionalExtensions;
using Itv.Dto;
using Itv.Errors;
using Itv.Errors.Common;
using Itv.Mappers;
using Itv.Models;
using Serilog;

namespace Itv.Storage.Binary;

/// <summary>
/// Almacenamiento de los datos en binario secuencial para las citas.
/// </summary>
public class CitaBinStorage : ICitaBinStorage {

    private readonly ILogger _logger = Log.ForContext<CitaBinStorage>();

    public CitaBinStorage() {
        _logger.Debug("Se esta iniciando el almacenamiento en binario secuencial.");
        InitStorage();
    }

    /// <inheritdoc cref="ICitaBinStorage.Cargar" />
    public Result<IEnumerable<Cita>, DomainError> Cargar(string path) {
        _logger.Debug("Intentando cargar los datos en formato binario.");
        
        if (!File.Exists(path)) {
            _logger.Warning("El fichero para los datos en formato binario no existe.");
            return Result.Failure<IEnumerable<Cita>, DomainError>(StorageErrors.FileNotFound(path));
        }

        try {
            using var stream = File.OpenRead(path);
            using var reader = new BinaryReader(stream, Encoding.UTF8);

            var count = reader.ReadInt32();
            var citas = new List<Cita>();

            for (int i = 0; i < count; i++) {
                var v = new CitaDto(
                    reader.ReadInt32(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadInt32(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadString(), 
                    reader.ReadBoolean()
                ).ToModel();
                citas.Add(v);
            }

            return Result.Success<IEnumerable<Cita>, DomainError>(citas);
        } catch (Exception e) {
            _logger.Error("Error al intentar cargar los datos en formato binario, mensaje error: {e.Message}",
                e.Message);
            return Result.Failure<IEnumerable<Cita>, DomainError>(StorageErrors.ReadError(e.Message));
        }
    }

    /// <inheritdoc cref="ICitaBinStorage.Salvar" />
    public Result<bool, DomainError> Salvar(IEnumerable<Cita> items, string path) {
        _logger.Debug("Intentando salvar los datos en formato binario.");
        
        try {
            using var stream = File.Create(path);
            using var writer = new BinaryWriter(stream, Encoding.UTF8);

            var dtos = items.Select(v => v.ToDto()).ToList();
            writer.Write(dtos.Count);

            foreach (var v in dtos) {
                writer.Write(v.Id);
                writer.Write(v.Matricula);
                writer.Write(v.Marca);
                writer.Write(v.Modelo);
                writer.Write(v.Cilindrada);
                writer.Write(v.Motor);
                writer.Write(v.DniDueño);
                writer.Write(v.FechaMatriculacion);
                writer.Write(v.FechaInspeccion);
                writer.Write(v.CreateAt);
                writer.Write(v.UpdateAt);
                writer.Write(v.IsDelete);
            }
            
            return Result.Success<bool, DomainError>(true);
            
        } catch (Exception e) {
            _logger.Error("Error al salvar los datos en formato binario, mensaje de error: {e.Message}",
                e.Message);
            return Result.Failure<bool, DomainError>(StorageErrors.WriteError(e.Message));
        }
    }
    
    /// <summary>
    /// Comprueba que exista el directorio donde almacenar el archivo de datos,
    /// en caso de que no exista lo crea.
    /// </summary>
    private void InitStorage() {
        if (Directory.Exists(Config.Configuracion.DataFolder)) {
            _logger.Warning("No existe el directorio data, creando..");
            return;
        }
        Directory.CreateDirectory("data");
    }
}