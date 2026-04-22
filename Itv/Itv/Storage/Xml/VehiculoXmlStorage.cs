using System.Text;
using System.Xml;
using System.Xml.Serialization;
using CSharpFunctionalExtensions;
using Itv.Dto;
using Itv.Errors;
using Itv.Errors.Common;
using Itv.Mappers;
using Itv.Models;
using Serilog;

namespace Itv.Storage.Xml;

public class VehiculoXmlStorage : IVehiculoXmlStorage{
    
    private readonly ILogger _logger = Log.ForContext<VehiculoXmlStorage>();

    private readonly XmlSerializerNamespaces _xmlSerializerNamespaces = new();
    private readonly XmlWriterSettings _xmlWriterSettings = new() {
        Indent = true,
        Encoding = Encoding.UTF8
    };

    public VehiculoXmlStorage() {
        InitStorage();
    }

    /// <inheritdoc cref="IVehiculoXmlStorage.Cargar" />
    public Result<IEnumerable<Vehiculo>, DomainError> Cargar(string path) {
        if (!Path.Exists(path)) {
            _logger.Debug("Intentando salvar los datos en formato xml.");
            return Result.Failure<IEnumerable<Vehiculo>, DomainError>(StorageErrors.FileNotFound(path));
        }

        try {
            var serializer = new XmlSerializer(typeof(List<VehiculoDto>));
            using var stream = File.OpenRead(path);
            var dtos = serializer.Deserialize(stream) as List<VehiculoDto>;
            var vehiculos =  dtos?.Select(dto => dto.ToModel());

            return Result.Success<IEnumerable<Vehiculo>, DomainError>(vehiculos);
        }
        catch (Exception e)
        {
            return Result.Failure<IEnumerable<Vehiculo>, DomainError>(StorageErrors.ReadError(e.Message));
        }    
    }

    /// <inheritdoc cref="IVehiculoXmlStorage.Salvar" />
    public Result<bool, DomainError> Salvar(IEnumerable<Vehiculo> items, string path) {
        try {
            var dtos = items.Select(v => v.ToDto()).ToList();
            var serializer = new XmlSerializer(typeof(List<VehiculoDto>));

            using var streamWriter = new StreamWriter(path, false, Encoding.UTF8);
            using var xmlWriter = XmlWriter.Create(streamWriter, _xmlWriterSettings);
            serializer.Serialize(xmlWriter, dtos, _xmlSerializerNamespaces);
            
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