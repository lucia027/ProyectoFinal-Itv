using CSharpFunctionalExtensions;
using Itv.Config;
using Itv.Entity;
using Itv.Errors;
using Itv.Errors.Common;
using Itv.Factory;
using Itv.Mappers;
using Itv.Models;
using Itv.Repository.Common;
using Microsoft.Data.Sqlite;
using Serilog;

namespace Itv.Repository.Ado;

/// <summary>
/// Repositorio en Ado.Net para gestionar las citas.
/// </summary>
public class CitaAdoRepository : ICitaRepository {
    
    private ILogger _logger = Log.ForContext<CitaAdoRepository>();
    private readonly string _connectionString;

    public CitaAdoRepository() : this(Configuracion.DropData, Configuracion.SeedData) { }
    
    public CitaAdoRepository(bool dropData, bool seedData) {
        _logger.Debug("Iniciando el repositorio en ado.");
        _connectionString = Configuracion.ConnectionString;
        EnsureDataFoder();
        EnsureTable();
        
        if (dropData) DeleteAll();
        if (dropData || seedData) foreach (var v in CitasFactory.Seed()) Create(v);
    }
    
    private SqliteConnection CreateConnection() => new(_connectionString);

    private void EnsureDataFoder() {
        if(Directory.Exists(Configuracion.DataFolder)) {
            _logger.Warning("No existe el directorio data, creando..");
            return;
        }
        Directory.CreateDirectory(Configuracion.DataFolder);
    }

    private void EnsureTable() {
        using var connection = CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            DROP TABLE IF EXISTS Citas;
            CREATE TABLE Citas (
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
                IsDeleted INTEGER NOT NULL DEFAULT 0,
            )";
        command.ExecuteNonQuery();
    }

    /// <inheritdoc cref="ICitaRepository.GetAll" />
    public IEnumerable<Cita> GetAll(int pagina, int tamPagina, bool isDeleteInclude, string campoBusqueda) {
        var entidades = new List<CitaEntity>();
        
        using var connection = CreateConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        if(!isDeleteInclude)  command.CommandText = @"
                SELECT * FROM Citas WHERE IsDeleted = 0 
                AND Matricula LIKE '%@CampoBusqueda%' 
                OR Marca LIkE '%@CampoBusqueda%'
                OR Modelo LIKE '%@CampoBusqueda%'
                OR Cilindrada LIKE '%@CampoBusqueda%'
                OR Motor LIKE '%@CampoBusqueda%'
                OR DniDueño LIKE '%@CampoBusqueda%'
                ";
        command.Parameters.AddWithValue("@CampoBusqueda", campoBusqueda);
        
        if(isDeleteInclude) command.CommandText =@"
                SELECT * FROM Citas WHERE IsDeleted = 1
                AND Matricula LIKE '%@CampoBusqueda%' 
                OR Marca LIkE '%@CampoBusqueda%'
                OR Modelo LIKE '%@CampoBusqueda%'
                OR Cilindrada LIKE '%@CampoBusqueda%'
                OR Motor LIKE '%@CampoBusqueda%'
                OR DniDueño LIKE '%@CampoBusqueda%'
                ";
        command.Parameters.AddWithValue("@CampoBusqueda", campoBusqueda);
        
        using var reader = command.ExecuteReader();
        while (reader.Read()) {
            entidades.Add(MapCita(reader));
        }

        return entidades
            .Select(c => c.ToModel())
            .OrderBy(c => c.Matricula)
            .Skip((pagina -1) * tamPagina)
            .Take(tamPagina);
    }

    /// <inheritdoc cref="ICitaRepository.GetById" />
    public Result<Cita, DomainError> GetById(int id) {
        using var connection = CreateConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Citas WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        
        using var reader = command.ExecuteReader();
        if (!reader.Read()) return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
        return Result.Success<Cita, DomainError>(MapCita(reader).ToModel());
    }
    
