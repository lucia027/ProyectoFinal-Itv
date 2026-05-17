using CSharpFunctionalExtensions;
using Itv.Entity;
using Itv.Enums;
using Itv.Errors;
using Itv.Errors.Common;
using Itv.Factory;
using Itv.Mappers;
using Itv.Models;
using Itv.Repository.Common;
using Microsoft.EntityFrameworkCore;
using Serilog;
namespace Itv.Repository.Efc;

public class CitaEfcRepository : ICitaRepository {

    private readonly ILogger _logger = Log.ForContext<CitaEfcRepository>();
    private readonly AppDbContext _context;

    public CitaEfcRepository(AppDbContext context, bool dropData = false, bool seedData = false) {
        _context = context;
        
        if (dropData) _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        if (seedData) {
            foreach (var c in CitasFactory.Seed()){
                Create(c);
            }
        }
    }

    /// <inheritdoc cref="ICitaRepository.GetAll" />
    public IEnumerable<Cita> GetAll(int pagina = 1, int tamPagina = 5, bool isDeleteInclude = true, string campoBusqueda = "") {
        var query = _context.Citas.AsNoTracking();
        if (!isDeleteInclude) {
            query =  query
                .OrderBy(v => v.Matricula)
                .Where(v => v.IsDelete == false && (
                            v.Matricula.Contains(campoBusqueda) || 
                            v.Marca.Contains(campoBusqueda) ||
                            v.Modelo.Contains(campoBusqueda) ||
                            v.Cilindrada.ToString().Contains(campoBusqueda) ||
                            v.Motor.ToString().Contains(campoBusqueda) ||
                            v.DniDueño.Contains(campoBusqueda))
                )
                .Skip((pagina -1) * tamPagina)
                .Take(tamPagina);
            return query.AsEnumerable().ToModel();
        }

        query = query           
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
        return query.AsEnumerable().ToModel();
    }

