using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Itv.Wpf.ViewModels.Cita;
using Itv.Wpf.ViewModels.ImportExport;
using Itv.Wpf.ViewModels.Informe;
using Itv.Wpf.Views;
using System.Windows;

namespace Itv.Wpf.ViewModels.Main;

public partial class MainViewModel : ObservableObject {
    public CitaViewModel CitaVm { get; }
    public InformeViewModel InformeVm { get; }
    public ImportExportViewModel ImportExportVm { get; }

    public MainViewModel(
        CitaViewModel citaVm,
        InformeViewModel informeVm,
        ImportExportViewModel importExportVm
    ) {
        CitaVm = citaVm;
        InformeVm = informeVm;
        ImportExportVm = importExportVm;
    }

    [ObservableProperty]
    private bool _isCitasVisible = true;

    [ObservableProperty]
    private bool _isInformesVisible;

    [ObservableProperty]
    private bool _isImportarVisible;

    [ObservableProperty]
    private bool _isExportarVisible;

    [RelayCommand]
    private void MostrarCitas() {
        OcultarTodo();
        IsCitasVisible = true;
    }

    [RelayCommand]
    private void MostrarInformes() {
        OcultarTodo();
        IsInformesVisible = true;
    }

    [RelayCommand]
    private void MostrarImportar() {
        OcultarTodo();
        IsImportarVisible = true;
    }

    [RelayCommand]
    private void MostrarExportar() {
        OcultarTodo();
        IsExportarVisible = true;
    }

    [RelayCommand]
    private void AbrirAcercaDe() {
        var acercaDe = new AcercaDeWindow {
            Owner = Application.Current.MainWindow
        };

        acercaDe.ShowDialog();
    }

    private void OcultarTodo() {
        IsCitasVisible = false;
        IsInformesVisible = false;
        IsImportarVisible = false;
        IsExportarVisible = false;
    }
}