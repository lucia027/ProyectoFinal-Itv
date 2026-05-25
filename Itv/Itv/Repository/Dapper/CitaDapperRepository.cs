using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using Itv.Entity;
using Itv.Enums;
using Itv.Errors;
using Itv.Errors.Common;
using Itv.Factory;
using Itv.Mappers;
using Itv.Models;
using Itv.Repository.Common;
using Serilog;

namespace Itv.Repository.Dapper;

/// <summary>
/// Repositorio en Dapper para gestionar las citas.
/// </summary>
public class CitaDapperRepository : ICitaRepository {
    private readonly ILogger _logger = Log.ForContext<CitaDapperRepository>();
    private readonly IDbConnection _connection;
    private Action? _onDispose;


    public CitaDapperRepository(IDbConnection connection,  Action? onDispose = null, bool dropData = false, bool seedData = false) {
        _connection = connection;
        _onDispose = onDispose;
        EnsureTable(dropData);

        if (seedData) Seed();
    }

    private void EnsureTable(bool dropData) {
        if(_connection.State != ConnectionState.Open)  _connection.Open(); 
        if(dropData) _connection.Execute("DROP TABLE IF EXISTS Cita");

        _connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Cita (
                Id INTEGER PRIMARY KEY,
                Matricula TEXT NOT NULL UNIQUE,
                Marca TEXT NOT NULL,
                Modelo TEXT NOT NULL,
                Cilindrada INTEGER,
                Motor TEXT NOT NULL DEFAULT 'Diesel',
                DniDueño TEXT NOT NULL,
                FechaMatriculacion TEXT NOT NULL,
                FechaInspeccion  TEXT NOT NULL,
                CreateAt TEXT NOT NULL,
                UpdateAt TEXT,
                IsDelete INTEGER NOT NULL DEFAULT 0
        )");
    }

    /// <inheritdoc cref="ICitaRepository.GetAll" />
    public IEnumerable<Cita> GetAll(int pagina = 1, int tamPagina = 5, bool isDeleteInclude = true, string campoBusqueda = "%%") {
        try {
            campoBusqueda = $"%{campoBusqueda?.Trim() ?? ""}%";
            String sql = "";
            if (isDeleteInclude) {
                sql = @"
                    SELECT * FROM Cita
                    WHERE LOWER(Matricula) LIKE LOWER(@CampoBusqueda) 
                    OR LOWER(Marca) LIkE LOWER(@CampoBusqueda)
                    OR LOWER(Modelo) LIKE LOWER(@CampoBusqueda)
                    OR LOWER(Cilindrada) LIKE LOWER(@CampoBusqueda)
                    OR LOWER(Motor) LIKE LOWER(@CampoBusqueda)
                    OR LOWER(DniDueño) LIKE LOWER(@CampoBusqueda)
                    ORDER BY Id LIMIT @TamPagina OFFSET @Offset
                ";
            }
            
            if(!isDeleteInclude)  {
                sql = @"
                    SELECT * FROM Cita
                    WHERE IsDelete = 0
                    AND (
                    LOWER(Matricula) LIKE LOWER(@CampoBusqueda) 
                    OR LOWER(Marca) LIkE LOWER(@CampoBusqueda)
                    OR LOWER(Modelo) LIKE LOWER(@CampoBusqueda)
                    OR LOWER(Cilindrada) LIKE LOWER(@CampoBusqueda)
                    OR LOWER(Motor) LIKE LOWER(@CampoBusqueda)
                    OR LOWER(DniDueño) LIKE LOWER(@CampoBusqueda))
                    ORDER BY Id LIMIT @TamPagina OFFSET @Offset
                ";
            }
            
            var entidades = _connection.Query<CitaEntity>(sql, new { TamPagina = tamPagina, Offset = (pagina - 1), CampoBusqueda =  campoBusqueda }).ToList();
            return entidades.Select(c => c.ToModel()).ToList();
    
        } catch (Exception e) {
            _logger.Error($"Ha sucedido un error al intentar obtener todas laas citas, mensaje error: {e.Message}");
            return [];
        }
    }

