using CSharpFunctionalExtensions;
using Itv.Errors;
using Itv.Errors.Common;
using Itv.Factory;
using Itv.Models;
using Serilog;

namespace Itv.Repository.Memory;

public class VehiculoMemoryRepository : IVehiculoMemoryRepository {
    private readonly ILogger _logger = Log.ForContext<VehiculoMemoryRepository>();

    
    private Dictionary<int, Vehiculo> _almacenId = new();
    private Dictionary<string, int> _almacenMatricula = new();
    private int _idCount;
    
    private VehiculoMemoryRepository(bool dropData, bool seedData) {
        if (dropData) DeleteAll();
        if(seedData) foreach (var v in VehiculoFactory.Seed()) Create(v);
    }

    /// <inheritdoc cref="IVehiculoMemoryRepository.GetAll" />
    public IEnumerable<Vehiculo> GetAll() {
        return _almacenId.Values;
    }

    /// <inheritdoc cref="IVehiculoMemoryRepository.GetById" />
    public Result<Vehiculo, DomainError> GetById(int id) {
        if (_almacenId.GetValueOrDefault(id) == null) return Result.Failure<Vehiculo, DomainError>(RepositoryErrors.IdNotFound(id));
        return Result.Success<Vehiculo, DomainError>(_almacenId[id]);
    }

    /// <inheritdoc cref="IVehiculoMemoryRepository.Create" />
    public Result<Vehiculo, DomainError> Create(Vehiculo entity) {
        if (_almacenMatricula.ContainsKey(entity.Matricula) || _almacenId.Values.Count(v => v.DniDueño == entity.DniDueño) >= 3) {
            _logger.Debug("No se ha podido crear el vehiculo.");
            return Result.Failure<Vehiculo, DomainError>(RepositoryErrors.MatriculaNotFound(entity));
        }

        var vehiculo = entity with { Id = GetNewId(), IsDelete = false};
        
        _almacenId.Add(vehiculo.Id, vehiculo);
        _almacenMatricula.Add(vehiculo.Matricula, vehiculo.Id);
        _logger.Debug("Vehiculo creado correctamente.");
        return Result.Success<Vehiculo, DomainError>(vehiculo);
    }

    /// <inheritdoc cref="IVehiculoMemoryRepository.Update" />
    public Result<Vehiculo, DomainError> Update(int id, Vehiculo entity) {
        if (!_almacenId.TryGetValue(id, out var viejo)) {
            _logger.Debug("No se ha podido actualizar el vehiculo el id no existe.");
            return Result.Failure<Vehiculo, DomainError>(RepositoryErrors.IdNotFound(id));
        }
        // Si la matrícula que nos dan es diferente de la que teniamos hay que mirar si la matrícula nueva ya la tiene otro coche que ya existe
        // y el coche que ya existe con esa matrícula tiene el mismo id que el vehículo proporcionado.
        if (entity.Matricula != viejo.Matricula && _almacenMatricula.TryGetValue(entity.Matricula, out var idExistente) && idExistente != id) {
            _logger.Debug("No se ha podido actualizar el vehiculo, fallo con las matriculas.");
            return Result.Failure<Vehiculo, DomainError>(RepositoryErrors.InvalidMatricula(id));
        }

        var vehiculoNuevo = entity with { Id = id, IsDelete = false};

        _almacenId[id] = vehiculoNuevo;
        // Si al final la matrícula la han cambiado solo tenemos que eliminar la que había y añadir la nueva.
        if (entity.Matricula != viejo.Matricula) {
            _almacenMatricula.Remove(viejo.Matricula);
            _almacenMatricula.Add(vehiculoNuevo.Matricula, vehiculoNuevo.Id);
            _logger.Debug("Se ha actualizado la lista de matriculas correctamente..");
        }

        return Result.Success<Vehiculo, DomainError>(vehiculoNuevo);
    }

    /// <inheritdoc cref="IVehiculoMemoryRepository.Delete" />
    public Result<Vehiculo, DomainError> Delete(int id) {
        if (!_almacenId.TryGetValue(id, out var eliminado)) {
            _logger.Debug("No se ha podido eliminar el vehiculo, el id no existe.");
            return Result.Failure<Vehiculo, DomainError>(RepositoryErrors.IdNotFound(id));
        }

        eliminado = eliminado with { IsDelete = true };
        _almacenId[id] = eliminado;
        
        return Result.Success<Vehiculo, DomainError>(eliminado);
    }
    
    /// <inheritdoc cref="IVehiculoMemoryRepository.DeleteHard" />
    public Result<Vehiculo, DomainError> DeleteHard(int id) {
        if (!_almacenId.TryGetValue(id, out var eliminado)) {
            _logger.Debug("No se ha podido eliminar el vehiculo, el id no existe.");
            return Result.Failure<Vehiculo, DomainError>(RepositoryErrors.IdNotFound(id));
        }

        _almacenMatricula.Remove(eliminado.Matricula);
        _almacenId.Remove(id);

        return Result.Success<Vehiculo, DomainError>(eliminado);
    }

    /// <inheritdoc cref="IVehiculoMemoryRepository.DeleteAll" />
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