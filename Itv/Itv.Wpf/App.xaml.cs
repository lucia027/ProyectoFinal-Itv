using System.Windows;
using Itv.Wpf.Infraestructure;

namespace Itv.Wpf;

public partial class App : Application {
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);

        Services = FrontDependenciesProvider.BuildServiceProvider();
    }
}