    /// <inheritdoc cref="ICitaRepository.GetByDateMatricula" />
    public Result<IEnumerable<Cita>, DomainError> GetByDateMatricula(DateTime inicio, DateTime? fin, bool isDeleteInclude = true) {
        if(fin == null) fin = DateTime.Now;
        List<Cita> citas = [];
        
        using var connection = CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        
        if (!isDeleteInclude) {
            command.CommandText = "SELECT * FROM Citas WHERE FechaMatricula BETWEEN @inicio AND @fin AND IsDelete LIKE 0";
            command.Parameters.AddWithValue("@inicio", inicio);
            command.Parameters.AddWithValue("@fin", fin);
        }
        
        if (isDeleteInclude) {
            command.CommandText = "SELECT * FROM Citas WHERE FechaMatricula BETWEEN @inicio AND @fin AND IsDelete LIKE 1";
            command.Parameters.AddWithValue("@inicio", inicio);
            command.Parameters.AddWithValue("@fin", fin);
        }
        
        using var reader = command.ExecuteReader();
        while (reader.Read()) {
            citas.Add(MapCita(reader).ToModel());
        }

        citas = citas.OrderBy(c => c.Matricula).ToList();

        if (!citas.Any()) return Result.Failure<IEnumerable<Cita>, DomainError>(RepositoryErrors.NotFoundCitasError());
        return Result.Success<IEnumerable<Cita>, DomainError>(citas);      
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
        
        using var connection = CreateConnection();
        connection.Open();

        var nuevaCitaEntity = entity.ToEntity();
        nuevaCitaEntity = nuevaCitaEntity with { Id = 0, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };

        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Citas (Id, Matricula, Marca, Modelo, Cilindrada, Motor, DniDueño, FechaMatriculacion, FechaInspeccion, CreateAt, UpdateAt, IsDelete)
            VALUES (@Id, @Matricula, @Marca, @Modelo, @Cilindrada, @Motor, @DniDueño, @FechaMatriculacion, @FechaInspeccion, @CreateAt, @UpdateAt, @IsDelete)
            Select last_insert_rowid();";
        
        command.Parameters.AddWithValue("@Id", nuevaCitaEntity.Id);
        command.Parameters.AddWithValue("@Matricula", nuevaCitaEntity.Matricula);
        command.Parameters.AddWithValue("@Marca", nuevaCitaEntity.Marca);
        command.Parameters.AddWithValue("@Modelo", nuevaCitaEntity.Modelo);
        command.Parameters.AddWithValue("@Cilindrada", nuevaCitaEntity.Cilindrada);
        command.Parameters.AddWithValue("@Motor", nuevaCitaEntity.Motor);
        command.Parameters.AddWithValue("@DniDueño", nuevaCitaEntity.DniDueño);
        command.Parameters.AddWithValue("@FechaMatriculacion", nuevaCitaEntity.FechaMatriculacion);
        command.Parameters.AddWithValue("@FechaInspeccion", nuevaCitaEntity.FechaInspeccion);
        command.Parameters.AddWithValue("@CreateAt", nuevaCitaEntity.CreateAt);
        command.Parameters.AddWithValue("@UpdateAt", nuevaCitaEntity.UpdateAt);
        command.Parameters.AddWithValue("@IsDelete", nuevaCitaEntity.IsDelete);
        
        nuevaCitaEntity = nuevaCitaEntity with{ Id = Convert.ToInt32(command.ExecuteScalar())};
        return Result.Success<Cita, DomainError>(nuevaCitaEntity.ToModel());
    }

