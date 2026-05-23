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
using Itv.Wpf.Views;

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
    private bool _incluirEliminadas;

    [ObservableProperty]
    private bool _isInformesVisible;

    [ObservableProperty] 
    private bool _isImportarVisible;

    [ObservableProperty]
    private bool _isExportarVisible;
    
    //Propiedades de Cita
    [ObservableProperty]
    private string _motorFiltroSeleccionado = "Todos";
    
    [ObservableProperty]
    private string _campoBusqueda = string.Empty;
    
    [ObservableProperty]
    private DateTime? _fechaInspeccionInicio;

    [ObservableProperty]
    private DateTime? _fechaInspeccionFin;
    
    [ObservableProperty]
    private ObservableCollection<CitaItemViewModel> _citas = new();

    [ObservableProperty]
    private CitaItemViewModel? _citaSeleccionada;

    [ObservableProperty]
    private CitaFormData _form = new();

    [ObservableProperty]
    private string _tituloFormulario = "Detalle de cita";

    [ObservableProperty]
    private string _descripcionFormulario = "Selecciona una cita de la tabla para cargar aquí sus datos.";

    [ObservableProperty]
    private string _textoPaginaActual = "Pag. 1 / 1";

    private bool IsBorradoLogico => Configuracion.UseLogicalDelete;

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
    
    //Crear - Actualizar - Eliminar
    [RelayCommand]
    private void CrearCita() {
        OcultarTodo();
        IsCitasVisible = true;

        CitaSeleccionada = null;
        Form = new CitaFormData();
        
        TituloFormulario = "Crear cita";
        DescripcionFormulario = "Rellena los datos del vehículo para registrar una nueva cita.";
    }

    [RelayCommand]
    private void ActualizarCita() {
        OcultarTodo();
        IsCitasVisible = true;
        
        if (CitaSeleccionada == null) {
            MessageBox.Show(
                "Selecciona una cita antes de actualizar.",
                "Actualizar cita",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
            return;
        }

        TituloFormulario = "Actualizar cita";
        DescripcionFormulario = "Modifica los datos de la cita seleccionada en la tabla.";
    }

    [RelayCommand]
    private void EliminarCita() {
        OcultarTodo();
        IsCitasVisible = true;

        if (CitaSeleccionada == null) {
            MessageBox.Show(
                "Selecciona una cita antes de eliminar.",
                "Eliminar cita",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
            return;
        }

        var mensaje = IsBorradoLogico 
            ? $"¿Seguro que quieres eliminar la cita con matrícula {CitaSeleccionada.Matricula}? El borrado será lógico." 
            : $"¿Seguro que quieres eliminar definitivamente la cita con matrícula {CitaSeleccionada.Matricula}? Esta acción no se puede deshacer";
        
        var confirmacion = MessageBox.Show(
            mensaje,
            "Confirmar eliminación",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question
        );
        if (confirmacion != MessageBoxResult.Yes) return;

        var res = _citaService.Delete(CitaSeleccionada.Id);
        if (res.IsSuccess) {
            MessageBox.Show(
                IsBorradoLogico ? "Cita eliminada correctamente." : "Cita eliminada permanentemente.",
                "Correcto",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            Form = new CitaFormData();
            CitaSeleccionada = null;
            CargarCitas();
            
            TituloFormulario = "Detalle de cita";
            DescripcionFormulario = "Selecciona una cita de la tabla para cargar aquí sus datos.";
        } else {
            MessageBox.Show(
                res.Error.Message,
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }

    [RelayCommand]
    private void Guardar() {
        if (!Form.IsValid) {
            MessageBox.Show(
                $"Hay errores de validación:\n\n{Form.GetValidationErrors()}",
                "Validación",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
            return;
        }

        var cita = Form.ToModel();
        var res = cita.Id == 0 ? _citaService.Create(cita) : _citaService.Update(cita.Id, cita);

        if (res.IsSuccess) {
            MessageBox.Show(
                "Cita guardada correctamente.",
                "Correcto",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
            
            CargarCitas();
            TituloFormulario = "Detalle de cita";
            DescripcionFormulario = "Selecciona una cita de la tabla para cargar aquí sus datos.";
        } else {
            MessageBox.Show(
                res.Error.Message,
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }
    
    [RelayCommand]
    private void Buscar() {
        FiltrarCitas();
    }
    
    [RelayCommand]
    private void AbrirAcercaDe() {
        var acercaDe = new AcercaDeWindow {
            Owner = Application.Current.MainWindow
        };

        acercaDe.ShowDialog();
    }

    private void CargarCitas() {
        try {
            _allCitas = _citaService.GetAll(1, int.MaxValue).ToList();
            FiltrarCitas();
        } catch (Exception e) {
            MessageBox.Show(
                $"Error al cargar citas: {e.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }

    private void FiltrarCitas() {
        IEnumerable<Models.Cita> filtradas = _allCitas;

        if (IsBorradoLogico && !IncluirEliminadas) {
            filtradas = filtradas.Where(c => !c.IsDelete);
        }
        if (!string.IsNullOrWhiteSpace(CampoBusqueda)) {
            filtradas = filtradas.Where(c =>
                c.Matricula.Contains(CampoBusqueda, StringComparison.OrdinalIgnoreCase) ||
                c.Marca.Contains(CampoBusqueda, StringComparison.OrdinalIgnoreCase) ||
                c.Modelo.Contains(CampoBusqueda, StringComparison.OrdinalIgnoreCase) ||
                c.DniDueño.Contains(CampoBusqueda, StringComparison.OrdinalIgnoreCase)
            );
        }
        if (!string.IsNullOrWhiteSpace(MotorFiltroSeleccionado) &&
            MotorFiltroSeleccionado != "Todos" &&
            Enum.TryParse<Motor>(MotorFiltroSeleccionado, out var motor)) {
            filtradas = filtradas.Where(c => c.Motor == motor);
        }
        if (FechaInspeccionInicio.HasValue) {
            filtradas = filtradas.Where(c =>
                c.FechaMatriculacion.Date >= FechaInspeccionInicio.Value.Date
            );
        }

        if (FechaInspeccionFin.HasValue) {
            filtradas = filtradas.Where(c =>
                c.FechaMatriculacion.Date <= FechaInspeccionFin.Value.Date
            );
        }

        var lista = filtradas
            .OrderBy(c => c.Id)
            .ToList();
        Citas = new ObservableCollection<CitaItemViewModel>(
            lista.Select(c => c.ToItemViewModel())
        );

        TextoPaginaActual = "Pág. 1 / 1";
    }

    partial void OnCitaSeleccionadaChanged(CitaItemViewModel? value) {
        if (value == null) return;
        Form = value.ToModel().ToFormData();
        
        TituloFormulario = "Detalle de cita";
        DescripcionFormulario = "Datos cargados desde la cita seleccionada.";
    }
    
    partial void OnFechaInspeccionInicioChanged(DateTime? value) {
        FiltrarCitas();
    }

    partial void OnFechaInspeccionFinChanged(DateTime? value) {
        FiltrarCitas();
    }
    
    partial void OnIncluirEliminadasChanged(bool value) {
        FiltrarCitas();
    }
    
    partial void OnMotorFiltroSeleccionadoChanged(string value) {
        FiltrarCitas();
    }

    private void OcultarTodo() {
        IsCitasVisible = false;
        IsInformesVisible = false;
        IsImportarVisible = false;
        IsExportarVisible = false;
    }
    
    public ObservableCollection<string> MotoresFiltroDisponibles { get; } = [
        "Todos",
        nameof(Motor.Diesel),
        nameof(Motor.Gasolina),
        nameof(Motor.Hibrido),
        nameof(Motor.Electrico)
    ];
}