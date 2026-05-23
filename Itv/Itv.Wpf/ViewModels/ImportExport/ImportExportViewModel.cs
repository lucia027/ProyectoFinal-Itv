using System.Reflection;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Itv.Config;
using Itv.Services.Citas;
using Itv.Services.ImportExport;
using Microsoft.Win32;

namespace Itv.Wpf.ViewModels.ImportExport;

public partial class ImportExportViewModel : ObservableObject {
    private readonly ICitaService _citaService;
    private readonly IImportExportService _importExportService;

    public ImportExportViewModel(
        ICitaService citaService,
        IImportExportService importExportService
    ) {
        _citaService = citaService;
        _importExportService = importExportService;
    }

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = "Listo";

    [ObservableProperty]
    private bool _sustituirDatos;

    public string TipoFormatoActual => ObtenerFormatoNormalizado() switch {
        "csv" => "CSV",
        "json" => "JSON",
        "xml" => "XML",
        "binario" => "BINARIO",
        _ => "FORMATO CONFIGURADO"
    };

    public string ExtensionActual => ObtenerFormatoNormalizado() switch {
        "csv" => "*.csv",
        "json" => "*.json",
        "xml" => "*.xml",
        "binario" => "*.bin",
        _ => "*.*"
    };

    public string DescripcionFormatoActual => ObtenerFormatoNormalizado() switch {
        "csv" => "Se importarán o exportarán citas usando archivos CSV.",
        "json" => "Se importarán o exportarán citas usando archivos JSON.",
        "xml" => "Se importarán o exportarán citas usando archivos XML.",
        "binario" => "Se importarán o exportarán citas usando archivo binario.",
        _ => "El formato será el que esté configurado actualmente en appsettings.json."
    };

    private string FiltroDialogo => ObtenerFormatoNormalizado() switch {
        "csv" => "CSV|*.csv",
        "json" => "JSON|*.json",
        "xml" => "XML|*.xml",
        "binario" => "Binario|*.bin",
        _ => "Todos los archivos|*.*"
    };

    private string ExtensionSinPunto => ObtenerFormatoNormalizado() switch {
        "csv" => "csv",
        "json" => "json",
        "xml" => "xml",
        "binario" => "bin",
        _ => "dat"
    };

