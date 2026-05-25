using Itv.Models;
using Itv.Wpf.ViewModels.Cita;
using Itv.Wpf.ViewModels.FormData;

namespace Itv.Wpf.Mappers;

/// <summary>
/// Clase estatica con las funciones para mapear una cita en el front.
/// </summary>
public static class CitaMapper {
    
    /// <summary>
    /// Convierte una Cita a un CitaFormData.
    /// </summary>
    /// <param name="dto">Cita a convertir.</param>
    /// <returns>CitaFormData convertido.</returns>
    public static CitaFormData ToFormData(this Cita model) {
        return new CitaFormData {
            Id = model.Id,
            Matricula = model.Matricula,
            Marca = model.Marca,
            Modelo = model.Modelo,
            Cilindrada = model.Cilindrada,
            Motor = model.Motor,
            DniDueño = model.DniDueño,
            FechaMatriculacion = model.FechaMatriculacion,
            FechaInspeccion = model.FechaInspeccion,
            CreatedAt = model.CreateAt,
            UpdatedAt = model.UpdateAt ?? DateTime.Now,
            IsDeleted = model.IsDelete,
        };
    }

    /// <summary>
    /// Convierte una CitaFormData a una Cita.
    /// </summary>
    /// <param name="formData">CitaFormData a convertir.</param>
    /// <returns>Cita convertido.</returns>
    public static Cita ToModel(this CitaFormData formData) {
        return new Cita {
            Id = formData.Id,
            Matricula = formData.Matricula,
            Marca = formData.Marca,
            Modelo = formData.Modelo,
            Cilindrada = formData.Cilindrada,
            Motor = formData.Motor,
            DniDueño = formData.DniDueño,
            FechaMatriculacion = formData.FechaMatriculacion,
            FechaInspeccion = formData.FechaInspeccion,
            CreateAt = formData.CreatedAt,
            UpdateAt = formData.UpdatedAt,
            IsDelete = formData.IsDeleted,
        };
    }

    
    /// <summary>
    /// Convierte una Cita a un CitaItemViewModel.
    /// </summary>
    /// <param name="model">Cita a convertir.</param>
    /// <returns>CitaItemViewModel convertida.</returns>
    public static CitaItemViewModel ToItemViewModel(this Cita model) {
        return new CitaItemViewModel {
            Id = model.Id,
            Matricula = model.Matricula,
            Marca = model.Marca,
            Modelo = model.Modelo,
            Cilindrada = model.Cilindrada,
            Motor = model.Motor,
            DniDueño = model.DniDueño,
            FechaMatriculacion = model.FechaMatriculacion,
            FechaInspeccion = model.FechaInspeccion,
            IsDelete = model.IsDelete
        };
    }
    
    /// <summary>
    /// Convierte una CitaItemViewModel a una Cita.
    /// </summary>
    /// <param name="item">CitaItemViewModel a convertir.</param>
    /// <returns>Cita convertida.</returns>
    public static Cita ToModel(this CitaItemViewModel item) {
        return new Cita {
            Id = item.Id,
            Matricula = item.Matricula,
            Marca = item.Marca,
            Modelo = item.Modelo,
            Cilindrada = item.Cilindrada,
            Motor = item.Motor,
            DniDueño = item.DniDueño,
            FechaInspeccion = item.FechaInspeccion,
            FechaMatriculacion = item.FechaMatriculacion,
            IsDelete = item.IsDelete
        };
    }
}