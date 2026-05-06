using CSharpFunctionalExtensions;
using Itv.Enums;
using Itv.Errors;
using Itv.Errors.Common;
using Itv.Factory;
using Itv.Models;
using Itv.Repository.Common;
using Serilog;

namespace Itv.Repository.Memory;

/// <summary>
/// Repositorio en memoria para gestionar las citas.
/// </summary>
public class CitaMemoryRepository : ICitaRepository {
    private readonly ILogger _logger = Log.ForContext<CitaMemoryRepository>();

    
    private readonly Dictionary<int, Cita> _almacenId = new();
    private readonly Dictionary<string, int> _almacenMatricula = new();
    private int _idCount;
    
    public CitaMemoryRepository(bool dropData, bool seedData) {
        if (dropData) DeleteAll();
        if (seedData) foreach (var v in CitasFactory.Seed()) Create(v);
    }

    /// <inheritdoc cref="ICitaRepository.GetAll" />
    public IEnumerable<Cita> GetAll(int pagina = 1, int tamPagina = 5, bool isDeleteInclude = true, string campoBusqueda = "") {
        if (!isDeleteInclude) {
            return _almacenId.Values
                .OrderBy(v => v.Matricula)
                .Where(v => v.IsDelete == false && 
                            v.Matricula.Contains(campoBusqueda) || 
                            v.Marca.Contains(campoBusqueda) ||
                            v.Modelo.Contains(campoBusqueda) ||
                            v.Cilindrada.ToString().Contains(campoBusqueda) ||
                            v.Motor.ToString().Contains(campoBusqueda) ||
                            v.DniDueño.Contains(campoBusqueda)
                )
                .Skip((pagina -1) * tamPagina)
                .Take(tamPagina);
        }

        return _almacenId.Values                
            .OrderBy(v => v.Matricula)
            .Where(v => v.Matricula.Contains(campoBusqueda) || 
                        v.Marca.Contains(campoBusqueda) ||
                        v.Modelo.Contains(campoBusqueda) ||
                        v.Cilindrada.ToString().Contains(campoBusqueda) ||
                        v.Motor.ToString().Contains(campoBusqueda) ||
                        v.DniDueño.Contains(campoBusqueda)
            )
            .Skip((pagina -1) * tamPagina)
            .Take(tamPagina);    
    }

    /// <inheritdoc cref="ICitaRepository.GetById" />
    public Result<Cita, DomainError> GetById(int id) {
        if (_almacenId.GetValueOrDefault(id) == null) return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
        return Result.Success<Cita, DomainError>(_almacenId[id]);
    }
    
    /// <inheritdoc cref="ICitaRepository.GetByDateMatricula" />
    public Result<IEnumerable<Cita>, DomainError> GetByDateMatricula(DateTime inicio, DateTime? fin, bool isDeleteInclude = true) {
        if(fin == null) fin = DateTime.Now;
        IEnumerable<Cita> citas;
        if (!isDeleteInclude) {
            citas = _almacenId.Values
                .OrderBy(v => v.Matricula)
                .Where(v => inicio <= v.FechaMatriculacion && v.FechaMatriculacion <= fin);
            return Result.Success<IEnumerable<Cita>, DomainError>(citas);    
        }

        citas = _almacenId.Values
            .OrderBy(v => v.Matricula)
            .Where(v => inicio <= v.FechaMatriculacion && v.FechaMatriculacion <= fin);

        if (!citas.Any()) return Result.Failure<IEnumerable<Cita>, DomainError>(RepositoryErrors.NotFoundCitasError());
        return Result.Success<IEnumerable<Cita>, DomainError>(citas);    
    }

    public Result<IEnumerable<Cita>, DomainError> GetByTipoMotor( bool isDeleteInclude, Motor motor) {
        IEnumerable<Cita> citas = [];

        if (!isDeleteInclude) {
            switch (motor) {
                case Motor.Diesel:
                    citas.Select(c => c.Motor == Motor.Diesel && c.IsDelete == false);
                    break;
                case Motor.Electrico:
                    citas.Select(c => c.Motor == Motor.Electrico && c.IsDelete == false);
                    break;
                case Motor.Gasolina:
                    citas.Select(c => c.Motor == Motor.Gasolina && c.IsDelete == false);
                    break;
                case Motor.Hibrido:
                    citas.Select(c => c.Motor == Motor.Hibrido && c.IsDelete == false);
                    break;
                default:
                    break;
            }
        }
        
        if (isDeleteInclude) {
            switch (motor) {
                case Motor.Diesel:
                    citas.Select(c => c.Motor == Motor.Diesel && c.IsDelete == true);
                    break;
                case Motor.Electrico:
                    citas.Select(c => c.Motor == Motor.Electrico && c.IsDelete == true);
                    break;
                case Motor.Gasolina:
                    citas.Select(c => c.Motor == Motor.Gasolina && c.IsDelete == true);
                    break;
                case Motor.Hibrido:
                    citas.Select(c => c.Motor == Motor.Hibrido && c.IsDelete == true);
                    break;
                default:
                    break;
            }
        }
        
        if (!citas.Any()) return Result.Failure<IEnumerable<Cita>, DomainError>(RepositoryErrors.NotFoundCitasError());
        return Result.Success<IEnumerable<Cita>, DomainError>(citas);    
    }

    /// <inheritdoc cref="ICitaRepository.Create" />
    public Result<Cita, DomainError> Create(Cita entity) {
        if (_almacenId.Values.Count(v => v.DniDueño == entity.DniDueño && v.FechaMatriculacion == entity.FechaMatriculacion) >= 3) {
            _logger.Debug("No se ha podido crear la cita.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.DniDueñoError(entity));
        }
        if (_almacenId.Values.Any(c => c.Matricula == entity.Matricula && c.FechaMatriculacion == entity.FechaMatriculacion)) {
            _logger.Debug("No se ha podido crear la cita.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.FechaMatriculacionError(entity));
        } 

        var cita = entity with { Id = GetNewId(), CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false};
        
        _almacenId.Add(cita.Id, cita);
        _almacenMatricula.Add(cita.Matricula, cita.Id);
        _logger.Debug("Cita creada correctamente.");
        return Result.Success<Cita, DomainError>(cita);
    }

    /// <inheritdoc cref="ICitaRepository.Update" />
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

    /// <inheritdoc cref="ICitaRepository.Delete" />
    public Result<Cita, DomainError> Delete(int id) {
        if (!_almacenId.TryGetValue(id, out var eliminado)) {
            _logger.Debug("No se ha podido eliminar la cita, el id no existe.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
        }

        eliminado = eliminado with { IsDelete = true };
        _almacenId[id] = eliminado;
        
        return Result.Success<Cita, DomainError>(eliminado);
    }
    
    /// <inheritdoc cref="ICitaRepository.DeleteHard" />
    public Result<Cita, DomainError> DeleteHard(int id) {
        if (!_almacenId.TryGetValue(id, out var eliminado)) {
            _logger.Debug("No se ha podido eliminar la cita, el id no existe.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
        }

        _almacenMatricula.Remove(eliminado.Matricula);
        _almacenId.Remove(id);

        return Result.Success<Cita, DomainError>(eliminado);
    }

    /// <inheritdoc cref="ICitaRepository.DeleteAll" />
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