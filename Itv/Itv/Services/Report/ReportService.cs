using System.Text;
using CSharpFunctionalExtensions;
using Itv.Enums;
using Itv.Errors;
using Itv.Errors.Common;
using Itv.Models;
using SelectPdf;
using Serilog;

namespace Itv.Services.Report;

/// <summary>
/// Servicio que gestiona la generacion de informes.
/// </summary>
public class ReportService : IReportService {
    private readonly ILogger _logger = Log.ForContext<ReportService>();
    private readonly string _reportDirectory;

    public ReportService(string reportDirectory) {
        _reportDirectory = reportDirectory;
        _logger.Debug("Se ha iniciado el servicio de reportes.");
    }

    /// <inheritdoc cref="IReportService.GenerarInforme" />
    public Informe GenerarInforme(IEnumerable<Cita> items) {
        var totalCitas = items.Count();
        if (totalCitas == 0) return new Informe();

        _logger.Debug($"Se ha generado correctamente el informe de un total de {totalCitas} citas registradas.");
        return new Informe{
            TotalCitas = totalCitas,
            TotalVehiculosMotorDiesel = items.Count(c => c.Motor == Motor.Diesel),
            TotalVehiculosMotorGasolina = items.Count(c => c.Motor == Motor.Gasolina),
            TotalVehiculosMotorHibrido = items.Count(c => c.Motor == Motor.Hibrido),
            TotalVehiculosMotorElectrico =  items.Count(c => c.Motor == Motor.Electrico),
            PromedioCilindradaCitas = items.Average(c => c.Cilindrada)
        };
    }
    
