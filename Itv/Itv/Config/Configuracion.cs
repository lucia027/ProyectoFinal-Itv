using System;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Itv.Config;

/// <summary>
/// Clase de configuracion que se encarga de leer el appsettings.json
/// </summary>
public static class Configuracion {
    
    static Configuracion() {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json",  false,  true)
            .Build();
    }

    public static IConfiguration Configuration { get; }

    public static CultureInfo Locale => CultureInfo.GetCultureInfo("es-ES");

    public static string DataFolder => Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        Configuration.GetValue<string>("Repository:Directory") ?? "data");
    
    public static string ConnectionString => Configuration.GetValue<string>("Repository:ConnectionString") ?? "Data Source=data/itv.db";
    
    public static string StorageType => Configuration.GetValue<string>("Storage:Type") ?? "json";

    public static string RepositoryType {
        get {
            var type = Configuration.GetValue<string>("Repository:Type") ?? "memory";

            return type.ToLower() switch {
                "memory" => "memory",
                "binary" => "binary",
                "json" => "json",  
                "dapper" => "dapper",  
                "ado" => "ado",  
                "efcore" => "efcore",
                _ => "memory"
            };
        }
    }
    
    public static string ItvFile {
        get {
            var extension = StorageType.ToLower() switch {
                "json" => "json",
                "xml" => "xml",
                "csv" => "csv",
                "bin" => "bin",
                _ => "json"
            };
            return Path.Combine(DataFolder, $"itv.{extension}");
        }
    }
    
    public static int CacheSize => Configuration.GetValue("Cache:Size", 10);
    
    public static string BackupDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Configuration.GetValue<string>("Backup:Directory") ?? "backup");
    
    public static string BackupFormat {
        get {
            var format = Configuration.GetValue<string>("Backup:Format") ?? "json";
            return format.ToLower() switch {
                "json" => "json",
                "xml" => "xml",
                "csv" => "csv",
                "bin" => "bin",
                _ => "json"
            };
        }
    }

    public static bool DropData => Configuration.GetValue("Repository:DropData", false);
    
    public static bool SeedData => Configuration.GetValue("Repository:SeedData", true);

}