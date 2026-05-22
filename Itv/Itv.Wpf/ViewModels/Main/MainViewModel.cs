
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Itv.Config;
using Itv.Enums;
using Itv.Services.Citas;
using Itv.Services.ImportExport;
using Itv.Services.Report;
using Itv.Wpf.ViewModels.Cita;
using Itv.Wpf.ViewModels.FormData;

namespace Itv.Wpf.ViewModels.Main;

public partial class MainViewModel(
    ICitaService citaService,
    IReportService reportService,
    IImportExportService importExportService
    ) : ObservableObject {

    private readonly ICitaService _citaService = citaService;
    private readonly IReportService _reportService = reportService;
    private readonly IImportExportService _importExportService = importExportService;

    private List<Models.Cita> _allCitas = new();

    //Propiedades observables para cambiar la vista con el menu.
    [ObservableProperty] 
    private bool _isCitasVisibles = true;

    [ObservableProperty]
    private bool _isInformesvisibles;

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

    public bool IsborradoLogico => Configuracion.UseLogicalDelete;

    public IEnumerable<Motor> MotoresDisponibles => Enum.GetValues<Motor>();

    [RelayCommand]
    private void MostrarCitas() {
        OcultarTodo();
        IsCitasVisibles = true;
        
        TituloFormulario = "Detalle de cita";
        DescripcionFormulario = "Selecciona una cita de la tabla para cargar aquí sus datos.";
    }
    
    //Funciones para cambiar entre las páginas del menu.
    [RelayCommand]
    private void MostrarInformes() {
        OcultarTodo();
        IsInformesvisibles = true;
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
        try {
            _allCitas = _citaService.GetAll()

        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }


    private void OcultarTodo() {
        IsCitasVisibles = false;
        IsInformesvisibles = false;
        IsImportarVisible = false;
        IsExportarVisible = false;
    }
}