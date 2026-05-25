using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Itv.Config;
using Itv.Enums;
using Itv.Services.Citas;
using Itv.Wpf.Mappers;
using Itv.Wpf.ViewModels.FormData;

namespace Itv.Wpf.ViewModels.Cita;

/// <summary>
/// ViewModel que gestiona el filtrado de citas y su gestion.
/// </summary>
public partial class CitaViewModel : ObservableObject {
    private readonly ICitaService _citaService;

    private List<Models.Cita> _allCitas = new();

    public CitaViewModel(ICitaService citaService) {
        _citaService = citaService;
        CargarCitas();
    }

    [ObservableProperty]
    private bool _incluirEliminadas;

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
    private int _paginaActual = 1;

    [ObservableProperty]
    private int _tamanoPagina = 10;

    [ObservableProperty]
    private int _totalPaginas = 1;

    [ObservableProperty]
    private int _totalRegistros;

    [ObservableProperty]
    private string _textoPaginaActual = "Pág. 1 / 1";

    [ObservableProperty]
    private string _statusMessage = "Listo";

    [ObservableProperty]
    private bool _isLoading;

    public bool IsBorradoLogico => Configuracion.UseLogicalDelete;

    public IEnumerable<Motor> MotoresDisponibles => Enum.GetValues<Motor>();

    public ObservableCollection<string> MotoresFiltroDisponibles { get; } = [
        "Todos",
        nameof(Motor.Diesel),
        nameof(Motor.Gasolina),
        nameof(Motor.Hibrido),
        nameof(Motor.Electrico)
    ];

    public bool PuedeIrAPaginaAnterior => PaginaActual > 1;

    public bool PuedeIrAPaginaSiguiente => PaginaActual < TotalPaginas;

    [RelayCommand]
    private void CrearCita() {
        CitaSeleccionada = null;
        Form = new CitaFormData();

        TituloFormulario = "Crear cita";
        DescripcionFormulario = "Rellena los datos del vehículo para registrar una nueva cita.";
    }

    [RelayCommand]
    private void ActualizarCita() {
        if (CitaSeleccionada == null) {
            MessageBox.Show(
                "Selecciona una cita antes de actualizar.",
                "Actualizar cita",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
            return;
        }

        var citaOriginal = _allCitas.FirstOrDefault(c => c.Id == CitaSeleccionada.Id);

        if (citaOriginal == null) {
            MessageBox.Show(
                "No se ha encontrado la cita seleccionada.",
                "Actualizar cita",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
            return;
        }

        Form = citaOriginal.ToFormData();

        TituloFormulario = "Actualizar cita";
        DescripcionFormulario = "Modifica los datos de la cita seleccionada en la tabla.";
    }

    [RelayCommand]
    private void EliminarCita() {
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
            : $"¿Seguro que quieres eliminar definitivamente la cita con matrícula {CitaSeleccionada.Matricula}? Esta acción no se puede deshacer.";

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

            ResetFormulario();
            CargarCitas();
        }
        else {
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

        var res = cita.Id == 0
            ? _citaService.Create(cita)
            : _citaService.Update(cita.Id, cita);

        if (res.IsSuccess) {
            MessageBox.Show(
                cita.Id == 0 ? "Cita creada correctamente." : "Cita actualizada correctamente.",
                "Correcto",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            ResetFormulario();
            CargarCitas();
        }
        else {
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
        PaginaActual = 1;
        FiltrarCitas();
    }

    [RelayCommand]
    private void LimpiarFiltros() {
        CampoBusqueda = string.Empty;
        MotorFiltroSeleccionado = "Todos";
        FechaInspeccionInicio = null;
        FechaInspeccionFin = null;
        IncluirEliminadas = false;
        PaginaActual = 1;

        FiltrarCitas();
    }

    private void CargarCitas() {
        try {
            IsLoading = true;
            StatusMessage = "Cargando citas...";

            _allCitas = _citaService.GetAll(1, int.MaxValue).ToList();

            PaginaActual = 1;
            FiltrarCitas();
        }
        catch (Exception e) {
            MessageBox.Show(
                $"Error al cargar citas: {e.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            StatusMessage = "Error al cargar citas";
        }
        finally {
            IsLoading = false;
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
                c.FechaInspeccion.Date >= FechaInspeccionInicio.Value.Date
            );
        }

        if (FechaInspeccionFin.HasValue) {
            filtradas = filtradas.Where(c =>
                c.FechaInspeccion.Date <= FechaInspeccionFin.Value.Date
            );
        }

        var listaFiltrada = filtradas
            .OrderBy(c => c.Id)
            .ToList();

        TotalRegistros = listaFiltrada.Count;

        TotalPaginas = TotalRegistros == 0
            ? 1
            : (int)Math.Ceiling((double)TotalRegistros / TamanoPagina);

        if (PaginaActual > TotalPaginas) {
            PaginaActual = TotalPaginas;
        }

        if (PaginaActual < 1) {
            PaginaActual = 1;
        }

        var pagina = listaFiltrada
            .Skip((PaginaActual - 1) * TamanoPagina)
            .Take(TamanoPagina)
            .Select(c => c.ToItemViewModel())
            .ToList();

        Citas = new ObservableCollection<CitaItemViewModel>(pagina);

        TextoPaginaActual = $"Pág. {PaginaActual} / {TotalPaginas}";
        StatusMessage = $"Página {PaginaActual}/{TotalPaginas} - Total: {TotalRegistros} citas";

        PaginaAnteriorCommand.NotifyCanExecuteChanged();
        PaginaSiguienteCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(PuedeIrAPaginaAnterior))]
    private void PaginaAnterior() {
        if (PaginaActual <= 1) return;

        PaginaActual--;
        FiltrarCitas();
    }

    [RelayCommand(CanExecute = nameof(PuedeIrAPaginaSiguiente))]
    private void PaginaSiguiente() {
        if (PaginaActual >= TotalPaginas) return;

        PaginaActual++;
        FiltrarCitas();
    }

    [RelayCommand]
    private void PrimeraPagina() {
        PaginaActual = 1;
        FiltrarCitas();
    }

    [RelayCommand]
    private void UltimaPagina() {
        PaginaActual = TotalPaginas;
        FiltrarCitas();
    }

    partial void OnCitaSeleccionadaChanged(CitaItemViewModel? value) {
        if (value == null) return;

        var citaOriginal = _allCitas.FirstOrDefault(c => c.Id == value.Id);

        if (citaOriginal == null) return;

        Form = citaOriginal.ToFormData();

        TituloFormulario = "Detalle de cita";
        DescripcionFormulario = "Datos cargados desde la cita seleccionada.";
    }

    partial void OnCampoBusquedaChanged(string value) {
        PaginaActual = 1;
        FiltrarCitas();
    }

    partial void OnFechaInspeccionInicioChanged(DateTime? value) {
        PaginaActual = 1;
        FiltrarCitas();
    }

    partial void OnFechaInspeccionFinChanged(DateTime? value) {
        PaginaActual = 1;
        FiltrarCitas();
    }

    partial void OnIncluirEliminadasChanged(bool value) {
        PaginaActual = 1;
        FiltrarCitas();
    }

    partial void OnMotorFiltroSeleccionadoChanged(string value) {
        PaginaActual = 1;
        FiltrarCitas();
    }

    private void ResetFormulario() {
        Form = new CitaFormData();
        CitaSeleccionada = null;

        TituloFormulario = "Detalle de cita";
        DescripcionFormulario = "Selecciona una cita de la tabla para cargar aquí sus datos.";
    }
}