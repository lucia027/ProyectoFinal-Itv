using Itv.Wpf.Dialog;
using Itv.Wpf.ViewModels.Cita;
using Itv.Wpf.ViewModels.ImportExport;
using Itv.Wpf.ViewModels.Informe;
using Itv.Wpf.ViewModels.Main;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Itv.Wpf.Infraestructure;

/// <summary>
/// Proveedor de dependencias para el Frontend WPF.
/// Extiende el Back con los ViewModels específicos de presentación.
/// </summary>
public static class FrontDependenciesProvider {
    /// <summary>
    /// Construye el proveedor de servicios combinando Back + Front.
    /// El Back se extiende con los ViewModels del Front mediante callback.
    /// </summary>
    /// <returns>Proveedor de servicios con todos los servicios registrados.</returns>
    public static IServiceProvider BuildServiceProvider() {
        Log.Information("Configurando servicios (Back + Front)...");

        var serviceProvider = Itv.Infraestructure.DependenciesProvider.BuildServiceProvider(services => {
            RegisterViewModels(services);
            Log.Information("ViewModels registradas desde Front");
        });

        Log.Information("Servicios configurados correctamente");

        return serviceProvider;
    }

    /// <summary>
    /// Registra todos los ViewModels del Front como servicios.
    /// </summary>
    private static void RegisterViewModels(IServiceCollection services) {
        services.AddTransient<MainViewModel>();
        services.AddTransient<CitaViewModel>();
        services.AddTransient<InformeViewModel>();
        services.AddTransient<ImportExportViewModel>();
        services.AddSingleton<IDialogService, DialogService>();
    }
}