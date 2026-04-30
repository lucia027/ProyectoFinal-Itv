using CSharpFunctionalExtensions;
using Itv.Config;
using Itv.Entity;
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

    private void EnsureTable()
    {
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

    public IEnumerable<Cita> GetAll(int pagina = 1, int tamPagina = 5, bool isDeleteInclude = true) {
        var entidades = new List<CitaEntity>();
        
        using var connection = CreateConnection();
        connection.Open();
        
        using var command = connection.CreateCommand();
        if(!isDeleteInclude)  command.CommandText = "SELECT * FROM Citas WHERE IsDeleted = 0";
        if(isDeleteInclude) command.CommandText = "SELECT * FROM Citas WHERE IsDeleted = 1";
        
        using var reader = command.ExecuteReader();
        while (reader.Read()) {
            entidades.Add(new CitaEntity());
        }
        
        return entidades
    }

    public Result<Cita, DomainError> GetById(int id) {
        throw new NotImplementedException();
    }

    public Result<Cita, DomainError> Create(Cita entity) {
        throw new NotImplementedException();
    }

    public Result<Cita, DomainError> Update(int id, Cita entity) {
        throw new NotImplementedException();
    }

    public Result<Cita, DomainError> Delete(int id) {
        throw new NotImplementedException();
    }

    public Result<Cita, DomainError> DeleteHard(int id) {
        throw new NotImplementedException();
    }

     public bool DeleteAll() {
        throw new NotImplementedException();
    }

    Result<Cita, DomainError> ICrud_Repository<int, Cita>.DeleteHard(int id) {
        throw new NotImplementedException();
    }

    bool ICrud_Repository<int, Cita>.DeleteAll() {
        throw new NotImplementedException();
    }
}