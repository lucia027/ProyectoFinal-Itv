using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Itv.Entity;

/// <summary>
/// Entidad para as citas del sistema.
/// </summary>
[Table("Citas")]
[Index(nameof(Matricula), IsUnique = true)]
public record CitaEntity {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }
    
    [Required]
    [MaxLength(7)]
    public string Matricula { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Marca { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Modelo { get; set; } = string.Empty; 
    
    [Required]
    [MaxLength(10)]
    public int Cilindrada { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Motor { get; set; } = "Diesel";
    
    [Required]
    [MaxLength(9)]
    public string DniDueño { get; set; } = string.Empty;
    
    [Column(TypeName = "datetime2")]
    public DateTime FechaMatriculacion { get; set; } = DateTime.Today;
    
    [Column(TypeName = "datetime2")]
    public DateTime FechaInspeccion { get; set; } = DateTime.Today;
    
    [Column(TypeName = "datetime2")]
    public DateTime CreateAt { get; set; } =  DateTime.Today;
    
    [Column(TypeName = "datetime2")]
    public DateTime? UpdateAt { get; set; } 
    
    public bool IsDelete { get; set; } =  false;
}