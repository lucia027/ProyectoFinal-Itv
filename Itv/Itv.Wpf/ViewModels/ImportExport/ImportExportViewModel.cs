using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Itv.Config;
using Itv.Services.Citas;
using Itv.Services.ImportExport;
using Itv.Wpf.Dialog;
using Microsoft.Win32;
using Serilog;

namespace Itv.Wpf.ViewModels.ImportExport;

/// <summary>
/// ViewModel que gestiona la importacion y exportacion de citas del sistema, en un formato especificado en el appsettings.
/// </summary>
public partial class ImportExportViewModel(
    ICitaService citaService,
    IImportExportService importExportService,
    IDialogService dialogService
    ) : ObservableObject {
    private readonly ICitaService _citaService = citaService;
    private readonly IDialogService _dialogService = dialogService;
    private readonly IImportExportService _importExportService = importExportService;
    private readonly ILogger _logger = Log.ForContext<ImportExportViewModel>();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private bool _sustituirDatos;

    public string FormatoActual => Configuracion.StorageType switch {
        "csv" => "CSV",
        "json" => "JSON",
        "xml" => "XML",
        "bin" => "BINARIO",
        _ => "JSON"
    };

    private string FiltroArchivo => Configuracion.StorageType switch {
        "csv" => "CSV|*.csv",
        "json" => "JSON|*.json",
        "xml" => "XML|*.xml",
        "bin" => "Binario|*.bin",
        _ => "JSON|*.json"
    };


    /// <summary>
    /// Exporta las citas en el formato configurado en appsettings.json.
    /// </summary>
    [RelayCommand]
    private void Exportar() {
        try {
            IsLoading = true;
            StatusMessage = $"Exportando los datos del sistema...";

            var dialog = new SaveFileDialog {
                Filter = FiltroArchivo,
                FileName = $"{FormatoActual}_Citas_{DateTime.Now:yyyyMMdd}"
            };

            if (dialog.ShowDialog() == true) {
                var citas = _citaService.GetAll(1, 1000).ToList();
                var path = Path.Combine(Configuracion.DataFolder, $"citas.{Configuracion.StorageType}");
                var result = _importExportService.ExportarDatos(citas, path);

                if (result.IsSuccess) {
                    File.Copy(path, dialog.FileName, true);
                    StatusMessage = $"Se han exportado {result.Value} registros de citas.";
                    _dialogService.ShowSuccess($"Se han exportado {result.Value} registros de citas.");
                } else {
                    _dialogService.ShowError(result.Error.Message);
                    StatusMessage = "Error al exportar";
                }
            }
        } catch (Exception e) {
            _logger.Error(e, "Error al exportar");
            StatusMessage = "Error al exportar";
        } finally {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Importa citas desde un archivo en el formato configurado en appsettings.json.
    /// </summary>
    [RelayCommand]
    private void Importar() {
        try {
            var dialog = new OpenFileDialog {
                Filter = FiltroArchivo,
                Title = $"Seleccionar archivo {FormatoActual}"
            };

            if (dialog.ShowDialog() != true) return;

            IsLoading = true;
            StatusMessage = $"Importando los datos del sistema...";

            if (SustituirDatos) _citaService.DeleteAll();

            var result = _importExportService.ImportarDatosSistema(dialog.FileName);
            if (result.IsSuccess) {
                var count = result.Value.Count();

                StatusMessage = $"Se han importado {count} registros de citas.";
                _dialogService.ShowSuccess($"Se han importado {count} registros de citas.");
            } else {
                _dialogService.ShowError(result.Error.Message, "Error al importar");
                StatusMessage = "Error al importar";
            }
        } catch (Exception e) {
            _logger.Error(e, "Error al importar");
            StatusMessage = "Error al importar";
        } finally {
            IsLoading = false;
        }
    }
}