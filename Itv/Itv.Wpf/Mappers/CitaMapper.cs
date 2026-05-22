using Itv.Models;
using Itv.Wpf.ViewModels.Cita;
using Itv.Wpf.ViewModels.FormData;

namespace Itv.Wpf.Mappers {
    public static class CitaMapper {
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

        public static void UpdateFromFormData(this CitaItemViewModel item, CitaFormData form) {
            item.Matricula = form.Matricula;
            item.Marca = form.Marca;
            item.Modelo = form.Modelo;
            item.Cilindrada = form.Cilindrada;
            item.Motor = form.Motor;
            item.DniDueño = form.DniDueño;
            item.FechaMatriculacion = form.FechaMatriculacion;
            item.FechaInspeccion = form.FechaInspeccion;
            item.IsDelete = form.IsDeleted;
        }

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
}