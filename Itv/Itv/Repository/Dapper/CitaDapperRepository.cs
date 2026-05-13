using System.Data;
using System.Runtime.InteropServices;
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
    private ILogger _logger = Log.ForContext<CitaDapperRepository>();
    private readonly IDbConnection _connection;
    private readonly Action? _onDispose;

    public CitaDapperRepository(IDbConnection connection, Action? onDispose = null, bool dropData = false, bool seedData = false) {
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
                IsDeleted INTEGER NOT NULL DEFAULT 0
        ");
    }

    /// <inheritdoc cref="ICitaRepository.GetAll" />
    public IEnumerable<Cita> GetAll(int pagina = 1, int tamPagina = 5, bool isDeleteInclude = true, string campoBusqueda = "") {
        try {
            String sql = "";
            if (isDeleteInclude) {
                sql = @"
                    SELECT * FROM Cita
                    ORDER BY Id LIMIT @TamPagina OFFSET @Offset
                    WHERE IsDeleted = 1
                    AND Matricula LIKE '%@CampoBusqueda%' 
                    OR Marca LIkE '%@CampoBusqueda%'
                    OR Modelo LIKE '%@CampoBusqueda%'
                    OR Cilindrada LIKE '%@CampoBusqueda%'
                    OR Motor LIKE '%@CampoBusqueda%'
                    OR DniDueño LIKE '%@CampoBusqueda%'
                ";
            }
            
            if(!isDeleteInclude)  {
                sql = @"
                    SELECT * FROM Cita
                    ORDER BY Id LIMIT @TamPagina OFFSET @Offset
                    WHERE IsDeleted = 0
                    AND Matricula LIKE '%@CampoBusqueda%' 
                    OR Marca LIkE '%@CampoBusqueda%'
                    OR Modelo LIKE '%@CampoBusqueda%'
                    OR Cilindrada LIKE '%@CampoBusqueda%'
                    OR Motor LIKE '%@CampoBusqueda%'
                    OR DniDueño LIKE '%@CampoBusqueda%'
                ";
            }
            
            var entities = _connection.Query<CitaEntity>(sql, new { TamPagina = tamPagina, Offset = (pagina - 1), CampoBusqueda =  campoBusqueda }).ToList();
            return entities.Select(CitaMapper.ToModel).OfType<Cita>().ToList();

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
            return Result.Success<Cita, DomainError>(CitaMapper.ToModel(entity));
        } catch (Exception e) {
            _logger.Error($"Ha surgido un error intentando encontrar la cita con el id={id}, mensaje de error: {e.Message}.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
        }
    }
    
    /// <inheritdoc cref="ICitaRepository.GetByDateMatricula" />
    public Result<IEnumerable<Cita>, DomainError> GetByDateMatricula(DateTime inicio, DateTime? fin, bool isDeleteInclude = true) {
        try {
            var sql = "";
            if(isDeleteInclude) sql = "SELECT * FROM Cita WHERE IsDelete = 1 AND FechaMatriculacion BETWEEN @Inicio AND @Fin";
            if(!isDeleteInclude) sql = "SELECT * FROM Cita WHERE IsDelete = 0 AND FechaMatriculacion BETWEEN @Inicio AND @Fin";
            
            var entities = _connection.Query(sql, new {Inicio = inicio, Fin = fin}).OfType<CitaEntity>().Select(c => c.ToModel());
            return Result.Success<IEnumerable<Cita>, DomainError>(entities);
        } catch (Exception e) {
            _logger.Error($"No se ha podido encontrar ninguna cita que coincida con el rango de fechas de matrculacion; inicio[{inicio}] y fin[{fin}]. Mensaje de error: {e.Message}.");
            return Result.Failure<IEnumerable<Cita>, DomainError>(RepositoryErrors.NotFoundCitasError());
        }
    }

    /// <inheritdoc cref="ICitaRepository.GetByTipoMotor" />
    public Result<IEnumerable<Cita>, DomainError> GetByTipoMotor(Motor motor, bool isDeleteInclude = true) {
        try {
            var sql = "";
            if(isDeleteInclude) sql = "SELECT * FROM Cita WHERE IsDelete = 1 AND Motor = @motor";
            if(!isDeleteInclude) sql = "SELECT * FROM Cita WHERE IsDelete = 0 AND Motor = @motor";
            
            var entities = _connection.Query(sql, new {Motor = motor}).OfType<CitaEntity>().Select(c => c.ToModel());
            return Result.Success<IEnumerable<Cita>, DomainError>(entities);
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
            return Result.Failure<Cita, DomainError>(RepositoryErrors.FechaMatriculacionError(entity));
        }

        var nuevaCita = entity with { Id = 0, CreateAt = DateTime.Now,  UpdateAt = null, IsDelete = false };
        var nuevaCitaEntity = entity.ToEntity();
        try {
            var sql = @" 
                INSERT INTO Cita (Id, Matricula, Marca, Modelo, Cilindrada, Motor, DniDueño, FechaMatriculacion, FechaInspeccion, CreateAt, UpdateAt, IsDelete)
                VALUES (@Id, @Matricula, @Marca, @Modelo, @Cilindrada, @Motor, @DniDueño, @FechaMatriculacion, @FechaInspeccion, @CreateAt, @UpdateAt, @IsDelete)
                Select last_insert_rowid()";
            var entidad = _connection.Query<CitaEntity>(sql, new {
                Id = nuevaCitaEntity.Id,
                Matricula = nuevaCitaEntity.Matricula,
                Marca = nuevaCitaEntity.Marca,
                Modelo = nuevaCitaEntity.Modelo,
                Cilindrada = nuevaCitaEntity.Cilindrada,
                Motor = nuevaCitaEntity.Motor,
                DniDueño = nuevaCitaEntity.DniDueño,
                FechaMatriculacion = nuevaCitaEntity.FechaMatriculacion,
                FechaInspeccion = nuevaCitaEntity.FechaInspeccion,
                CreateAt = nuevaCitaEntity.CreateAt,
                UpdateAt = nuevaCitaEntity.UpdateAt,
                IsDelete = nuevaCitaEntity.IsDelete,
            });
            var id = _connection.ExecuteScalar<int>(sql);
            return Result.Success<Cita, DomainError>(GetById(id).Value);
        } catch (Exception e) {
            _logger.Error($"Error en la creacion de la nueva cita, mensaje de error: {e.Message}.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.CreationError());
        }
    }

    /// <inheritdoc cref="ICitaRepository.Update" />
    public Result<Cita, DomainError> Update(int id, Cita entity) {
        throw new NotImplementedException();
    }

    /// <inheritdoc cref="ICitaRepository.Delete" />
    public Result<Cita, DomainError> Delete(int id) {
        throw new NotImplementedException();
    }

    /// <inheritdoc cref="ICitaRepository.DeleteHard" />
    Result<Cita, DomainError> ICitaRepository.DeleteHard(int id) {
        throw new NotImplementedException();
    }

    /// <inheritdoc cref="ICitaRepository.DeleteAll" />
    public bool DeleteAll() {
        throw new NotImplementedException();
    }
    
    private void Seed() {
        foreach (var c in CitasFactory.Seed()) Create(c);
    }
    
    private bool VerificacionDniDueño(Cita cita) {
        try {
            var sql = "SELECT COUNT(*) FROM Cita WHERE DniDueño LIKE @DniDueño AND FechaMAtriculacion LIKE @FechaMatriculacion";
            return _connection.ExecuteScalar<int>(sql, new { DniDueño = cita.DniDueño, FechaMatriculacion = cita.FechaMatriculacion}) < 3;
        } catch (Exception e) {
            _logger.Error($"Error verificando la condicion del dni del dueño, mensaje: {e.Message}.");
            return false;
        }
    }
    
    private bool VerificacionMatricula(Cita cita) {
        try {
            var sql = "SELECT COUNT(*) FROM Cita WHERE Matricula LIKE @Matricula AND FechaMatriculacion LIKE @FechaMatriculacion";
            return _connection.ExecuteScalar<int>(sql, new { Matricula = cita.Matricula, FechaMatriculacion = cita.FechaMatriculacion}) < 0;
        } catch (Exception e) {
            _logger.Error($"Error verificando la condicion de la matricula, mensaje: {e.Message}.");
            return false;
        }
    }
}