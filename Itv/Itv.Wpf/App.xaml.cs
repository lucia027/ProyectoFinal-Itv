using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Itv.Config;
using Itv.Wpf.Infraestructure;
using Serilog;
using Serilog.Debugging;

namespace Itv.Wpf;

public partial class App : Application {
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e) {
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        ConfigureSerilog();
        Log.Information("Iniciando aplicacion..");

        Services = FrontDependenciesProvider.BuildServiceProvider();
        Log.Information("Servicios creados todos con exito.");

        ConfigureExceptionHandling();

        base.OnStartup(e);
    }
    
    /// <summary>
    /// Configura Serilog leyendo la configuracion de appsettings.json
    /// </summary>
    private void ConfigureSerilog() {
        SelfLog.Enable(msg => Debug.WriteLine($"SERILOG DIAG: {msg}"));
        
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuracion.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        Log.Information("Serilog inicializado desde JSON");
    }

    /// <summary>
    /// Configura los manejadores de excepciones no controladas
    /// </summary>
    private void ConfigureExceptionHandling() {
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
        Log.Fatal(e.Exception, "Excepcion no manejada en el hilo de UI");

        MessageBox.Show(
            $"Error: {e.Exception.Message}",
            "Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);

        e.Handled = true;
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e) {
        Log.Fatal(e.ExceptionObject as Exception, "Excepcion no manejada en la aplicacion");
    }

    private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e) {
        Log.Error(e.Exception, "Excepcion en tarea asincrona");
        e.SetObserved();
    }

    protected override void OnExit(ExitEventArgs e) {
        Log.Information("Aplicacion cerrandose");

        Log.CloseAndFlush();

        if (Services is IDisposable disposable) {
            disposable.Dispose();
        }

        base.OnExit(e);
    }
}