    /// <inheritdoc cref="ICitaRepository.GetById" />
    public Result<Cita, DomainError> GetById(int id) {
        try {
            var entity = _context.Citas.FirstOrDefault(c => c.Id == id);
            return Result.Success<Cita, DomainError>(entity.ToModel());
        } catch (Exception e) {
            _logger.Error($"Error al intentar encontrar la cita con el id: {id}.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
        }
    }
    
    /// <inheritdoc cref="ICitaRepository.GetByDateInspeccion" />
    public Result<IEnumerable<Cita>, DomainError> GetByDateInspeccion(DateTime inicio, DateTime? fin, bool isDeleteInclude = true) {
        try {
            fin ??= DateTime.Now;
            var query = _context.Citas.AsNoTracking();
            IEnumerable<Cita> citas;
            
            if (!isDeleteInclude) {
                citas = query
                    .OrderBy(v => v.Matricula)
                    .Where(v => v.IsDelete == false && ( inicio <= v.FechaInspeccion && v.FechaInspeccion <= fin))
                    .AsEnumerable()
                    .ToModel();
                return Result.Success<IEnumerable<Cita>, DomainError>(citas);    
            }

            citas = query
                .OrderBy(v => v.Matricula)
                .Where(v => inicio <= v.FechaInspeccion && v.FechaInspeccion <= fin)
                .AsEnumerable()
                .ToModel();

            if (!citas.Any()) return Result.Failure<IEnumerable<Cita>, DomainError>(RepositoryErrors.NotFoundCitasError());
            return Result.Success<IEnumerable<Cita>, DomainError>(citas);   
            
        } catch (Exception e) {
            _logger.Error($"Error al intentar encontrar la cita segun su fecha de inspeccion.");
            return Result.Failure<IEnumerable<Cita>, DomainError>(RepositoryErrors.NotFoundCitasError());
        }
    }

    /// <inheritdoc cref="ICitaRepository.GetByTipoMotor" />
    public Result<IEnumerable<Cita>, DomainError> GetByTipoMotor(Motor motor, bool isDeleteInclude = true) {
        try {
            var query = _context.Citas.AsNoTracking();
            IEnumerable<Cita> citas = [];
        
            if (!isDeleteInclude) {
                citas = query
                    .Where(c => c.Motor == motor.ToString() && c.IsDelete == false)
                    .AsEnumerable()
                    .ToModel();
            }
            if (isDeleteInclude) {
                citas = query
                    .Where(c => c.Motor == motor.ToString())
                    .AsEnumerable()
                    .ToModel();
            }
            
            if (!citas.Any()) return Result.Failure<IEnumerable<Cita>, DomainError>(RepositoryErrors.NotFoundCitasError());
            return Result.Success<IEnumerable<Cita>, DomainError>(citas);   
            
        } catch (Exception e) {
            _logger.Error($"Error al intentar encontrar la cita segun su tipo de motor.");
            return Result.Failure<IEnumerable<Cita>, DomainError>(RepositoryErrors.NotFoundCitasError());
        }
    }
    
    /// <inheritdoc cref="ICitaRepository.Create" />
    public Result<Cita, DomainError> Create(Cita entity) {
        try {
            if (_context.Citas.Count(v => v.DniDueño == entity.DniDueño && v.FechaInspeccion == entity.FechaInspeccion) >= 3) {
                _logger.Debug("No se ha podido crear la cita.");
                return Result.Failure<Cita, DomainError>(RepositoryErrors.DniDueñoError(entity));
            }
            if (_context.Citas.Any(c => c.Matricula == entity.Matricula && c.FechaInspeccion == entity.FechaInspeccion)) {
                _logger.Debug("No se ha podido crear la cita.");
                return Result.Failure<Cita, DomainError>(RepositoryErrors.FechaInspeccionError(entity));
            } 

            var cita = entity with { CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false};
        
            _context.Citas.Add(cita.ToEntity());
            _context.SaveChanges();
        
            _logger.Debug("Cita creada correctamente.");
            return Result.Success<Cita, DomainError>(cita);
        } catch (Exception e) {
            _logger.Error($"Error al intentar crear una nueva cita.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.CreationError());
        }
    }

    /// <inheritdoc cref="ICitaRepository.Update" />
    public Result<Cita, DomainError> Update(int id, Cita entity) {
        try {
            if (GetById(id).IsFailure) {
                _logger.Debug("No se ha podido actualizar la cita el id no existe.");
                return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
            }
            var viejo = GetById(id).Value;
            if (entity.Matricula != viejo.Matricula && _context.Citas.Any(c => c.Matricula == entity.Matricula && c.Id != id)) {
                _logger.Debug("No se ha podido actualizar la cita, fallo con las matriculas.");
                return Result.Failure<Cita, DomainError>(RepositoryErrors.InvalidMatricula(entity.Matricula));
            }
            if (_context.Citas.Count(v => v.DniDueño == entity.DniDueño && v.FechaInspeccion == entity.FechaInspeccion) >= 3) {
                _logger.Debug("No se ha podido actualizar la cita.");
                return Result.Failure<Cita, DomainError>(RepositoryErrors.DniDueñoError(entity));
            }
            if (_context.Citas.Any(c => c.Matricula == entity.Matricula && c.FechaInspeccion == entity.FechaInspeccion)) {
                _logger.Debug("No se ha podido actualizar la cita.");
                return Result.Failure<Cita, DomainError>(RepositoryErrors.FechaInspeccionError(entity));
            }

            entity.UpdateAt = DateTime.Today;
            entity.IsDelete = false;
            _context.SaveChanges();
    
            return Result.Success<Cita, DomainError>(entity);
        } catch (Exception e) {
            _logger.Error($"Error al intentar actualizar una cita.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.UpdatingError());
        }
    }

    /// <inheritdoc cref="ICitaRepository.Delete" />
    public Result<Cita, DomainError> Delete(int id) {
        try {
            var eliminado = _context.Citas.FirstOrDefault(c => c.Id == id);

            if (eliminado == null){
                _logger.Debug("No se ha podido eliminar la cita, el id no existe.");
                return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
            }

            eliminado.IsDelete = true;
            _context.SaveChanges();
            
            return Result.Success<Cita, DomainError>(eliminado.ToModel());
        } catch (Exception e) {
            _logger.Error($"Error al intentar eliminar una cita.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.DeletionError());
        }
    }

    /// <inheritdoc cref="ICitaRepository.DeleteHard" />
    public Result<Cita, DomainError> DeleteHard(int id) {
        try {
            var eliminado = _context.Citas.FirstOrDefault(c => c.Id == id);

            if (eliminado == null){
                _logger.Debug("No se ha podido eliminar la cita, el id no existe.");
                return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
            }

            _context.Citas.Remove(eliminado);
            _context.SaveChanges();
            
            return Result.Success<Cita, DomainError>(eliminado.ToModel());
        } catch (Exception e) {
            _logger.Error($"Error al intentar eliminar una cita.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.DeletionError());
        }
    }

    /// <inheritdoc cref="ICitaRepository.DeleteAll" />
    public bool DeleteAll() {
        try {
            _context.Citas.RemoveRange(_context.Citas);
            _context.SaveChanges();
            
            return true;
        } catch (Exception e) {
            _logger.Error($"Error al intentar eliminar una cita.");
            return false;
        }    
    }
}