    /// <inheritdoc cref="ICitaRepository.Update" />
    public Result<Cita, DomainError> Update(int id, Cita entity) {
        var citaVieja = GetById(id);
        if (citaVieja.IsFailure) {
            _logger.Debug("No se ha podido actualizar la cita el id no existe.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
        }
        
        using var connection = CreateConnection();
        connection.Open();

        var nuevaCitaEntity = entity.ToEntity();
        nuevaCitaEntity = nuevaCitaEntity with {Id = id, UpdateAt = null, IsDelete = false };
        
        using var command =  connection.CreateCommand();
        command.CommandText = @"
            UPDATE Citas SET  Matricula = @Matricula, Marca = @Marca, Modelo = @Modelo, Cilindrada = @Cilindrada, Motor = @Motor, DniDueño = @DniDueño, FechaMatriculacion = @FechaMatriculacion, FechaInspeccion = @FechaInspeccion, CreateAt = @CreateAt, UpdateAt = @UpdateAt, IsDelete = @IsDelete
            Where Id = @Id
            ";

        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Matricula", nuevaCitaEntity.Matricula);
        command.Parameters.AddWithValue("@Marca", nuevaCitaEntity.Marca);
        command.Parameters.AddWithValue("@Modelo", nuevaCitaEntity.Modelo);
        command.Parameters.AddWithValue("@Cilindrada", nuevaCitaEntity.Cilindrada);
        command.Parameters.AddWithValue("@Motor", nuevaCitaEntity.Motor);
        command.Parameters.AddWithValue("@DniDueño", nuevaCitaEntity.DniDueño);
        command.Parameters.AddWithValue("@FechaMatriculacion", nuevaCitaEntity.FechaMatriculacion);
        command.Parameters.AddWithValue("@FechaInspeccion", nuevaCitaEntity.FechaInspeccion);
        command.Parameters.AddWithValue("@CreateAt", nuevaCitaEntity.CreateAt);
        command.Parameters.AddWithValue("@UpdateAt", nuevaCitaEntity.UpdateAt);
        command.Parameters.AddWithValue("@IsDelete", nuevaCitaEntity.IsDelete);
        
        command.ExecuteNonQuery();
        
        return Result.Success<Cita, DomainError>(nuevaCitaEntity.ToModel());
    }

    /// <inheritdoc cref="ICitaRepository.Delete" />
    public Result<Cita, DomainError> Delete(int id) {
        if (GetById(id).IsFailure) {
            _logger.Debug("No se ha podido eliminar la cita, el id no existe.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
        }
        
        using var connection = CreateConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Citas SET IsDelete = 1 WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        
        using var reader = command.ExecuteReader();
        return Result.Success<Cita, DomainError>(MapCita(reader).ToModel());
    }
    
    /// <inheritdoc cref="ICitaRepository.DeleteHard" />
    public Result<Cita, DomainError> DeleteHard(int id) {
        if (GetById(id).IsFailure) {
            _logger.Debug("No se ha podido eliminar la cita, el id no existe.");
            return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
        }
        
        using var connection = CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();

        using var reader = command.ExecuteReader();
        var eliminado = MapCita(reader);
        
        command.CommandText = "DELETE FROM Citas WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
        
        return Result.Success<Cita, DomainError>(eliminado.ToModel());    
    }

    /// <inheritdoc cref="ICitaRepository.DeleteAll" />
    public bool DeleteAll() {
         using var connection = CreateConnection();
         connection.Open();
        
         using var command = connection.CreateCommand();
         command.CommandText = "DELETE FROM Citas";
         
         return command.ExecuteNonQuery() <= 0;
     }
     
    private CitaEntity MapCita(SqliteDataReader reader) {
        return new CitaEntity {
            Id = reader.GetInt32(0),
            Matricula = reader.GetString(1),
            Marca = reader.GetString(2),
            Modelo = reader.GetString(3), 
            Cilindrada = reader.GetInt32(4),
            Motor = reader.GetString(5),
            DniDueño = reader.GetString(6),
            FechaMatriculacion = DateTime.Parse(reader.GetString(7)),
            FechaInspeccion = DateTime.Parse(reader.GetString(8)),
            CreateAt = DateTime.Parse(reader.GetString(9)),
            UpdateAt = reader.IsDBNull(10) ? null : DateTime.Parse(reader.GetString(10)),
            IsDelete = reader.GetInt32(11) == 1
        };
    }

    private bool VerificacionDniDueño(Cita entity) {
        using var connection = CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        
        command.CommandText = "SELECT COUNT(*) FROM Citas WHERE DniDueño LIKE @DniDueño AND FechaMAtriculacion LIKE @FechaMatriculacion";
        command.Parameters.AddWithValue("@DniDueño", entity.DniDueño);
        command.Parameters.AddWithValue("@FechaMatriculacion", entity.FechaMatriculacion);

        if(Convert.ToInt32(command.ExecuteScalar()) >= 3) {
            return false;
        }
        return true;
    }
    
    private bool VerificacionMatricula(Cita entity) {
        using var connection = CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        
        command.CommandText = "SELECT COUNT(*) FROM Citas WHERE Matricula LIKE @Matricula AND FechaMAtriculacion LIKE @FechaMatriculacion";
        command.Parameters.AddWithValue("@Matricula", entity.Matricula);
        command.Parameters.AddWithValue("@FechaMatriculacion", entity.FechaMatriculacion);    

        if(Convert.ToInt32(command.ExecuteScalar()) >= 1) {
            return false;
        }
        return true;
    }
}