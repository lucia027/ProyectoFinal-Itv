using System.Windows;
using Itv.Wpf.ViewModels.Main;
using Microsoft.Extensions.DependencyInjection;

namespace Itv.Wpf.Views;

/// <summary>
/// Ventana principal de la aplicacion.
/// </summary>
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<MainViewModel>();
    }
}