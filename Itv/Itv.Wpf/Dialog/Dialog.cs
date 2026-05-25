using System.Windows;

namespace Itv.Wpf.Dialog;

/// <summary>
/// Gestiona los dialogos que se le muestra al usuario.
/// </summary>
public class DialogService : IDialogService {
    
    /// <inheritdoc cref="IDialogService.ShowError" />
    public void ShowError(string message, string title = "Error") {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    /// <inheritdoc cref="IDialogService.ShowSuccess" />
    public void ShowSuccess(string message, string title = "Éxito") {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    /// <inheritdoc cref="IDialogService.ShowWarning" />
    public void ShowWarning(string message, string title = "Advertencia") {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    /// <inheritdoc cref="IDialogService.ShowInfo" />
    public void ShowInfo(string message, string title = "Información") {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    /// <inheritdoc cref="IDialogService.ShowConfirmation" />
    public bool ShowConfirmation(string message, string title = "Confirmar") {
        return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) 
               == MessageBoxResult.Yes;
    }
}