    /// <inheritdoc cref="ICitaRepository.GetById" />
    public Result<Cita, DomainError> GetById(int id) {
        try {
            var sql = "SELECT * FROM Cita WHERE Id = @Id";
            var entity = _connection.QueryFirstOrDefault<CitaEntity>(sql, new { Id = id });
            return Result.Success<Cita, DomainError>(entity!.ToModel());
        } catch (Exception e) {
            _logger.Error($"Ha surgido un error intentando encontrar la cita con el id={id}, mensaje de error: {e.Message}.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
        }
    }
    
    /// <inheritdoc cref="ICitaRepository.GetByDateInspeccion" />
    public Result<IEnumerable<Cita>, DomainError> GetByDateInspeccion(DateTime inicio, DateTime? fin, bool isDeleteInclude = true) {
        if (fin == null) fin = DateTime.Now;
        try {
            var sql = "";
            if(isDeleteInclude) sql = "SELECT * FROM Cita WHERE FechaInspeccion BETWEEN @Inicio AND @Fin";
            if(!isDeleteInclude) sql = "SELECT * FROM Cita WHERE IsDelete = 0 AND FechaInspeccion BETWEEN @Inicio AND @Fin";
            
            var entidades = _connection.Query<CitaEntity>(sql, new {Inicio = inicio, Fin = fin}).Select(c => c.ToModel());
            return Result.Success<IEnumerable<Cita>, DomainError>(entidades);
        } catch (Exception e) {
            _logger.Error($"No se ha podido encontrar ninguna cita que coincida con el rango de fechas de inspeccion, inicio[{inicio}] y fin[{fin}]. Mensaje de error: {e.Message}.");
            return Result.Failure<IEnumerable<Cita>, DomainError>(RepositoryErrors.NotFoundCitasError());
        }
    }

    /// <inheritdoc cref="ICitaRepository.GetByTipoMotor" />
    public Result<IEnumerable<Cita>, DomainError> GetByTipoMotor(Motor motor, bool isDeleteInclude = true) {
        try {
            var sql = "";
            if(isDeleteInclude) sql = "SELECT * FROM Cita WHERE Motor = @Motor";
            if(!isDeleteInclude) sql = "SELECT * FROM Cita WHERE IsDelete = 0 AND Motor = @Motor";
            
            var entidades = _connection.Query<CitaEntity>(sql, new {Motor = (motor.ToString())}).Select(c => c.ToModel());
            return Result.Success<IEnumerable<Cita>, DomainError>(entidades);
        } catch (Exception e) {
            _logger.Error($"No se ha podido encontrar ninguna cita que coincida con el tipo de motor especificado({motor}). Mensaje de error: {e.Message}.");
            return Result.Failure<IEnumerable<Cita>, DomainError>(RepositoryErrors.NotFoundCitasError());
        }
    }

    /// <inheritdoc cref="ICitaRepository.Create" />
    public Result<Cita, DomainError> Create(Cita entity) {
        if (!VerificacionDniDueño(entity)) {
            _logger.Debug("No se ha podido crear la cita.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.DniDueñoError(entity));
        }
        if (!VerificacionMatricula(entity)) {
            _logger.Debug("No se ha podido crear la cita.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.FechaInspeccionError(entity));
        }

        var nuevaCita = entity with { Id = 0, CreateAt = DateTime.Today,  UpdateAt = null, IsDelete = false };
        var nuevaCitaEntity = nuevaCita.ToEntity();
        try {
            var sql = @" 
                INSERT INTO Cita ( Matricula, Marca, Modelo, Cilindrada, Motor, DniDueño, FechaMatriculacion, FechaInspeccion, CreateAt, UpdateAt, IsDelete)
                VALUES ( @Matricula, @Marca, @Modelo, @Cilindrada, @Motor, @DniDueño, @FechaMatriculacion, @FechaInspeccion, @CreateAt, @UpdateAt, @IsDelete);
                Select last_insert_rowid()";
            var id = _connection.ExecuteScalar<int>(sql, new {
                nuevaCitaEntity.Matricula,
                nuevaCitaEntity.Marca,
                nuevaCitaEntity.Modelo,
                nuevaCitaEntity.Cilindrada,
                nuevaCitaEntity.Motor,
                nuevaCitaEntity.DniDueño,
                nuevaCitaEntity.FechaMatriculacion,
                nuevaCitaEntity.FechaInspeccion,
                nuevaCitaEntity.CreateAt,
                nuevaCitaEntity.UpdateAt,
                nuevaCitaEntity.IsDelete,
            });
            return Result.Success<Cita, DomainError>(GetById(id).Value);
        } catch (Exception e) {
            _logger.Error($"Error en la creacion de la nueva cita, mensaje de error: {e.Message}.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.CreationError());
        }
    }

    /// <inheritdoc cref="ICitaRepository.Update" />
    public Result<Cita, DomainError> Update(int id, Cita entity) {
        var citaVieja = GetById(id);
        if (citaVieja.IsFailure) {
            _logger.Debug("No se ha podido actualizar la cita el id no existe.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
        }
        if (entity.Matricula != citaVieja.Value.Matricula && ComprobarDisponibilidadMatricula(entity.Matricula) && ObtenerIdPorMatricula(entity.Matricula) != id) {
            _logger.Debug("No se ha podido actualizar la cita la matricula ya la tiene otro vehiculo.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.InvalidMatricula(entity.Matricula));
        }
        if (!VerificacionDniDueño(entity)) {
            _logger.Debug("No se ha podido actualizar la cita.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.DniDueñoError(entity));
        }
        if (!VerificacionMatricula(entity)) {
            _logger.Debug("No se ha podido actualizar la cita.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.FechaInspeccionError(entity));
        }
        
        var nuevaCita = entity with { Id = id, UpdateAt = DateTime.Today, IsDelete = false };
        var nuevaCitaEntity = nuevaCita.ToEntity();
        try {
            var sql = @" UPDATE Cita SET  Id = @Id, Matricula = @Matricula, Marca = @Marca, Modelo = @Modelo, Cilindrada = @Cilindrada, Motor = @Motor, DniDueño = @DniDueño, FechaMatriculacion = @FechaMatriculacion, FechaInspeccion = @FechaInspeccion, CreateAt = @CreateAt, UpdateAt = @UpdateAt, IsDelete = @IsDelete
                         Where Id = @Id";
            _connection.Execute(sql, new {
                nuevaCitaEntity.Id,
                nuevaCitaEntity.Matricula,
                nuevaCitaEntity.Marca,
                nuevaCitaEntity.Modelo,
                nuevaCitaEntity.Cilindrada,
                nuevaCitaEntity.Motor,
                nuevaCitaEntity.DniDueño,
                nuevaCitaEntity.FechaMatriculacion,
                nuevaCitaEntity.FechaInspeccion,
                nuevaCitaEntity.CreateAt,
                nuevaCitaEntity.UpdateAt,
                nuevaCitaEntity.IsDelete,
            });
            return Result.Success<Cita, DomainError>(nuevaCitaEntity.ToModel());
        } catch (Exception e) {
            _logger.Error($"Error en la actualizacion de la nueva cita, mensaje de error: {e.Message}.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.UpdatingError());
        }
    }

    /// <inheritdoc cref="ICitaRepository.Delete" />
    public Result<Cita, DomainError> Delete(int id) {
        if (GetById(id).IsFailure) {
            _logger.Debug("No se ha podido eliminar la cita, el id no existe.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
        }

        try {
            var sql = "UPDATE Cita SET IsDelete = 1 WHERE Id = @Id";
            _connection.Execute(sql, new { Id = id });
            return Result.Success<Cita, DomainError>(GetById(id).Value);
        } catch (Exception e) {
            _logger.Error($"Error en la eliminacion de la cita, mensaje de error: {e.Message}.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.DeletionError());
        }
    }

    /// <inheritdoc cref="ICitaRepository.DeleteHard" />
    public Result<Cita, DomainError> DeleteHard(int id) {
        if (GetById(id).IsFailure) {
            _logger.Debug("No se ha podido eliminar la cita, el id no existe.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
        }

        try {
            var eliminado = GetById(id).Value;
            var sql = "DELETE FROM Cita WHERE Id = @@Id";
            _connection.Execute(sql, new { Id = id });
            return Result.Success<Cita, DomainError>(eliminado);
        } catch (Exception e) {
            _logger.Error($"Error en la eliminacion de la cita, mensaje de error: {e.Message}.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.DeletionError());
        }    
    }

    /// <inheritdoc cref="ICitaRepository.DeleteAll" />
    public bool DeleteAll() {
        try {
            var sql = "DELETE FROM Cita";
            _connection.Execute(sql);
            return true;
        } catch (Exception e) {
            _logger.Error($"Error en la eliminacion de la cita, mensaje de error: {e.Message}.");
            return false;
        }
    }
    
    private void Seed() {
        foreach (var c in CitasFactory.Seed()) Create(c);
    }
    
    private bool VerificacionDniDueño(Cita cita) {
        try {
            var sql = "SELECT COUNT(*) FROM Cita WHERE DniDueño LIKE @DniDueño AND FechaInspeccion LIKE @FechaInspeccion";
            return _connection.ExecuteScalar<int>(sql, new { cita.DniDueño, cita.FechaInspeccion}) < 3;
        } catch (Exception e) {
            _logger.Error($"Error verificando la condicion del dni del dueño, mensaje: {e.Message}.");
            return false;
        }
    }
    
    private bool VerificacionMatricula(Cita cita) {
        try {
            var sql = "SELECT COUNT(*) FROM Cita WHERE Matricula LIKE @Matricula AND FechaInspeccion LIKE @FechaInspeccion";
            return _connection.ExecuteScalar<int>(sql, new { cita.Matricula, cita.FechaInspeccion}) == 0;
        } catch (Exception e) {
            _logger.Error($"Error verificando la condicion de la matricula, mensaje: {e.Message}.");
            return false;
        }
    }

    private bool ComprobarDisponibilidadMatricula(string matricula) {
        try {
            var sql = "SELECT COUNT(*) FROM Cita WHERE Matricula LIKE @Matricula";
            return _connection.ExecuteScalar<int>(sql, new { Matricula = matricula}) >= 1;
        } catch (Exception e) {
            _logger.Error($"Error verificando la condicion de la matricula, mensaje: {e.Message}.");
            return false;
        }
    }

    private int? ObtenerIdPorMatricula(string matricula) {
        try {
            var sql = "SELECT Id FROM Cita WHERE Matricula LIKE @Matricula";
            if (_connection.ExecuteScalar<int>(sql, new { Matricula = matricula }) == 0) return null;
            return _connection.ExecuteScalar<int>(sql, new { Matricula = matricula });
        } catch (Exception e) {
            _logger.Error($"Error verificando la condicion de la matricula, mensaje: {e.Message}.");
            return null;
        }
    }
}