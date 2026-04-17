using System.Runtime.CompilerServices;
using Itv.Config;
using Itv.Factory;
using Itv.Models;
using Serilog;

namespace Itv.Repository.Memory;

public class VehiculoMemoryRepository : IVehiculoRepository {
    private readonly ILogger _logger = Log.ForContext<VehiculoMemoryRepository>();

    
    private Dictionary<int, Vehiculo> _almacenId = new();
    private Dictionary<string, int> _almacenMatricula = new();
    private int _idCount;
    
    private VehiculoMemoryRepository() : this(Configuracion.DropData, Configuracion.SeedData) {}

    private VehiculoMemoryRepository(bool dropData, bool seedData) {
        if (dropData) DeleteAll();
        if(seedData) foreach (var v in VehiculoFactory.Seed()) Create(v);
    }

    /// <inheritdoc cref="IVehiculoRepository.GetAll" />
    public IEnumerable<Vehiculo> GetAll() {
        return _almacenId.Values;
    }

    /// <inheritdoc cref="IVehiculoRepository.GetById" />
    public Vehiculo? GetById(int id) {
        return _almacenId.GetValueOrDefault(id);
    }

    /// <inheritdoc cref="IVehiculoRepository.Create" />
    public Vehiculo? Create(Vehiculo entity) {
        if (_almacenMatricula.ContainsKey(entity.Matricula) || _almacenId.Values.Count(v => v.DniDueño == entity.DniDueño) >= 3) {
            _logger.Debug("No se ha podido crear el vehiculo.");
            return null;
        }

        var vehiculo = entity with { Id = GetNewId(), IsDelete = false};
        
        _almacenId.Add(vehiculo.Id, vehiculo);
        _almacenMatricula.Add(vehiculo.Matricula, vehiculo.Id);
        _logger.Debug("Vehiculo creado correctamente.");
        return vehiculo;
    }

    /// <inheritdoc cref="IVehiculoRepository.Update" />
    public Vehiculo? Update(int id, Vehiculo entity) {
        if (!_almacenId.TryGetValue(id, out var viejo)) {
            _logger.Debug("No se ha podido actualizar el vehiculo el id no existe.");
            return null;
        }
        // Si la matrícula que nos dan es diferente de la que teniamos hay que mirar si la matrícula nueva ya la tiene otro coche que ya existe
        // y el coche que ya existe con esa matrícula tiene el mismo id que el vehículo proporcionado.
        if (entity.Matricula != viejo.Matricula && _almacenMatricula.TryGetValue(entity.Matricula, out var idExistente) && idExistente != id) {
            _logger.Debug("No se ha podido actualizar el vehiculo, fallo con las matriculas.");
            return null;
        }

        var vehiculoNuevo = entity with { Id = id, IsDelete = false};

        _almacenId[id] = vehiculoNuevo;
        // Si al final la matrícula la han cambiado solo tenemos que eliminar la que había y añadir la nueva.
        if (entity.Matricula != viejo.Matricula) {
            _almacenMatricula.Remove(viejo.Matricula);
            _almacenMatricula.Add(vehiculoNuevo.Matricula, vehiculoNuevo.Id);
            _logger.Debug("Se ha actualizado la lista de matriculas correctamente..");
        }

        return vehiculoNuevo;
    }

    /// <inheritdoc cref="IVehiculoRepository.Delete" />
    public Vehiculo? Delete(int id) {
        if (!_almacenId.TryGetValue(id, out var eliminado)) {
            _logger.Debug("No se ha podido eliminar el vehiculo, el id no existe.");
            return null;
        }

        eliminado = eliminado with { IsDelete = true };
        _almacenId[id] = eliminado;
        
        return eliminado;
    }
    
    /// <inheritdoc cref="IVehiculoRepository.DeleteHard" />
    public Vehiculo? DeleteHard(int id) {
        if (!_almacenId.TryGetValue(id, out var eliminado)) {
            _logger.Debug("No se ha podido eliminar el vehiculo, el id no existe.");
            return null;
        }

        _almacenMatricula.Remove(eliminado.Matricula);
        _almacenId.Remove(id);

        return eliminado;
    }

    /// <inheritdoc cref="IVehiculoRepository.DeleteAll" />
    public bool DeleteAll() {
        _almacenId.Clear();
        _almacenMatricula.Clear();
        return true;
    }

    /// <summary>
    /// Devuelve un nuevo id unico.
    /// </summary>
    /// <returns>Id unico.</returns>
    private int GetNewId() {
        return _idCount++;
    }
}