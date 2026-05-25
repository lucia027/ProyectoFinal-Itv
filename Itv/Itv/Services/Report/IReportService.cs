using CSharpFunctionalExtensions;
using Itv.Errors.Common;
using Itv.Models;

namespace Itv.Services.Report;

public interface IReportService {

    /// <summary>
    /// Genera un nuevo objeto Informe en base a las estadisticas de las citas registradas.
    /// </summary>
    /// <param name="items">Citas registradas.</param>
    /// <returns>Objeto informe creado.</returns>
    Informe GenerarInforme(IEnumerable<Cita> items);

    /// <summary>
    /// Genera un html con el informe resultante.
    /// </summary>
    /// <param name="items">Citas registradas en el sistema.</param>
    /// <returns>Result con el html o con un error.</returns>
    Result<string, DomainError> GenerarInformeHtml(IEnumerable<Cita> items);

    /// <summary>
    /// Genera un informe en formato html a partir del html generado anteriormente.
    /// </summary>
    /// <param name="html">Informe en formato html.</param>
    /// <param name="fileName">Nombre del archivo html.</param>
    /// <returns>Result con true si es correcto y error.</returns>
    Result<bool, DomainError> GuardarInformeHtml(string html, string fileName);

    /// <summary>
    /// Genera un informe en formato pdf a partir del html generado anteriormente.
    /// </summary>
    /// <param name="html">Informe en formato html.</param>
    /// <param name="fileName">Nombre del archivo pdf.</param>
    /// <returns>Result con true si es correcto y error.</returns>
    Result<bool, DomainError> GuardarInformePdf(string html, string fileName);
}