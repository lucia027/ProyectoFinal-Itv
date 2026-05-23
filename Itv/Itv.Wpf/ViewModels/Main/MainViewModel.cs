using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Itv.Config;
using Itv.Enums;
using Itv.Services.Citas;
using Itv.Services.ImportExport;
using Itv.Services.Report;
using Itv.Wpf.Mappers;
using Itv.Wpf.ViewModels.Cita;
using Itv.Wpf.ViewModels.FormData;

namespace Itv.Wpf.ViewModels.Main;

public partial class MainViewModel : ObservableObject {
    private readonly ICitaService _citaService;
    private readonly IReportService _reportService;
    private readonly IImportExportService _importExportService;

    private List<Models.Cita> _allCitas = new();

    public MainViewModel(
        ICitaService citaService,
        IReportService reportService,
        IImportExportService importExportService
    ) {
        _citaService = citaService;
        _reportService = reportService;
        _importExportService = importExportService;

        CargarCitas();
    }

    //Propiedades observables para cambiar la vista con el menu.
    [ObservableProperty] 
    private bool _isCitasVisible = true;

    [ObservableProperty]
    private bool _isInformesVisible;

    [ObservableProperty] 
    private bool _isImportarVisible;

    [ObservableProperty]
    private bool _isExportarVisible;
    
    //Propiedades de Cita
    [ObservableProperty]
    private ObservableCollection<CitaItemViewModel> _citas = new();

    [ObservableProperty]
    private CitaItemViewModel? _CitaSeleccionada;

    [ObservableProperty]
    private CitaFormData _form = new();

    [ObservableProperty]
    private string _tituloFormulario = "Detalle de cita";

    [ObservableProperty]
    private string _descripcionFormulario = "Selecciona una cita de la tabla para cargar aquí sus datos.";

    [ObservableProperty]
    private string _textoPaginaActual = "Pag. 1 / 1";

    public bool IsBorradoLogico => Configuracion.UseLogicalDelete;

    public IEnumerable<Motor> MotoresDisponibles => Enum.GetValues<Motor>();

    //Funciones para cambiar entre las páginas del menu.
    [RelayCommand]
    private void MostrarCitas() {
        OcultarTodo();
        IsCitasVisible = true;
        
        TituloFormulario = "Detalle de cita";
        DescripcionFormulario = "Selecciona una cita de la tabla para cargar aquí sus datos.";
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

    private void CargarCitas() {
        try
        {
            _allCitas = _citaService.GetAll(1, int.MaxValue, true).ToList();
            var citasVisibles = _allCitas
                .Where(c => !IsBorradoLogico || !c.IsDelete)
                .Select(c => c.ToItemViewModel())
                .ToList();

            Citas = new ObservableCollection<CitaItemViewModel>(citasVisibles);
            TextoPaginaActual = "Pag. 1 / 1";
        }
        catch (Exception e)
        {
            MessageBox.Show(
                $"Error al cargar citas: {e.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }

    partial void OnCitaSeleccionadaChanged(CitaItemViewModel? value) {
        if (value == null) return;
        Form = value.ToModel().ToFormData();
        
        TituloFormulario = "Detalle de cita";
        DescripcionFormulario = "Datos cargados desde la cita seleccionada.";
    }
    
    

    private void OcultarTodo() {
        IsCitasVisible = false;
        IsInformesVisible = false;
        IsImportarVisible = false;
        IsExportarVisible = false;
    }
}