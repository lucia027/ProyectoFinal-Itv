using System.Globalization;
using FluentAssertions;
using Itv.Enums;
using Itv.Models;
using Itv.Services.Report;

namespace Itv.Test.Services.Report;

[TestFixture]
public class ReportServiceTests {

    [TestFixture]
    public class CasosValidos : ReportServiceTests {
        
        [SetUp]
        public void SetUp() {
            _tempDirPath = Path.Combine(Path.GetTempPath(), $"ReportTests_{Guid.NewGuid()}");
            Directory.CreateDirectory(_tempDirPath);

            _service = new ReportService(_tempDirPath);

            _originalCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("es-ES");
            CultureInfo.CurrentUICulture = new CultureInfo("es-ES");
        }

        [TearDown]
        public void TearDown() {
            CultureInfo.CurrentCulture = _originalCulture;
            CultureInfo.CurrentUICulture = _originalCulture;

            if (Directory.Exists(_tempDirPath)) {
                try {
                    Directory.Delete(_tempDirPath, true);
                } catch {
                    // Ignorar errores de limpieza
                }
            }
        }

        private ReportService _service = null!;
        private CultureInfo _originalCulture = null!;
        private string _tempDirPath = null!;
        
        [Test]
        public void GenerarInforme_CitasValidas_RetornaInformeCorrecto() {
            // Arrange
            var cita1 = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var cita2 = new Cita { Id = 2, Matricula = "4321BBB", Marca = "Seat", Modelo = "Leon", Cilindrada = 1600, Motor = Motor.Gasolina, DniDueño = "87654321X", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var cita3 = new Cita { Id = 3, Matricula = "5678CCC", Marca = "Ford", Modelo = "Focus", Cilindrada = 1400, Motor = Motor.Hibrido, DniDueño = "11111111A", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var cita4 = new Cita { Id = 4, Matricula = "8765DDD", Marca = "Tesla", Modelo = "Model 3", Cilindrada = 0, Motor = Motor.Electrico, DniDueño = "22222222B", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var citas = new List<Cita> { cita1, cita2, cita3, cita4 };

            // Act
            var res = _service.GenerarInforme(citas);

            // Assert
            res.Should().NotBeNull();
            res.TotalCitas.Should().Be(4);
            res.TotalVehiculosMotorDiesel.Should().Be(1);
            res.TotalVehiculosMotorGasolina.Should().Be(1);
            res.TotalVehiculosMotorHibrido.Should().Be(1);
            res.TotalVehiculosMotorElectrico.Should().Be(1);
            res.PromedioCilindradaCitas.Should().Be(1200);
        }

        [Test]
        public void GenerarInforme_ListaVacia_RetornaInformeVacio() {
            // Arrange
            var citas = new List<Cita>();

            // Act
            var res = _service.GenerarInforme(citas);

            // Assert
            res.Should().NotBeNull();
            res.TotalCitas.Should().Be(0);
            res.TotalVehiculosMotorDiesel.Should().Be(0);
            res.TotalVehiculosMotorGasolina.Should().Be(0);
            res.TotalVehiculosMotorHibrido.Should().Be(0);
            res.TotalVehiculosMotorElectrico.Should().Be(0);
            res.PromedioCilindradaCitas.Should().Be(0);
        }

        [Test]
        public void GenerarInformeHtml_CitasValidas_RetornaHtmlConDatos() {
            // Arrange
            var cita1 = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var cita2 = new Cita { Id = 2, Matricula = "4321BBB", Marca = "Seat", Modelo = "Leon", Cilindrada = 1600, Motor = Motor.Gasolina, DniDueño = "87654321X", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var cita3 = new Cita { Id = 3, Matricula = "5678CCC", Marca = "Ford", Modelo = "Focus", Cilindrada = 1400, Motor = Motor.Hibrido, DniDueño = "11111111A", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var cita4 = new Cita { Id = 4, Matricula = "8765DDD", Marca = "Tesla", Modelo = "Model 3", Cilindrada = 0, Motor = Motor.Electrico, DniDueño = "22222222B", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var citas = new List<Cita> { cita1, cita2, cita3, cita4 };

            // Act
            var res = _service.GenerarInformeHtml(citas);

            // Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().NotBeNullOrWhiteSpace();
            res.Value.Should().Contain("<!DOCTYPE html>");
            res.Value.Should().Contain("Informe general de ITV");
            res.Value.Should().Contain("Total de citas");
            res.Value.Should().Contain(">4<");
            res.Value.Should().Contain("Promedio de cilindrada");
            res.Value.Should().Contain("1200");
            res.Value.Should().Contain("Gasolina");
            res.Value.Should().Contain("Diésel");
            res.Value.Should().Contain("Híbrido");
            res.Value.Should().Contain("Eléctrico");
        }

        [Test]
        public void GenerarInformeHtml_CitasValidas_RetornaBarrasConPorcentajes() {
            // Arrange
            var cita1 = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var cita2 = new Cita { Id = 2, Matricula = "4321BBB", Marca = "Seat", Modelo = "Leon", Cilindrada = 1600, Motor = Motor.Gasolina, DniDueño = "87654321X", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var cita3 = new Cita { Id = 3, Matricula = "5678CCC", Marca = "Ford", Modelo = "Focus", Cilindrada = 1400, Motor = Motor.Hibrido, DniDueño = "11111111A", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var cita4 = new Cita { Id = 4, Matricula = "8765DDD", Marca = "Tesla", Modelo = "Model 3", Cilindrada = 0, Motor = Motor.Electrico, DniDueño = "22222222B", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var citas = new List<Cita> { cita1, cita2, cita3, cita4 };

            // Act
            var res = _service.GenerarInformeHtml(citas);

            // Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().Contain("width: 25%;");
        }

        [Test]
        public void GuardarInformeHtml_HtmlValido_GuardaInformeCorrectamente() {
            // Arrange
            var html = "<html><body><h1>Informe ITV</h1></body></html>";
            var fileName = "informe_test.html";
            var expectedPath = Path.Combine(_tempDirPath, fileName);

            // Act
            var res = _service.GuardarInformeHtml(html, fileName);

            // Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().BeTrue();
            File.Exists(expectedPath).Should().BeTrue();
            File.ReadAllText(expectedPath).Should().Be(html);
        }

        [Test]
        public void GuardarInformeHtml_DirectorioNoExiste_CreaDirectorioYGuardaInforme() {
            // Arrange
            Directory.Delete(_tempDirPath, true);

            var html = "<html><body><h1>Informe ITV</h1></body></html>";
            var fileName = "informe_test.html";
            var expectedPath = Path.Combine(_tempDirPath, fileName);

            // Act
            var res = _service.GuardarInformeHtml(html, fileName);

            // Assert
            res.IsSuccess.Should().BeTrue();
            Directory.Exists(_tempDirPath).Should().BeTrue();
            File.Exists(expectedPath).Should().BeTrue();
            File.ReadAllText(expectedPath).Should().Be(html);
        }

        [Test]
        public void GuardarInformePdf_HtmlValido_GuardaPdfCorrectamente() {
            // Arrange
            var html = "<html><body><h1>Informe ITV</h1><p>Prueba PDF</p></body></html>";
            var fileName = "informe_test.pdf";
            var expectedPath = Path.Combine(_tempDirPath, fileName);

            // Act
            var res = _service.GuardarInformePdf(html, fileName);

            // Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().BeTrue();
            File.Exists(expectedPath).Should().BeTrue();
            new FileInfo(expectedPath).Length.Should().BeGreaterThan(0);
        }
    }

    [TestFixture]
    public class CasosInvalidos : ReportServiceTests {
        
            [SetUp]
    public void SetUp() {
        _tempDirPath = Path.Combine(Path.GetTempPath(), $"ReportTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDirPath);

        _service = new ReportService(_tempDirPath);

        _originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("es-ES");
        CultureInfo.CurrentUICulture = new CultureInfo("es-ES");
    }

    [TearDown]
    public void TearDown() {
        CultureInfo.CurrentCulture = _originalCulture;
        CultureInfo.CurrentUICulture = _originalCulture;

        if (Directory.Exists(_tempDirPath)) {
            try {
                Directory.Delete(_tempDirPath, true);
            } catch {
                // Ignorar errores de limpieza
            }
        }
    }

    private ReportService _service = null!;
    private CultureInfo _originalCulture = null!;
    private string _tempDirPath = null!;
        
        [Test]
        public void GenerarInformeHtml_ListaVacia_RetornaFallo() {
            // Arrange
            var citas = new List<Cita>();

            // Act
            var res = _service.GenerarInformeHtml(citas);

            // Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void GuardarInformeHtml_NombreInvalido_RetornaFallo() {
            // Arrange
            var html = "<html><body>Informe ITV</body></html>";
            var fileName = "archivo\0invalido.html";

            // Act
            var res = _service.GuardarInformeHtml(html, fileName);

            // Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void GuardarInformePdf_NombreInvalido_RetornaFallo() {
            // Arrange
            var html = "<html><body>Informe ITV</body></html>";
            var fileName = "archivo\0invalido.pdf";

            // Act
            var res = _service.GuardarInformePdf(html, fileName);

            // Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void GuardarInformeHtml_DirectorioInvalido_RetornaFallo() {
            // Arrange
            var rutaArchivoComoDirectorio = Path.Combine(Path.GetTempPath(), $"ReportTests_{Guid.NewGuid()}.txt");
            File.WriteAllText(rutaArchivoComoDirectorio, "esto es un archivo, no un directorio");

            var service = new ReportService(rutaArchivoComoDirectorio);
            var html = "<html><body>Informe ITV</body></html>";
            var fileName = "informe.html";

            try {
                // Act
                var res = service.GuardarInformeHtml(html, fileName);

                // Assert
                res.IsFailure.Should().BeTrue();
                res.Error.Message.Should().NotBeNullOrWhiteSpace();
            } finally {
                if (File.Exists(rutaArchivoComoDirectorio)) {
                    File.Delete(rutaArchivoComoDirectorio);
                }
            }
        }

        [Test]
        public void GuardarInformePdf_DirectorioInvalido_RetornaFallo() {
            // Arrange
            var rutaArchivoComoDirectorio = Path.Combine(Path.GetTempPath(), $"ReportTests_{Guid.NewGuid()}.txt");
            File.WriteAllText(rutaArchivoComoDirectorio, "esto es un archivo, no un directorio");

            var service = new ReportService(rutaArchivoComoDirectorio);
            var html = "<html><body>Informe ITV</body></html>";
            var fileName = "informe.pdf";

            try {
                // Act
                var res = service.GuardarInformePdf(html, fileName);

                // Assert
                res.IsFailure.Should().BeTrue();
                res.Error.Message.Should().NotBeNullOrWhiteSpace();
            } finally {
                if (File.Exists(rutaArchivoComoDirectorio)) {
                    File.Delete(rutaArchivoComoDirectorio);
                }
            }
        }
    }
}