using System;
using System.Text;
using Itv.Dto;
using Itv.Enums;
using Itv.Models;

namespace Itv.Mappers;
/// <summary>
/// Clase estatica con las funciones para mapear un vehículo a modelo y a dto.
/// </summary>
public static class VehiculoMapper {
    
    private const string DateTimeFormat = "s";

    /// <summary>
    /// Convierte un VehiculoDto a un Vehículo.
    /// </summary>
    /// <param name="dto">VehiculoDto a convertir.</param>
    /// <returns>Vehiculo convertido.</returns>
    public static Vehiculo ToModel(this VehiculoDto dto) {
        return new Vehiculo {
            Id = dto.Id,
            Matricula = dto.Matricula,
            Marca = dto.Marca,
            Modelo = dto.Modelo,
            Cilindrada = (dto.Cilindrada < 0) ? 0 : dto.Cilindrada,
            Motor = Enum.TryParse(dto.Motor, out Motor motor) ? motor : Motor.Diesel,
            DniDueño = dto.DniDueño,
            CreateAt = DateTime.Parse(dto.CreateAt),
            UpdateAt = DateTime.Parse(dto.UpdateAt),
            IsDelete = dto.IsDelete
        };
    }

    /// <summary>
    /// Convierte un Vehículo a un VehiculoDto.
    /// </summary>
    /// <param name="vehiculo">Vehículo a convertir.</param>
    /// <returns>VehiculoDto convertido.</returns>
    public static VehiculoDto ToDto(this Vehiculo vehiculo) {
        return new VehiculoDto (
            vehiculo.Id,
            vehiculo.Matricula,
            vehiculo.Marca,
            vehiculo.Modelo,
            vehiculo.Cilindrada,
            vehiculo.Motor.ToString(),
            vehiculo.DniDueño,
            vehiculo.CreateAt.ToString(DateTimeFormat),
            vehiculo.UpdateAt.ToString(DateTimeFormat),
            vehiculo.IsDelete
        );
    }
}