    [RelayCommand]
    private void Exportar() {
        try {
            IsLoading = true;
            StatusMessage = $"Exportando datos en formato {TipoFormatoActual}...";

            var dialog = new SaveFileDialog {
                Filter = FiltroDialogo,
                FileName = $"Citas_{DateTime.Now:yyyyMMdd_HHmmss}.{ExtensionSinPunto}"
            };

            if (dialog.ShowDialog() != true) {
                StatusMessage = "Exportación cancelada";
                return;
            }

            var citas = _citaService.GetAll(1, int.MaxValue).ToList();

            /*
             * El servicio decide internamente si exporta en CSV, JSON, XML o binario
             * según appsettings.json.
             */
            var result = _importExportService.ExportarDatos(citas, dialog.FileName);

            if (result.IsSuccess) {
                MessageBox.Show(
                    $"Exportación completada correctamente.\n\nFormato: {TipoFormatoActual}\nRegistros exportados: {result.Value}",
                    "Exportar datos",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                StatusMessage = $"Exportados {result.Value} registros en formato {TipoFormatoActual}.";
            }
            else {
                MessageBox.Show(
                    result.Error.Message,
                    "Error al exportar",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                StatusMessage = "Error al exportar datos.";
            }
        }
        catch (Exception ex) {
            MessageBox.Show(
                $"Error al exportar datos: {ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            StatusMessage = "Error al exportar datos.";
        }
        finally {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void Importar() {
        try {
            var dialog = new OpenFileDialog {
                Filter = FiltroDialogo,
                Title = $"Seleccionar archivo {TipoFormatoActual}"
            };

            if (dialog.ShowDialog() != true) {
                StatusMessage = "Importación cancelada";
                return;
            }

            IsLoading = true;
            StatusMessage = $"Importando datos desde formato {TipoFormatoActual}...";

            if (SustituirDatos) {
                var confirmacion = MessageBox.Show(
                    "Has marcado sustituir datos actuales.\n\nAntes de importar se intentarán eliminar los registros existentes.\n\n¿Quieres continuar?",
                    "Confirmar importación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (confirmacion != MessageBoxResult.Yes) {
                    StatusMessage = "Importación cancelada.";
                    return;
                }

                EliminarDatosActualesSiExiste();
            }

            /*
             * El servicio decide internamente si importa CSV, JSON, XML o binario
             * según appsettings.json.
             */
            var result = _importExportService.ImportarDatosSistema(dialog.FileName);

            if (result.IsSuccess) {
                var cantidad = result.Value?.Count() ?? 0;

                MessageBox.Show(
                    $"Importación completada correctamente.\n\nFormato: {TipoFormatoActual}\nRegistros importados: {cantidad}",
                    "Importar datos",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                StatusMessage = $"Importados {cantidad} registros desde formato {TipoFormatoActual}.";
            }
            else {
                MessageBox.Show(
                    result.Error.Message,
                    "Error al importar",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                StatusMessage = "Error al importar datos.";
            }
        }
        catch (Exception ex) {
            MessageBox.Show(
                $"Error al importar datos: {ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            StatusMessage = "Error al importar datos.";
        }
        finally {
            IsLoading = false;
        }
    }

    private void EliminarDatosActualesSiExiste() {
        /*
         * Lo hago por reflexión para que no te rompa el proyecto
         * si tu ICitaService no tiene DeleteAll().
         *
         * Si tu servicio sí tiene DeleteAll(), se ejecutará.
         * Si no lo tiene, avisa y continúa sin borrar.
         */

        var method = _citaService.GetType().GetMethod("DeleteAll");

        if (method == null) {
            MessageBox.Show(
                "No se ha encontrado el método DeleteAll() en ICitaService.\n\nLa importación continuará sin eliminar los datos anteriores.",
                "Aviso",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            return;
        }

        method.Invoke(_citaService, null);
    }

    private static string ObtenerFormatoNormalizado() {
        var valor = LeerFormatoDesdeConfiguracion();

        if (string.IsNullOrWhiteSpace(valor)) {
            return "desconocido";
        }

        valor = valor.Trim().ToLower();

        return valor switch {
            "csv" => "csv",
            "json" => "json",
            "xml" => "xml",
            "binary" => "binario",
            "bin" => "binario",
            "binario" => "binario",
            "bson" => "binario",
            _ => valor
        };
    }

    private static string LeerFormatoDesdeConfiguracion() {
        /*
         * Como no sé el nombre exacto de tu propiedad en Configuracion,
         * busco varios nombres típicos.
         *
         * Si sabes el nombre exacto, puedes simplificar esto.
         */

        var posiblesNombres = new[] {
            "ImportExportType",
            "ImportExportFormat",
            "ExportType",
            "ExportFormat",
            "StorageType",
            "FileType",
            "FileFormat",
            "TipoExportacion",
            "TipoImportExport",
            "Formato"
        };

        var tipoConfiguracion = typeof(Configuracion);

        foreach (var nombre in posiblesNombres) {
            var propiedad = tipoConfiguracion.GetProperty(
                nombre,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase
            );

            var valorPropiedad = propiedad?.GetValue(null)?.ToString();

            if (!string.IsNullOrWhiteSpace(valorPropiedad)) {
                return valorPropiedad;
            }

            var campo = tipoConfiguracion.GetField(
                nombre,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase
            );

            var valorCampo = campo?.GetValue(null)?.ToString();

            if (!string.IsNullOrWhiteSpace(valorCampo)) {
                return valorCampo;
            }
        }

        return string.Empty;
    }
}