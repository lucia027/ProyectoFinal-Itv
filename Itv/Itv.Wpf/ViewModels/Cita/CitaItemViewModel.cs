using CommunityToolkit.Mvvm.ComponentModel;
using Itv.Enums;

namespace Itv.Wpf.ViewModels.Cita;


/// <summary>
///     Base reactiva para elementos de personas en listas.
///     Proporciona notificación de cambios para las propiedades comunes.
/// </summary>
public partial class CitaItemViewModel : ObservableObject {
    [ObservableProperty] private int id;
    
    [ObservableProperty] private string _matricula = string.Empty;

    [ObservableProperty] private string _marca = string.Empty;

    [ObservableProperty] private string _modelo = string.Empty;

    [ObservableProperty] private int _cilindrada;
    
    [ObservableProperty] private Motor _motor;
    
    [ObservableProperty] private string _dniDueño = string.Empty;
    
    [ObservableProperty] private DateTime _fechaMatriculacion;

    [ObservableProperty] private DateTime _fechaInspeccion;

    [ObservableProperty] private bool _isDelete;
}