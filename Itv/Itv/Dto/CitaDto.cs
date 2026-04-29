using System.Xml.Serialization;

namespace Itv.Dto;

/// <summary>
/// Objeto de transferencia de datos creado para las citas.
/// </summary>
[XmlRoot("Itv")]
[XmlType("Cita")]
public record CitaDto(
    [property: XmlAttribute("id")] int Id,
    [property: XmlAttribute("matricula")] string Matricula,
    [property: XmlAttribute("marca")] string Marca,
    [property: XmlAttribute("modelo")] string Modelo,
    [property: XmlAttribute("cilindrada")] int Cilindrada,
    [property: XmlAttribute("motor")] string Motor,
    [property: XmlAttribute("dniDueño")] string DniDueño,
    [property: XmlAttribute("CreateAt")] string FechaMatriculacion,
    [property: XmlAttribute("CreateAt")] string FechaInspeccion,
    [property: XmlAttribute("CreateAt")] string CreateAt,
    [property: XmlAttribute("UpdateAt")] string UpdateAt,
    [property: XmlAttribute("isDelete")] bool IsDelete
) {
    public CitaDto() : this (0, "", "", "", 0 ,"", "", "", "", "", "", false) { }
}