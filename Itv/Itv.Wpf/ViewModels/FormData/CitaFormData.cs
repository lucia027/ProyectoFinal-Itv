using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Itv.Enums;
using Itv.Validator;

namespace Itv.Wpf.ViewModels.FormData;

/// <summary>
/// FormData para la verificación de CITA en la capa de presentación.
/// Separa la lógica de validación UI del modelo de dominio puro.
/// IMPORTANTE: Este es el único lugar donde se implementa IDataErrorInfo para cita.
/// </summary>
public partial class CitaFormData : ObservableObject, IDataErrorInfo {

    [ObservableProperty] private int _id;

    [ObservableProperty] private string _matricula = string.Empty;

    [ObservableProperty] private string _marca = string.Empty;

    [ObservableProperty] private string _modelo = string.Empty;

    [ObservableProperty] private int _cilindrada;
    
    [ObservableProperty] private Motor _motor;
    
    [ObservableProperty] private string _dniDueño = string.Empty;

    [ObservableProperty] private DateTime _fechaMatriculacion = DateTime.Today;

    
    [ObservableProperty] private DateTime _fechaInspeccion = DateTime.Today.AddDays(1);

    /// <summary>
    /// Marca de tiempo de creación del registro
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Marca de tiempo de la última actualuzación del resgistro.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>Indica si el registro está marcado como eliminado (soft delete).</summary>
    public bool IsDeleted { get; set; }

    /// <summary>Resumen de errores globales del formulario. Requerido por IDataErrorInfo.</summary>
    public string Error => string.Empty;

    /// <summary>
    ///     Validación del campo por campo requerida por IDataErrorInfo para el binding WPF con ValidatesOnDataErrors=True.
    /// </summary>
    /// <param name="columnName">Nombre de la propiedad a validar.</param>
    /// <returns>Mensaje de error en español si el campo es inválido; cadena vacía o null si es válido.</returns>
    public string this[string columnName] => columnName switch {
        nameof(Matricula) when !Matricula.IsValidMatricula()
            => "La matrícula es obligatoria y no puede estar vacía",

        nameof(Marca) when !Marca.IsValidMarca()
            => "La marca es obligatoria (2-50 caracteres)",

        nameof(Modelo) when !Modelo.IsValidModelo()
            => "El modelo es obligatorio (2-50 caracteres)",

        nameof(Cilindrada) when !Cilindrada.IsValidCilindrada()
            => "La cilindrada debe de estar entre 0 y 3000",
        
        nameof(DniDueño) when !DniDueño.IsValidDniDueño()
            => "El DNI del propietario es obligatorio o tiene un formato inválido",

        nameof(FechaInspeccion) when !FechaInspeccion.IsValidFechaInpeccion()
            => "La inspección debe estar entre hoy y los próximos 30 días",
        
        nameof(FechaMatriculacion) when !FechaMatriculacion.IsValidFechaMatriculacion() 
            => "La fecha de matriculación no puede ser futura",
        
        _ => null!
    };


    /// <summary>
    ///     Verifica que todos los campos del formulario sean válidos antes de persistir.
    /// </summary>
    /// <returns>True si el formulario no tiene errores de validación.</returns>
    public bool IsValid =>
         string.IsNullOrEmpty(this[nameof(Matricula)]) &&
         string.IsNullOrEmpty(this[nameof(Marca)]) &&
         string.IsNullOrEmpty(this[nameof(Modelo)]) &&
         string.IsNullOrEmpty(this[nameof(Cilindrada)]) &&
         string.IsNullOrEmpty(this[nameof(DniDueño)])&&
         string.IsNullOrEmpty(this[nameof(FechaMatriculacion)]) &&
         string.IsNullOrEmpty(this[nameof(FechaInspeccion)]);

    

    /// <summary>
    ///     Devuelve una cadena con todos los errores de validación actuales, uno por línea.
    /// </summary>
    /// <returns>Texto con los errores de validación, o cadena vacía si el formulario es válido.</returns>
    public string GetValidationErrors() {
        var campos = new[] {
            (nameof(Matricula), "Matricula"),
            (nameof(Marca), "Marca"),
            (nameof(Modelo), "Modelo"),
            (nameof(Cilindrada), "Cilindrada"),
            (nameof(DniDueño), "DNI del Dueño"),
            (nameof(FechaMatriculacion), "Fecha de Matriculación"),
            (nameof(FechaInspeccion), "Fecha de Inspección"),

        };

        var errores = campos
            .Select(c => (Campo: c.Item2, Error: this[c.Item1]))
            .Where(c => !string.IsNullOrWhiteSpace(c.Error))
            .Select(c => $"• {c.Campo}: {c.Error}");

        return string.Join("\n", errores);
    }
    
    partial void OnMatriculaChanged(string value) => OnPropertyChanged(nameof(IsValid));
    partial void OnMarcaChanged(string value) => OnPropertyChanged(nameof(IsValid));
    partial void OnModeloChanged(string value) => OnPropertyChanged(nameof(IsValid));
    partial void OnCilindradaChanged(int value) => OnPropertyChanged(nameof(IsValid));
    partial void OnDniDueñoChanged(string value) => OnPropertyChanged(nameof(IsValid));
    partial void OnFechaMatriculacionChanged(DateTime value) => OnPropertyChanged(nameof(IsValid));
    partial void OnFechaInspeccionChanged(DateTime value) => OnPropertyChanged(nameof(IsValid));
}