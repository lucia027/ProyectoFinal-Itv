using CSharpFunctionalExtensions;
using FluentAssertions;
using Itv.Enums;
using Itv.Errors;
using Itv.Errors.Common;
using Itv.Models;
using Itv.Services.ImportExport;
using Itv.Storage.Common;
using Moq;

namespace Itv.Test.Services.ImportExport;

[TestFixture]
public class ImportExportServiceTests {

    [TestFixture]
    public class CasosValidos : ImportExportServiceTests {
        
        private Mock<IStorage<Cita>> _storage = null!;
        private ImportExportService _service = null!;

        [SetUp]
        public void SetUp() {
            _storage = new Mock<IStorage<Cita>>();
            _service = new ImportExportService(_storage.Object);
        }
        
        [Test]
        public void ExportarDatos_CitasValidas_RetornaSuccess() {
            // Arrange
            var path = "citas.json";
            var cita1 = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var cita2 = new Cita { Id = 2, Matricula = "4321BBB", Marca = "Seat", Modelo = "Leon", Cilindrada = 1600, Motor = Motor.Gasolina, DniDueño = "87654321X", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var citas = new List<Cita> { cita1, cita2 };

            _storage.Setup(s => s.Salvar(It.Is<List<Cita>>(l => l.Count == 2 && l.Any(c => c.Id == 1) && l.Any(c => c.Id == 2)), path)).Returns(Result.Success<bool, DomainError>(true));

            // Act
            var res = _service.ExportarDatos(citas, path);

            // Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().Be(2);

            // Verify
            _storage.Verify(s => s.Salvar(It.Is<List<Cita>>(l => l.Count == 2 && l.Any(c => c.Id == 1) && l.Any(c => c.Id == 2)), path), Times.Once);
        }

        [Test]
        public void ExportarDatos_ListaVacia_RetornaSuccess() {
            // Arrange
            var path = "citas.json";
            var citas = new List<Cita>();
            _storage.Setup(s => s.Salvar(It.Is<List<Cita>>(l => l.Count == 0), path)).Returns(Result.Success<bool, DomainError>(true));

            // Act
            var res = _service.ExportarDatos(citas, path);

            // Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().Be(0);

            // Verify
            _storage.Verify(s => s.Salvar(It.Is<List<Cita>>(l => l.Count == 0), path), Times.Once);
        }

        [Test]
        public void ImportarDatos_PathValido_RetornaSuccess() {
            // Arrange
            var path = "citas.json";
            var cita1 = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var cita2 = new Cita { Id = 2, Matricula = "4321BBB", Marca = "Seat", Modelo = "Leon", Cilindrada = 1600, Motor = Motor.Gasolina, DniDueño = "87654321X", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var citas = new List<Cita> { cita1, cita2 };
            _storage.Setup(s => s.Cargar(path)).Returns(Result.Success<IEnumerable<Cita>, DomainError>(citas));

            // Act
            var res = _service.ImportarDatos(path);

            // Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().HaveCount(2);
            res.Value.Should().Contain(c => c.Matricula == "1234BBB");
            res.Value.Should().Contain(c => c.Matricula == "4321BBB");

            // Verify
            _storage.Verify(s => s.Cargar(path), Times.Once);
        }

        [Test]
        public void ExportarDatosSistema_CitasValidas_RetornaSuccess() {
            // Arrange
            var cita1 = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var cita2 = new Cita { Id = 2, Matricula = "4321BBB", Marca = "Seat", Modelo = "Leon", Cilindrada = 1600, Motor = Motor.Gasolina, DniDueño = "87654321X", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var citas = new List<Cita> { cita1, cita2 };
            _storage.Setup(s => s.Salvar(It.Is<List<Cita>>(l => l.Count == 2), string.Empty)).Returns(Result.Success<bool, DomainError>(true));

            // Act
            var res = _service.ExportarDatosSistema(citas);

            // Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().Be(2);

            // Verify
            _storage.Verify(s => s.Salvar(It.Is<List<Cita>>(l => l.Count == 2), string.Empty), Times.Once);
        }

        [Test]
        public void ImportarDatosSistema_PathValido_RetornaCitas() {
            // Arrange
            var path = "citas.json";
            var cita1 = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var cita2 = new Cita { Id = 2, Matricula = "4321BBB", Marca = "Seat", Modelo = "Leon", Cilindrada = 1600, Motor = Motor.Gasolina, DniDueño = "87654321X", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var citas = new List<Cita> { cita1, cita2 };

            _storage.Setup(s => s.Cargar(path)).Returns(Result.Success<IEnumerable<Cita>, DomainError>(citas));

            // Act
            var res = _service.ImportarDatosSistema(path);

            // Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().HaveCount(2);

            // Verify
            _storage.Verify(s => s.Cargar(path), Times.Once);
        }
    }

    [TestFixture]
    public class CasosInvalidos : ImportExportServiceTests {
        
        private Mock<IStorage<Cita>> _storage = null!;
        private ImportExportService _service = null!;

        [SetUp]
        public void SetUp() {
            _storage = new Mock<IStorage<Cita>>();
            _service = new ImportExportService(_storage.Object);
        }
        
        [Test]
        public void ExportarDatos_StorageFalla_RetornaFallo() {
            // Arrange
            var path = "citas.json";
            var cita = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var citas = new List<Cita> { cita };
            var error = CitaErrors.NotFoundCitasError();
            _storage.Setup(s => s.Salvar(It.Is<List<Cita>>(l => l.Count == 1), path)).Returns(Result.Failure<bool, DomainError>(error));

            // Act
            var res = _service.ExportarDatos(citas, path);

            // Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Should().Contain("No se han encontrado citas");

            // Verify
            _storage.Verify(s => s.Salvar(It.Is<List<Cita>>(l => l.Count == 1), path), Times.Once);
        }

        [Test]
        public void ImportarDatos_StorageFalla_RetornaFallo() {
            // Arrange
            var path = "citas.json";
            var error = CitaErrors.NotFoundCitasError();
            _storage.Setup(s => s.Cargar(path)).Returns(Result.Failure<IEnumerable<Cita>, DomainError>(error));

            // Act
            var res = _service.ImportarDatos(path);

            // Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Should().Contain("No se han encontrado citas");

            // Verify
            _storage.Verify(s => s.Cargar(path), Times.Once);
        }

        [Test]
        public void ExportarDatosSistema_StorageFalla_RetornaFallo() {
            // Arrange
            var cita = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var citas = new List<Cita> { cita };
            var error = CitaErrors.NotFoundCitasError();
            _storage.Setup(s => s.Salvar(It.Is<List<Cita>>(l => l.Count == 1), string.Empty)).Returns(Result.Failure<bool, DomainError>(error));

            // Act
            var res = _service.ExportarDatosSistema(citas);

            // Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Should().Contain("No se han encontrado citas");

            // Verify
            _storage.Verify(s => s.Salvar(It.Is<List<Cita>>(l => l.Count == 1), string.Empty), Times.Once);
        }

        [Test]
        public void ImportarDatosSistema_StorageFalla_RetornaFallo() {
            // Arrange
            var path = "citas.json";
            var error = CitaErrors.NotFoundCitasError();
            _storage.Setup(s => s.Cargar(path)).Returns(Result.Failure<IEnumerable<Cita>, DomainError>(error));

            // Act
            var res = _service.ImportarDatosSistema(path);

            // Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Should().Contain("No se han encontrado citas");

            // Verify
            _storage.Verify(s => s.Cargar(path), Times.Once);
        }
    }
}