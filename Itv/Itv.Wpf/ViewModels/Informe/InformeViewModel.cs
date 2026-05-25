using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Itv.Services.Citas;
using Itv.Services.Report;
using Itv.Wpf.Dialog;
using Microsoft.Win32;
using Serilog;
namespace Itv.Wpf.ViewModels.Informe;

/// <summary>
/// ViewModel que gestiona la generacion de informes.
/// </summary>
public partial class InformeViewModel(
    ICitaService citaService,
    IReportService reportService,
    IDialogService dialogService
    ) : ObservableObject {

    private readonly ILogger _logger = Log.ForContext<InformeViewModel>();
    private readonly ICitaService _citaService = citaService;
    private readonly IReportService _reportService = reportService;
    private readonly IDialogService _dialogService = dialogService;
    
    [ObservableProperty]
    private bool _isGenerating;

    [ObservableProperty]
    private bool _incluirEliminadas;

    [ObservableProperty]
    private string _statusMessage = "";

    [RelayCommand]
    private void GenerarInformeHtml() {
        try {
            IsGenerating = true;
            StatusMessage = "Generando informe HTML...";

            var citas = _citaService.GetAll(1, int.MaxValue).ToList();
            if (!IncluirEliminadas) citas = citas.Where(c => !c.IsDelete).ToList();

            var informeHtml = _reportService.GenerarInformeHtml(citas);
            if (informeHtml.IsFailure) {
                _dialogService.ShowError(informeHtml.Error.Message);
                return;
            }

            var saveDialog = new SaveFileDialog {
                Filter = "HTML|*.html",
                FileName = $"Informe_Citas_{DateTime.Now:yyyyMMdd_HHmmss}.html"
            };
            
            if (saveDialog.ShowDialog() == true) { 
                var result = _reportService.GuardarInformeHtml(informeHtml.Value, saveDialog.FileName);
                if (result.IsSuccess) {
                    StatusMessage = "Informe HTML generado";
                    _dialogService.ShowSuccess("Se ha generado correctamente el informe html.");
                } else {
                    _dialogService.ShowError(result.Error.Message);
                }
            }
        } catch (Exception e) {
            _logger.Error(e, "Error al generar informe en html");
            StatusMessage = "Error al generar";
        } finally {
            IsGenerating = false;
        }
    }

    [RelayCommand]
    private void GenerarInformePdf() {
        try {
            IsGenerating = true;
            StatusMessage = "Generando informe PDF...";

            var citas = _citaService.GetAll(1, int.MaxValue).ToList();
            if (!IncluirEliminadas) citas = citas.Where(c => !c.IsDelete).ToList();

            var informeHtml = _reportService.GenerarInformeHtml(citas);
            if (informeHtml.IsFailure) {
                _dialogService.ShowError(informeHtml.Error.Message);
                return;
            }

            var saveDialog = new SaveFileDialog {
                Filter = "PDF|*.pdf",
                FileName = $"Informe_Citas_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            };

            if (saveDialog.ShowDialog() == true) { 
                var result = _reportService.GuardarInformePdf(informeHtml.Value, saveDialog.FileName);
                if (result.IsSuccess) {
                    StatusMessage = "Informe PDF generado";
                    _dialogService.ShowSuccess("Se ha generado correctamente el informe pdf.");
                } else {
                    _dialogService.ShowError(result.Error.Message);
                }
            }
        } catch (Exception e) {
            _logger.Error(e, "Error al generar informe en pdf");
            StatusMessage = "Error al generar";
        } finally {
            IsGenerating = false;
        }
    }
}