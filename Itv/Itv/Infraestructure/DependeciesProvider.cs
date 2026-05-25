using Itv.Cache;
using Itv.Config;
using Itv.Entity;
using Itv.Models;
using Itv.Repository.Common;
using Itv.Repository.Dapper;
using Itv.Repository.Efc;
using Itv.Repository.Memory;
using Itv.Services.Citas;
using Itv.Services.ImportExport;
using Itv.Services.Report;
using Itv.Storage.Binary;
using Itv.Storage.Common;
using Itv.Storage.Csv;
using Itv.Storage.Json;
using Itv.Storage.Xml;
using Itv.Validator;
using Itv.Validator.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace Itv.Infraestructure;

/// <summary>
/// Proveedor de dependencias centralizado, configura la inyeccion de dependencias para toda la aplicacion.
/// </summary>
public static class DependenciesProvider {
    
    /// <summary>
    /// Configura el contenedor de inyeccion de dependencias.
    /// </summary>
    /// <param name="configureAdditional">Para registrar servicios adiccionales, es opcional.</param>
    /// <returns>Proveedor de serviciosya configurado.</returns>
    public static IServiceProvider BuildServiceProvider(Action<IServiceCollection>? configureAdditional = null) {
        var services = new ServiceCollection();

        CleanData();

        RegisterCaches(services);
        RegisterValidators(services);
        RegisterStorages(services);
        RegisterRepositories(services);
        RegisterServices(services);
        
        configureAdditional?.Invoke(services);
        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Registra el tipo de storage del sistema según el appsettings.json
    /// </summary>
    private static void RegisterStorages(IServiceCollection services) {
        services.AddTransient<IStorage<Cita>>(sp => {
            var storageType = Configuracion.StorageType.ToLower();
            return storageType switch {
                "json" => new CitaJsonStorage(),
                "csv" => new CitaCsvStorage(),
                "binary" or "bin" => new CitaBinStorage(),
                "xml" => new CitaXmlStorage(),
                _ => new CitaJsonStorage()
            };
        });
    }

    /// <summary>
    /// Registra el tipo de repositorio del sistema según el appsettings.json
    /// </summary>
    /// <param name="services"></param>
    private static void RegisterRepositories(IServiceCollection services) {
        services.AddSingleton<ICitaRepository>(sp => {
            var repoType = Configuracion.RepositoryType.ToLower();
            return repoType switch {
                "memory" => new CitaMemoryRepository(Configuracion.DropData, Configuracion.SeedData),
                "ado" => CreateDapperRepository(Configuracion.DropData, Configuracion.SeedData),
                "dapper" => CreateDapperRepository(Configuracion.DropData, Configuracion.SeedData),
                "efcore" => CreateEfRepository(Configuracion.DropData, Configuracion.SeedData),
                _ => new CitaMemoryRepository(Configuracion.DropData, Configuracion.SeedData)
            };
        });
    }

    /// <summary>
    /// Crea el repositorio en Dapper con la conexion a SQLite.
    /// </summary>
    private static CitaDapperRepository CreateDapperRepository(bool dropData, bool seedData) {
        var dataFolder = Configuracion.DataFolder;
        if (!Directory.Exists(dataFolder)) Directory.CreateDirectory(dataFolder);

        var dbPath = Path.Combine(dataFolder, "gestionitv.db");
        var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();
        return new CitaDapperRepository(connection, connection.Close, dropData, seedData);
    }

    /// <summary>
    /// Crea el repositorio en Entity Framework Core con la conexion a SQLite.
    /// </summary>
    private static CitaEfcRepository CreateEfRepository(bool dropData, bool seedData) {
        var dataFolder = Configuracion.DataFolder;
        if (!Directory.Exists(dataFolder))
            Directory.CreateDirectory(dataFolder);
        
        var dbPath = Path.Combine(dataFolder, "gestionitv");
        var context = new AppDbContext($"Data Source={dbPath}");
        
        return new CitaEfcRepository(context, dropData, seedData);
    }

    /// <summary>
    /// Registra el validador de citas.
    /// </summary>
    private static void RegisterValidators(IServiceCollection services) {
        services.AddTransient<IValidator<Cita>, CitaValidator>();
    }

    /// <summary>
    /// Registra la cache del sistema.
    /// </summary>
    private static void RegisterCaches(IServiceCollection services) {
        services.AddSingleton<ICache<int, Cita>>(sp =>
            new CacheLru<int, Cita>(Configuracion.CacheSize));
    }

    /// <summary>
    /// Registra todos los servicios del sistema.
    /// </summary>
    private static void RegisterServices(IServiceCollection services) {
        services.AddTransient<IReportService, ReportService>(
            sp => new ReportService(Configuracion.ReportDirectory));
    
        services.AddTransient<IImportExportService, ImportExportService>();
    
        services.AddScoped<ICitaService, CitaService>(cs =>
            new CitaService(
                cs.GetRequiredService<IValidator<Cita>>(),
                cs.GetRequiredService<ICitaRepository>(),
                cs.GetRequiredService<ICache<int, Cita>>()
                ));
    }

    /// <summary>
    /// Impia el directorio de informes.
    /// </summary>
    private static void CleanData() {
        if (Configuracion.DropData || Configuracion.SeedData) {
            CleanDirectory(Configuracion.ReportDirectory);
        }
    }

    /// <summary>
    /// Limpia un directorio eliminado.
    /// </summary>
    private static void CleanDirectory(string path) {
        try {
            if (Directory.Exists(path)) {
                foreach (var file in Directory.GetFiles(path)) 
                    try {
                        File.Delete(path);
                    } catch { }

                foreach (var dir in Directory.GetDirectories(path)) 
                    try {
                        Directory.Delete(dir, true);
                    } catch { }
                Directory.CreateDirectory(path);
            }
        } catch (Exception ex) {
            Console.WriteLine($"No se ha podido limpiar el directorio {path}: {ex.Message}");
        }
    }
}