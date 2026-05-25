namespace Itv.Wpf.Dialog;

/// <summary>
/// Define el contrato para mostrar diálogos especificos al usuario.
/// </summary>
public interface IDialogService {
    /// <summary>
    /// Muestra un diálogo de error.
    /// </summary>
    /// <param name="message">Mensaje de error.</param>
    /// <param name="title">Título de la ventana, por defecto será "Error".</param>
    void ShowError(string message, string title = "Error");

    /// <summary>
    /// Muestra un diálogo de exito.
    /// </summary>
    /// <param name="message">Mensaje de exito.</param>
    /// <param name="title">Título de la ventana, por defecto será "Exito".</param>
    void ShowSuccess(string message, string title = "Éxito");

    /// <summary>
    /// Muestra un diálogo de advertencia.
    /// </summary>
    /// <param name="message">Mensaje de advertencia.</param>
    /// <param name="title">Título de la ventana, por defecto será "Advertencia".</param>
    void ShowWarning(string message, string title = "Advertencia");

    /// <summary>
    /// Muestra un diálogo de información.
    /// </summary>
    /// <param name="message">Mensaje de información.</param>
    /// <param name="title">Título de la ventana, por defecto será "Información".</param>
    void ShowInfo(string message, string title = "Información");

    /// <summary>
    /// Muestra un diálogo de confirmacion.
    /// </summary>
    /// <param name="message">Mensaje de confirmacion.</param>
    /// <param name="title">Título de la ventana, por defecto será "Confirmar".</param>
    bool ShowConfirmation(string message, string title = "Confirmar");
}