using Itv.Enums;

namespace Itv.Models;

/// <summary>
/// Representa a un vehiculo en el sistema.
/// </summary>
public record Vehiculo {
    public int Id { get; init; }
    public string Matricula { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Cilindrada { get; set; }
    public Motor Motor { get; set; }
    public string DniDueño { get; set; } = string.Empty;
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
    public bool IsDelete { get; set; }
}