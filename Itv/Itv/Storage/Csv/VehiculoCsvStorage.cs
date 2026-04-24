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

public class VehiculoCsvStorage : IVehiculoCsvStorage {
    
    private ILogger _logger = Log.Logger.ForContext<VehiculoCsvStorage>();

    public VehiculoCsvStorage() {
        InitStorage();
    }

    public Result<IEnumerable<Vehiculo>, DomainError> Cargar(string path) {

        if (!Path.Exists(path)) {
            return Result.Failure<IEnumerable<Vehiculo>, DomainError>(StorageErrors.FileNotFound(path));
        }

        try { 
            var vehiculos = File.ReadLines(path, Encoding.UTF8)
                .Skip(1)
                .Select(l => l.Split(";"))
                .Select(campos => new VehiculoDto(
                        int.Parse(campos[0]),
                        campos[1],
                        campos[2],
                        campos[3],
                        int.Parse(campos[4]),
                        campos[5],
                        campos[6],
                        campos[7],
                        campos[8],
                        bool.TryParse(campos[9], out var d) && d
                        ).ToModel()
                );
            return Result.Success<IEnumerable<Vehiculo>, DomainError>(vehiculos);
        } catch (Exception e) {
            return Result.Failure<IEnumerable<Vehiculo>, DomainError>(StorageErrors.InvalidFormat(e.Message));
        }    
    }

    public Result<bool, DomainError> Salvar(IEnumerable<Vehiculo> items, string path) {
        try {
            _logger.Debug("Intentando salvar los datos en formato csv.");
            using var writer = new StreamWriter(path, false, Encoding.UTF8);
            writer.WriteLine("Id;Matricula;Marca;Modelo;Cilindrada;Motor;DniDueño;CreateAt;UpdateAt;IsDelete");

            foreach (var v in items) {
                var dto = v.ToDto();
                writer.WriteLine($"{dto.Id};{dto.Matricula};{dto.Marca};{dto.Modelo};{dto.Cilindrada};{dto.Motor};{dto.DniDueño};;{dto.CreateAt};{dto.UpdateAt}{dto.IsDelete}");
            }

            return Result.Success<bool, DomainError>(true);

        } catch (Exception e) {
            return Result.Failure<bool, DomainError>(StorageErrors.WriteError(e.Message));
        }        
    }

    private void InitStorage() {
        if (Directory.Exists(Configuracion.DataFolder)) return;
        Directory.CreateDirectory(Configuracion.DataFolder);
    }
}