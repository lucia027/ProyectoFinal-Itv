using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;
using Itv.Dto;
using Itv.Errors;
using Itv.Errors.Common;
using Itv.Mappers;
using Itv.Models;
using Serilog;

namespace Itv.Storage.Json;

/// <summary>
/// Almacenamiento de los datos en json para las citas.
/// </summary>
public class CitaJsonStorage : ICitaJsonStorage {
    private readonly ILogger _logger = Log.ForContext<CitaJsonStorage>();

    private readonly JsonSerializerOptions _options = new() {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() },
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping 
    };
    
    public CitaJsonStorage() {
        InitStorage();
    }
    
    /// <inheritdoc cref="ICitaJsonStorage.Cargar" />
    public Result<IEnumerable<Cita>, DomainError> Cargar(string path) {
        if (!Path.Exists(path)) {
            _logger.Debug("Intentando cargar los datos en formato json.");
            return Result.Failure<IEnumerable<Cita>, DomainError>(StorageErrors.FileNotFound(path));
        }

        try {
            var json = File.ReadAllText(path, Encoding.UTF8);
            var dtos = JsonSerializer.Deserialize<List<CitaDto>>(json, _options);

            if (dtos == null) {
                return Result.Failure<IEnumerable<Cita>, DomainError>(StorageErrors.InvalidFormat("Los dtos no se han podido deserializar."));
            }
            return Result.Success<IEnumerable<Cita>, DomainError>(dtos.Select(v => v.ToModel()));
        } catch (Exception e) {
            return Result.Failure<IEnumerable<Cita>, DomainError>(StorageErrors.ReadError(e.Message));
        }
    }

    /// <inheritdoc cref="ICitaJsonStorage.Salvar" />
    public Result<bool, DomainError> Salvar(IEnumerable<Cita> items, string path) {
        try {
            var dtos = items
                .Select(i => i.ToDto())
                .ToList();
            var json = JsonSerializer.Serialize(dtos, _options);
            File.WriteAllText(path, json, new UTF8Encoding(false));
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