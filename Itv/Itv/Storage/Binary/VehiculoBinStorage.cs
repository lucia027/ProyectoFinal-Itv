using System.Text;
using CSharpFunctionalExtensions;
using Itv.Dto;
using Itv.Errors;
using Itv.Errors.Common;
using Itv.Mappers;
using Itv.Models;
using Serilog;

namespace Itv.Storage.Binary;

public class VehiculoBinStorage : IVehiculoBinStorage {

    private readonly ILogger _logger = Log.ForContext<VehiculoBinStorage>();

    public VehiculoBinStorage() {
        InitStorage();
    }

    /// <inheritdoc cref="IVehiculoBinStorage.Cargar" />
    public Result<IEnumerable<Vehiculo>, DomainError> Cargar(string path) {
        if (!File.Exists(path)) {
            _logger.Debug("Intentando salvar los datos en formato bin.");
            return Result.Failure<IEnumerable<Vehiculo>, DomainError>(StorageErrors.FileNotFound(path));
        }

        try {
            using var stream = File.OpenRead(path);
            using var reader = new BinaryReader(stream, Encoding.UTF8);

            var count = reader.ReadInt32();
            var vehiculos = new List<Vehiculo>();

            for (int i = 0; i < count; i++) {
                var v = new VehiculoDto(
                    reader.ReadInt32(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadInt32(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadString(),
                    reader.ReadBoolean()
                ).ToModel();
                vehiculos.Add(v);
            }

            return Result.Success<IEnumerable<Vehiculo>, DomainError>(vehiculos);
        } catch (Exception e) {
            return Result.Failure<IEnumerable<Vehiculo>, DomainError>(StorageErrors.ReadError(e.Message));
        }
    }

    /// <inheritdoc cref="IVehiculoBinStorage.Salvar" />
    public Result<bool, DomainError> Salvar(IEnumerable<Vehiculo> items, string path) {
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
                writer.Write(v.CreateAt);
                writer.Write(v.UpdateAt);
                writer.Write(v.IsDelete);
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
        if (Directory.Exists(Config.Configuracion.DataFolder)) {
            return;
        }
        Directory.CreateDirectory("data");
    }
}