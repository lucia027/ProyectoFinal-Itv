using System.Globalization;
using Itv.Dto;
using Itv.Enums;
using Itv.Models;

namespace Itv.Mappers;
/// <summary>
/// Clase estatica con las funciones para mapear una cita a modelo y a dto.
/// </summary>
public static class CitaMapper {
    
    private const string DateTimeFormat = "s";
    private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

    /// <summary>
    /// Convierte un CitaDto a un cita.
    /// </summary>
    /// <param name="dto">CitaDto a convertir.</param>
    /// <returns>Cita convertido.</returns>
    public static Cita ToModel(this CitaDto dto) {
        return new Cita {
            Id = dto.Id,
            Matricula = dto.Matricula,
            Marca = dto.Marca,
            Modelo = dto.Modelo,
            Cilindrada = (dto.Cilindrada < 0) ? 0 : dto.Cilindrada,
            Motor = Enum.TryParse(dto.Motor, out Motor motor) ? motor : Motor.Diesel,
            DniDueño = dto.DniDueño,
            FechaMatriculacion = DateTime.TryParse(dto.CreateAt, InvariantCulture, out var m) ? m : DateTime.Now,
            FechaInspeccion = DateTime.TryParse(dto.CreateAt, InvariantCulture, out var i) ? i : DateTime.Now,
            CreateAt = DateTime.TryParse(dto.CreateAt, InvariantCulture, out var c) ? c : DateTime.Now,
            UpdateAt = DateTime.TryParse(dto.UpdateAt, InvariantCulture, out var u) ? u : DateTime.Now,
            IsDelete = dto.IsDelete
        };
    }

    /// <summary>
    /// Convierte una Cita a un CitaDto.
    /// </summary>
    /// <param name="cita">Cita a convertir.</param>
    /// <returns>CitaDto convertido.</returns>
    public static CitaDto ToDto(this Cita cita) {
        return new CitaDto (
            cita.Id,
            cita.Matricula,
            cita.Marca,
            cita.Modelo,
            cita.Cilindrada,
            cita.Motor.ToString(),
            cita.DniDueño,
            cita.FechaMatriculacion.ToString(DateTimeFormat, InvariantCulture),
            cita.FechaInspeccion.ToString(DateTimeFormat, InvariantCulture),
            cita.CreateAt.ToString(DateTimeFormat, InvariantCulture),
            cita.UpdateAt?.ToString(DateTimeFormat, InvariantCulture) ?? "No se ha actualizado",
            cita.IsDelete
        );
    }
}