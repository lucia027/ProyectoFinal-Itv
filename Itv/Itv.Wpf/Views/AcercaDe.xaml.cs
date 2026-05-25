using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace Itv.Wpf.Views;

/// <summary>
/// Ventana de "Acerca De" de la aplicacion.
/// </summary>
public partial class AcercaDeWindow : Window {
    public AcercaDeWindow() {
        InitializeComponent();
    }

    /// <summary>
    /// Cierra la ventana.
    /// </summary>
    private void OnCerrarClick(object sender, RoutedEventArgs e) {
        Close();
    }

    /// <summary>
    /// Abre en enlace al repositorio en el navegador.
    /// </summary>
    private void OnGitHubClick(object sender, RequestNavigateEventArgs e) {
        try {
            Process.Start(new ProcessStartInfo {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });

            e.Handled = true;
        } catch (Exception exception) { }
    }
}