using CSharpFunctionalExtensions;
using Itv.Errors;
using Itv.Errors.Common;
using Itv.Factory;
using Itv.Models;
using Serilog;

namespace Itv.Repository.Memory;

public class CitaMemoryRepository : ICitaMemoryRepository {
    private readonly ILogger _logger = Log.ForContext<CitaMemoryRepository>();

    
    private readonly Dictionary<int, Cita> _almacenId = new();
    private readonly Dictionary<string, int> _almacenMatricula = new();
    private int _idCount;
    
    public CitaMemoryRepository(bool dropData, bool seedData) {
        if (dropData) DeleteAll();
        if (seedData) foreach (var v in CitasFactory.Seed()) Create(v);
    }

    /// <inheritdoc cref="ICitaMemoryRepository.GetAll" />
    public IEnumerable<Cita> GetAll(int pagina = 1, int tamPagina = 5, bool isDeleteInclude = true) {
        if (!isDeleteInclude) {
            return _almacenId.Values
                .OrderBy(v => v.Id)
                .Where(v => v.IsDelete == false)
                .Skip((pagina -1) * tamPagina)
                .Take(tamPagina);
        }

        return _almacenId.Values                
            .OrderBy(v => v.Id)
            .Skip((pagina -1) * tamPagina)
            .Take(tamPagina);
    }

    /// <inheritdoc cref="ICitaMemoryRepository.GetById" />
    public Result<Cita, DomainError> GetById(int id) {
        if (_almacenId.GetValueOrDefault(id) == null) return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
        return Result.Success<Cita, DomainError>(_almacenId[id]);
    }

    /// <inheritdoc cref="ICitaMemoryRepository.Create" />
    public Result<Cita, DomainError> Create(Cita entity) {
        if (_almacenMatricula.ContainsKey(entity.Matricula)) {
            _logger.Debug("No se ha podido crear la cita.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.InvalidMatricula(entity.Matricula));
        }
        if (_almacenId.Values.Count(v => v.DniDueño == entity.DniDueño) >= 3) {
            _logger.Debug("No se ha podido crear la cita.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.DniDueñoError(entity));
        }

        var cita = entity with { Id = GetNewId(), CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false};
        
        _almacenId.Add(cita.Id, cita);
        _almacenMatricula.Add(cita.Matricula, cita.Id);
        _logger.Debug("Cita creada correctamente.");
        return Result.Success<Cita, DomainError>(cita);
    }

    /// <inheritdoc cref="ICitaMemoryRepository.Update" />
    public Result<Cita, DomainError> Update(int id, Cita entity) {
        if (!_almacenId.TryGetValue(id, out var viejo)) {
            _logger.Debug("No se ha podido actualizar la cita el id no existe.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
        }
        // Si la matrícula que nos dan es diferente de la que teniamos hay que mirar si la matrícula nueva ya la tiene otro coche que ya existe
        // y el coche que ya existe con esa matrícula tiene el mismo id que la cita proporcionada.
        if (entity.Matricula != viejo.Matricula && _almacenMatricula.TryGetValue(entity.Matricula, out var idExistente) && idExistente != id) {
            _logger.Debug("No se ha podido actualizar la cita, fallo con las matriculas.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.InvalidMatricula(entity.Matricula));
        }

        var citaNuevo = entity with { Id = id, UpdateAt = DateTime.Today ,IsDelete = false};

        _almacenId[id] = citaNuevo;
        // Si al final la matrícula la han cambiado solo tenemos que eliminar la que había y añadir la nueva.
        if (entity.Matricula != viejo.Matricula) {
            _almacenMatricula.Remove(viejo.Matricula);
            _almacenMatricula.Add(citaNuevo.Matricula, citaNuevo.Id);
            _logger.Debug("Se ha actualizado la lista de matriculas correctamente..");
        }

        return Result.Success<Cita, DomainError>(citaNuevo);
    }

    /// <inheritdoc cref="ICitaMemoryRepository.Delete" />
    public Result<Cita, DomainError> Delete(int id) {
        if (!_almacenId.TryGetValue(id, out var eliminado)) {
            _logger.Debug("No se ha podido eliminar la cita, el id no existe.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
        }

        eliminado = eliminado with { IsDelete = true };
        _almacenId[id] = eliminado;
        
        return Result.Success<Cita, DomainError>(eliminado);
    }
    
    /// <inheritdoc cref="ICitaMemoryRepository.DeleteHard" />
    public Result<Cita, DomainError> DeleteHard(int id) {
        if (!_almacenId.TryGetValue(id, out var eliminado)) {
            _logger.Debug("No se ha podido eliminar la cita, el id no existe.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
        }

        _almacenMatricula.Remove(eliminado.Matricula);
        _almacenId.Remove(id);

        return Result.Success<Cita, DomainError>(eliminado);
    }

    /// <inheritdoc cref="ICitaMemoryRepository.DeleteAll" />
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