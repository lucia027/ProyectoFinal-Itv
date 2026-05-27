
namespace Itv.Models;

/// <summary>
/// Representa a un informe en el sistema.
/// </summary>
public record Informe {
    public int TotalCitas { get; set; }
    public int TotalVehiculosMotorGasolina { get; set; }
    public int TotalVehiculosMotorDiesel { get; set; }
    public int TotalVehiculosMotorHibrido { get; set; }
    public int TotalVehiculosMotorElectrico { get; set; }
    public double PromedioCilindradaCitas { get; set; }
}