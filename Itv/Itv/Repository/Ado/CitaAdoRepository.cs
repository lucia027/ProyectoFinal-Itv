using CSharpFunctionalExtensions;
using Itv.Config;
using Itv.Entity;
using Itv.Enums;
using Itv.Errors;
using Itv.Errors.Common;
using Itv.Factory;
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
        if (seedData) foreach (var v in CitasFactory.Seed()) Create(v);
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

    public IEnumerable<Cita> GetAll(int pagina, int tamPagina, bool isDeleteInclude, string campoBusqueda) {
        var entidades = new List<Cita>();
        
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
            .OrderBy(c => c.Id)
            .Skip((pagina -1) * tamPagina)
            .Take(tamPagina);
    }

    public Result<Cita, DomainError> GetById(int id) {
        using var connection = CreateConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Citas WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        
        using var reader = command.ExecuteReader();
        if (!reader.Read()) return Result.Failure<Cita, DomainError>(RepositoryErrors.IdNotFound(id));
        return Result.Success<Cita, DomainError>(MapCita(reader));
    }

    public Result<Cita, DomainError> Create(Cita entity) {
        if()
        
    }

    public Result<Cita, DomainError> Update(int id, Cita entity) {
        throw new NotImplementedException();
    }

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
        return Result.Success<Cita, DomainError>(MapCita(reader));
    }

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
        
        return Result.Success<Cita, DomainError>(eliminado);    
    }

     public bool DeleteAll() {
         using var connection = CreateConnection();
         connection.Open();
        
         using var command = connection.CreateCommand();
         command.CommandText = "DELETE FROM Citas";
         
         return command.ExecuteNonQuery() <= 0;
     }
     
    private Cita MapCita(SqliteDataReader reader) {
        return new Cita {
            Id = reader.GetInt32(0),
            Matricula = reader.GetString(1),
            Marca = reader.GetString(2),
            Modelo = reader.GetString(3), 
            Cilindrada = reader.GetInt32(4),
            Motor = Enum.TryParse(reader.GetString(5), out Motor m) ? m : Motor.Diesel,
            DniDueño = reader.GetString(6),
            FechaMatriculacion = DateTime.Parse(reader.GetString(7)),
            FechaInspeccion = DateTime.Parse(reader.GetString(8)),
            CreateAt = DateTime.Parse(reader.GetString(9)),
            UpdateAt = reader.IsDBNull(10) ? null : DateTime.Parse(reader.GetString(10)),
            IsDelete = reader.GetInt32(11) == 1
        };
    }
}