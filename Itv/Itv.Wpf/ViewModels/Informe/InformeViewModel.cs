using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Itv.Services.Citas;
using Itv.Services.Report;
using Microsoft.Win32;

namespace Itv.Wpf.ViewModels.Informe;

public partial class InformeViewModel : ObservableObject {
    private readonly ICitaService _citaService;
    private readonly IReportService _reportService;

    public InformeViewModel(
        ICitaService citaService,
        IReportService reportService
    ) {
        _citaService = citaService;
        _reportService = reportService;
    }

    [ObservableProperty]
    private bool _isGenerating;

    [ObservableProperty]
    private bool _incluirEliminadas;

    [ObservableProperty]
    private string _statusMessage = "Listo";

    [RelayCommand]
    private void GenerarInformeHtml() {
        try {
            IsGenerating = true;
            StatusMessage = "Generando informe HTML...";

            var citas = _citaService.GetAll(1, int.MaxValue).ToList();

            if (!IncluirEliminadas) {
                citas = citas.Where(c => !c.IsDelete).ToList();
            }

            var htmlResult = _reportService.GenerarInformeHtml(citas);

            if (htmlResult.IsFailure) {
                MessageBox.Show(
                    htmlResult.Error.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                StatusMessage = "Error al generar HTML";
                return;
            }

            var saveDialog = new SaveFileDialog {
                Filter = "HTML|*.html",
                FileName = $"Informe_Citas_{DateTime.Now:yyyyMMdd_HHmmss}.html"
            };

            if (saveDialog.ShowDialog() != true) {
                StatusMessage = "Generación cancelada";
                return;
            }

            var saveResult = _reportService.GuardarInformeHtml(htmlResult.Value, saveDialog.FileName);

            if (saveResult.IsSuccess) {
                MessageBox.Show(
                    "Informe HTML generado correctamente.",
                    "Correcto",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                StatusMessage = "Informe HTML generado";
            }
            else {
                MessageBox.Show(
                    saveResult.Error.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                StatusMessage = "Error al guardar HTML";
            }
        }
        catch (Exception ex) {
            MessageBox.Show(
                $"Error al generar informe HTML: {ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            StatusMessage = "Error al generar HTML";
        }
        finally {
            IsGenerating = false;
        }
    }

    [RelayCommand]
    private void GenerarInformePdf() {
        try {
            IsGenerating = true;
            StatusMessage = "Generando informe PDF...";

            var citas = _citaService.GetAll(1, int.MaxValue).ToList();

            if (!IncluirEliminadas) {
                citas = citas.Where(c => !c.IsDelete).ToList();
            }

            var htmlResult = _reportService.GenerarInformeHtml(citas);

            if (htmlResult.IsFailure) {
                MessageBox.Show(
                    htmlResult.Error.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                StatusMessage = "Error al generar HTML base";
                return;
            }

            var saveDialog = new SaveFileDialog {
                Filter = "PDF|*.pdf",
                FileName = $"Informe_Citas_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            };

            if (saveDialog.ShowDialog() != true) {
                StatusMessage = "Generación cancelada";
                return;
            }

            var pdfResult = _reportService.GuardarInformePdf(htmlResult.Value, saveDialog.FileName);

            if (pdfResult.IsSuccess) {
                MessageBox.Show(
                    "Informe PDF generado correctamente.",
                    "Correcto",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                StatusMessage = "Informe PDF generado";
            }
            else {
                MessageBox.Show(
                    pdfResult.Error.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                StatusMessage = "Error al guardar PDF";
            }
        }
        catch (Exception ex) {
            MessageBox.Show(
                $"Error al generar informe PDF: {ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            StatusMessage = "Error al generar PDF";
        }
        finally {
            IsGenerating = false;
        }
    }
}