    /// <inheritdoc cref="IReportService.GenerarInformeHtml" />
    public Result<string, DomainError> GenerarInformeHtml(IEnumerable<Cita> items) {
        try {
            var TotalCitas = items.Count();
            var TotalVehiculosMotorDiesel = items.Count(c => c.Motor == Motor.Diesel);
            var TotalVehiculosMotorGasolina = items.Count(c => c.Motor == Motor.Gasolina);
            var TotalVehiculosMotorHibrido = items.Count(c => c.Motor == Motor.Hibrido);
            var TotalVehiculosMotorElectrico = items.Count(c => c.Motor == Motor.Electrico);
            var PromedioCilindradaCitas = (TotalCitas > 0) ? items.Average(c => c.Cilindrada) : 0.0; 

            var porcentajeDiesel = TotalVehiculosMotorDiesel * 100 / TotalCitas;
            var porcentajeHibrido = TotalVehiculosMotorHibrido * 100 / TotalCitas;
            var porcentajeElectrico = TotalVehiculosMotorElectrico * 100 / TotalCitas;
            var porcentajeGasolina = TotalVehiculosMotorGasolina * 100 / TotalCitas;

            var html = $@"
            <!DOCTYPE html>
                <html lang=""es"">
                <head>
                    <meta charset=""UTF-8"" />
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                    <title>Informe ITV</title>
                    <style>
                        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
                
                        body {{
                            font-family: Arial, Helvetica, sans-serif;
                            background: #eaf3fb;
                            color: #1c2b39;
                            padding: 24px;
                        }}
                
                        .report {{
                            max-width: 1050px;
                            margin: 0 auto;
                            background: #ffffff;
                            border-radius: 18px;
                            overflow: hidden;
                            box-shadow: 0 12px 30px rgba(20, 54, 86, 0.15);
                            border: 1px solid #d7e7f5;
                        }}
                
                        .hero {{
                            background: linear-gradient(135deg, #0f2f4f, #1c6ea4);
                            color: white;
                            padding: 34px 38px;
                        }}
                
                        .hero small {{
                            display: inline-block;
                            background: rgba(255,255,255,0.16);
                            padding: 6px 12px;
                            border-radius: 999px;
                            margin-bottom: 14px;
                            font-size: 13px;
                        }}
                
                        .hero h1 {{
                            font-size: 34px;
                            margin-bottom: 8px;
                            letter-spacing: -0.5px;
                        }}
                
                        .hero p {{
                            color: #d9edf9;
                            font-size: 15px;
                        }}
                
                        .content {{
                            padding: 30px 34px 34px;
                        }}
                
                        .summary {{
                            display: grid;
                            grid-template-columns: repeat(2, minmax(0, 1fr));
                            gap: 18px;
                            margin-bottom: 28px;
                        }}
                
                        .card {{
                            background: #f5faff;
                            border: 1px solid #d7e7f5;
                            border-radius: 16px;
                            padding: 22px;
                        }}
                
                        .card .label {{
                            color: #607d97;
                            font-size: 14px;
                            margin-bottom: 8px;
                        }}
                
                        .card .value {{
                            color: #0f3b63;
                            font-size: 34px;
                            font-weight: 800;
                        }}
                
                        .section-title {{
                            color: #0f2f4f;
                            font-size: 21px;
                            margin-bottom: 16px;
                            padding-bottom: 10px;
                            border-bottom: 2px solid #cfe3f5;
                        }}
                
                        .engine-list {{
                            display: grid;
                            gap: 14px;
                        }}
                
                        .engine-row {{
                            display: grid;
                            grid-template-columns: 120px 1fr 70px;
                            gap: 14px;
                            align-items: center;
                            padding: 14px;
                            border-radius: 14px;
                            background: #fbfdff;
                            border: 1px solid #e1edf7;
                        }}
                
                        .engine-name {{
                            font-weight: 700;
                            color: #183c5b;
                        }}
                
                        .bar {{
                            height: 12px;
                            background: #dcecf8;
                            border-radius: 999px;
                            overflow: hidden;
                        }}
                
                        .bar-fill {{
                            height: 100%;
                            background: linear-gradient(90deg, #4aa3df, #0f5f9c);
                            border-radius: 999px;
                        }}
                
                        .engine-count {{
                            text-align: right;
                            font-weight: 800;
                            color: #0f3b63;
                        }}
                
                        .footer {{
                            margin-top: 28px;
                            padding: 18px;
                            text-align: center;
                            color: #55718a;
                            background: #f1f7fc;
                            border-radius: 14px;
                            font-size: 14px;
                        }}
                
                        @media (max-width: 700px) {{
                            body {{ padding: 12px; }}
                            .hero {{ padding: 26px 22px; }}
                            .content {{ padding: 22px; }}
                            .summary {{ grid-template-columns: 1fr; }}
                            .engine-row {{ grid-template-columns: 1fr; }}
                            .engine-count {{ text-align: left; }}
                        }}
                    </style>
                </head>
                <body>
                    <main class=""report"">
                        <header class=""hero"">
                            <small>Informe generado el {DateTime.Now}</small>
                            <h1>Informe general de ITV</h1>
                            <p>Resumen de citas registradas y distribución de vehículos por tipo de motor.</p>
                        </header>
                
                        <section class=""content"">
                            <div class=""summary"">
                                <article class=""card"">
                                    <div class=""label"">Total de citas</div>
                                    <div class=""value"">{TotalCitas}</div>
                                </article>
                                <article class=""card"">
                                    <div class=""label"">Promedio de cilindrada</div>
                                    <div class=""value"">{PromedioCilindradaCitas} cc</div>
                                </article>
                            </div>
                
                            <h2 class=""section-title"">Vehículos por tipo de motor</h2>
                
                            <div class=""engine-list"">
                                <div class=""engine-row"">
                                    <div class=""engine-name"">Gasolina</div>
                                    <div class=""bar""><div class=""bar-fill"" style=""width: {porcentajeGasolina}%;""></div></div>
                                    <div class=""engine-count"">{TotalVehiculosMotorGasolina}</div>
                                </div>
                
                                <div class=""engine-row"">
                                    <div class=""engine-name"">Diésel</div>
                                    <div class=""bar""><div class=""bar-fill"" style=""width: {porcentajeDiesel}%;""></div></div>
                                    <div class=""engine-count"">{TotalVehiculosMotorDiesel}</div>
                                </div>
                
                                <div class=""engine-row"">
                                    <div class=""engine-name"">Híbrido</div>
                                    <div class=""bar""><div class=""bar-fill"" style=""width: {porcentajeHibrido}%;""></div></div>
                                    <div class=""engine-count"">{TotalVehiculosMotorHibrido}</div>
                                </div>
                
                                <div class=""engine-row"">
                                    <div class=""engine-name"">Eléctrico</div>
                                    <div class=""bar""><div class=""bar-fill"" style=""width: {porcentajeElectrico}%;""></div></div>
                                    <div class=""engine-count"">{TotalVehiculosMotorElectrico}</div>
                                </div>
                            </div>
                
                            <div class=""footer"">
                                Sistema de Gestión ITV · Informe automático
                            </div>
                        </section>
                    </main>
                </body>
                </html>
            ";
            
            _logger.Debug("Se ha generado correctamente el informe html.");
            return Result.Success<string, DomainError>(html);
        } catch (Exception e) {
            _logger.Error($"Han ocurrido errores generando el html, mensaje de error: {e.Message}");
            return Result.Failure<string, DomainError>(ReportErrors.GenerationErrors(e.Message));
        }
    }

    /// <inheritdoc cref="IReportService.GuardarInformeHtml" />
    public Result<bool, DomainError> GuardarInformeHtml(string html, string fileName) {
        var directory = _reportDirectory;

        try {
            if (!Directory.Exists(_reportDirectory)) Directory.CreateDirectory(directory);

            var filePath = Path.Combine(directory, fileName);
            File.WriteAllText(filePath, html, Encoding.UTF8);
            
            _logger.Debug("Se ha guardado correctamente el informe html.");
            return Result.Success<bool, DomainError>(true);
        } catch (Exception e) {
            _logger.Error($"Han ocurrido errores guardando el html, mensaje de error: {e.Message}");
            return Result.Failure<bool, DomainError>(ReportErrors.SaveError(e.Message));
        }
    }

    /// <inheritdoc cref="IReportService.GuardarInformePdf" />
    public Result<bool, DomainError> GuardarInformePdf(string html, string fileName) {
        var directory = _reportDirectory;

        try {
            if (!Directory.Exists(_reportDirectory)) Directory.CreateDirectory(directory);

            var filePath = Path.Combine(directory, fileName);
            File.WriteAllText(filePath, html, Encoding.UTF8);
            
            var converter = new HtmlToPdf();
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.MarginTop = 10;
            converter.Options.MarginBottom = 10;
            converter.Options.MarginLeft = 10;
            converter.Options.MarginRight = 10;

            var doc = converter.ConvertHtmlString(html);
            doc.Save(filePath);
            doc.Close();
            
            _logger.Debug("Se ha guardado correctamente el informe pdf.");
            return Result.Success<bool, DomainError>(true);
        } catch (Exception e) {
            _logger.Error($"Han ocurrido errores guardando el pdf, mensaje de error: {e.Message}");
            return Result.Failure<bool, DomainError>(ReportErrors.SaveError(e.Message));
        }
    }
}