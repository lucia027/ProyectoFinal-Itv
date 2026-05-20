using CSharpFunctionalExtensions;
using FluentAssertions;
using Itv.Cache;
using Itv.Enums;
using Itv.Errors;
using Itv.Errors.Common;
using Itv.Models;
using Itv.Repository.Common;
using Itv.Services.Citas;
using Itv.Validator.Common;
using Moq;

namespace Itv.Test.Services.Citas;

[TestFixture]
public class CitaServiceTests {

    [TestFixture]
    public class CasosValidos : CitaServiceTests {
        
        private Mock<IValidator<Cita>> _validator = null!;
        private Mock<ICitaRepository> _repository = null!;
        private Mock<ICache<int, Cita>> _cache = null!;
        private CitaService _service = null!;

        [SetUp]
        public void SetUp() {
            _validator = new Mock<IValidator<Cita>>();
            _repository = new Mock<ICitaRepository>();
            _cache = new Mock<ICache<int, Cita>>();

            _service = new CitaService(_validator.Object, _repository.Object, _cache.Object);
        }
        
        [Test]
        public void GetAll_CitasExistentes_RetornaListado() {
            // Arrange
            var cita1 = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var cita2 = new Cita { Id = 2, Matricula = "4321BBB", Marca = "Seat", Modelo = "Leon", Cilindrada = 1600, Motor = Motor.Gasolina, DniDueño = "87654321X", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var citas = new List<Cita> { cita1, cita2 };

            _repository.Setup(r => r.GetAll(1, 5, true, "")).Returns(citas);

            // Act
            var res = _service.GetAll();

            // Assert
            res.Should().NotBeNull();
            res.Should().HaveCount(2);
            res.Should().Contain(c => c.Matricula == "1234BBB");
            res.Should().Contain(c => c.Matricula == "4321BBB");
            
            //Verify
            _repository.Verify(r => r.GetAll(1, 5, true, ""), Times.Once);
        }

        [Test]
        public void GetById_CitaEnCache_RetornaSuccess() {
            // Arrange
            var cita = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };

            _cache.Setup(c => c.Get(cita.Id)).Returns(cita);

            // Act
            var res = _service.GetById(cita.Id);

            // Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().BeEquivalentTo(cita);
            
            //Verify
            _cache.Verify(c => c.Get(cita.Id), Times.Once);
            _repository.Verify(r => r.GetById(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void GetByDateInspeccion_CitasExistentes_RetornaSuccess() {
            // Arrange
            var cita1 = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var cita2 = new Cita { Id = 2, Matricula = "4321BBB", Marca = "Seat", Modelo = "Leon", Cilindrada = 1600, Motor = Motor.Gasolina, DniDueño = "87654321X", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var citas = new List<Cita> { cita1, cita2 };

            _repository.Setup(r => r.GetByDateInspeccion(DateTime.Today, DateTime.Today.AddDays(7), true)).Returns(Result.Success<IEnumerable<Cita>, DomainError>(citas));

            // Act
            var res = _service.GetByDateInspeccion(DateTime.Today, DateTime.Today.AddDays(7), true);

            // Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().HaveCount(2);
            
            //Verify
            _repository.Verify(r => r.GetByDateInspeccion(DateTime.Today, DateTime.Today.AddDays(7), true), Times.Once);
        }

        [Test]
        public void GetByTipoMotor_CitasExistentes_RetornaSuccess() {
            // Arrange
            var cita1 = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var cita2 = new Cita { Id = 2, Matricula = "4321BBB", Marca = "Seat", Modelo = "Leon", Cilindrada = 1600, Motor = Motor.Diesel, DniDueño = "87654321X", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var citas = new List<Cita> { cita1, cita2 };

            _repository.Setup(r => r.GetByTipoMotor(Motor.Diesel, true)).Returns(Result.Success<IEnumerable<Cita>, DomainError>(citas));

            // Act
            var res = _service.GetByTipoMotor(Motor.Diesel);

            // Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().HaveCount(2);
            res.Value.Should().OnlyContain(c => c.Motor == Motor.Diesel);
            
            //Verify
            _repository.Verify(r => r.GetByTipoMotor(Motor.Diesel, true), Times.Once);
        }

        [Test]
        public void Create_CitaValida_RetornaSuccess() {
            // Arrange
            var cita = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var citaCreada = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };

            _validator.Setup(v => v.Validate(cita)).Returns(Result.Success<Cita, DomainError>(cita));
            _repository.Setup(r => r.Create(cita)).Returns(Result.Success<Cita, DomainError>(citaCreada));

            // Act
            var res = _service.Create(cita);

            // Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().BeEquivalentTo(citaCreada);
            
            //Verify
            _validator.Verify(v => v.Validate(cita), Times.Once);
            _repository.Verify(r => r.Create(cita), Times.Once);
            _cache.Verify(c => c.Add(citaCreada.Id, citaCreada), Times.Once);
        }

        [Test]
        public void Update_CitaValida_RetornaSuccess() {
            // Arrange
            var citaExistente = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var citaNueva = new Cita { Id = 1, Matricula = "4321BBB", Marca = "Seat", Modelo = "Leon", Cilindrada = 1600, Motor = Motor.Hibrido, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = DateTime.Today, IsDelete = false };

            _repository.Setup(r => r.GetById(citaExistente.Id)).Returns(Result.Success<Cita, DomainError>(citaExistente));
            _repository.Setup(r => r.Update(citaExistente.Id, citaNueva)).Returns(Result.Success<Cita, DomainError>(citaNueva));

            // Act
            var res = _service.Update(citaExistente.Id, citaNueva);

            // Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Matricula.Should().Be("4321BBB");
            res.Value.Motor.Should().Be(Motor.Hibrido);
            
            //Verify
            _repository.Verify(r => r.GetById(citaExistente.Id), Times.Once);
            _repository.Verify(r => r.Update(citaExistente.Id, citaNueva), Times.Once);
            _cache.Verify(c => c.Remove(citaExistente.Id), Times.Once);
            _cache.Verify(c => c.Add(citaExistente.Id, citaNueva), Times.Once);
        }

        [Test]
        public void Delete_CitaValida_RetornaSuccess() {
            // Arrange
            var cita = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var citaEliminada = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = DateTime.Today, IsDelete = true };

            _repository.Setup(r => r.GetById(cita.Id)).Returns(Result.Success<Cita, DomainError>(cita));
            _repository.Setup(r => r.Delete(cita.Id)).Returns(Result.Success<Cita, DomainError>(citaEliminada));

            // Act
            var res = _service.Delete(cita.Id);

            // Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.IsDelete.Should().BeTrue();
            
            //Verify
            _repository.Verify(r => r.GetById(cita.Id), Times.Once);
            _repository.Verify(r => r.Delete(cita.Id), Times.Once);
            _cache.Verify(c => c.Remove(cita.Id), Times.Once);
        }

        [Test]
        public void DeleteAll_CitasExistentes_EliminaTodas() {
            // Arrange
            _repository.Setup(r => r.DeleteAll()).Returns(true);

            // Act
            var res = _service.DeleteAll();

            // Assert
            res.Should().BeTrue();
            
            //Verify
            _repository.Verify(r => r.DeleteAll(), Times.Once);
        }
    }

    [TestFixture]
    public class CasosInvalidos : CitaServiceTests {
        
        private Mock<IValidator<Cita>> _validator = null!;
        private Mock<ICitaRepository> _repository = null!;
        private Mock<ICache<int, Cita>> _cache = null!;
        private CitaService _service = null!;

        [SetUp]
        public void SetUp() {
            _validator = new Mock<IValidator<Cita>>();
            _repository = new Mock<ICitaRepository>();
            _cache = new Mock<ICache<int, Cita>>();

            _service = new CitaService(_validator.Object, _repository.Object, _cache.Object);
        }
        
        [Test]
        public void GetById_CitaNoExiste_RetornaFallo() {
            // Arrange
            var error = CitaErrors.NotFoundCitasError();

            _cache.Setup(c => c.Get(66)).Returns((Cita?)null);
            _repository.Setup(r => r.GetById(66)).Returns(Result.Failure<Cita, DomainError>(error));

            // Act
            var res = _service.GetById(66);

            // Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Should().Contain("No se han encontrado citas");
            
            //Verify
            _cache.Verify(c => c.Get(66), Times.Once);
            _repository.Verify(r => r.GetById(66), Times.Once);
        }

        [Test]
        public void Create_CitaInvalida_RetornaFallo() {
            // Arrange
            var cita = new Cita { Id = 1, Matricula = "", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var error = CitaErrors.NotFoundCitasError();

            _validator.Setup(v => v.Validate(cita)).Returns(Result.Failure<Cita, DomainError>(error));

            // Act
            var res = _service.Create(cita);

            // Assert
            res.IsFailure.Should().BeTrue();
            
            //Verify
            _validator.Verify(v => v.Validate(cita), Times.Once);
            _repository.Verify(r => r.Create(It.IsAny<Cita>()), Times.Never);
            _cache.Verify(c => c.Add(It.IsAny<int>(), It.IsAny<Cita>()), Times.Never);
        }

        [Test]
        public void Create_RepositorioErroneo_RetornaFallo() {
            // Arrange
            var cita = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = null, IsDelete = false };
            var error = CitaErrors.NotFoundCitasError();

            _validator.Setup(v => v.Validate(cita)).Returns(Result.Success<Cita, DomainError>(cita));
            _repository.Setup(r => r.Create(cita)).Returns(Result.Failure<Cita, DomainError>(error));

            // Act
            var res = _service.Create(cita);

            // Assert
            res.IsFailure.Should().BeTrue();
            
            //Verify
            _validator.Verify(v => v.Validate(cita), Times.Once);
            _repository.Verify(r => r.Create(cita), Times.Once);
            _cache.Verify(c => c.Add(It.IsAny<int>(), It.IsAny<Cita>()), Times.Never);
        }

        [Test]
        public void Update_CitaInexistente_RetornaFallo() {
            // Arrange
            var citaNueva = new Cita { Id = 66, Matricula = "4321BBB", Marca = "Seat", Modelo = "Leon", Cilindrada = 1600, Motor = Motor.Hibrido, DniDueño = "12345678Z", FechaInspeccion = DateTime.Today, FechaMatriculacion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = DateTime.Today, IsDelete = false };

            _repository.Setup(r => r.GetById(66)).Returns(Result.Failure<Cita, DomainError>(CitaErrors.NotFoundCitasError()));

            // Act
            var res = _service.Update(66, citaNueva);

            // Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Should().Contain("No se han encontrado citas");
            
            //Verify
            _repository.Verify(r => r.GetById(66), Times.Once);
            _repository.Verify(r => r.Update(It.IsAny<int>(), It.IsAny<Cita>()), Times.Never);
            _cache.Verify(c => c.Remove(It.IsAny<int>()), Times.Never);
            _cache.Verify(c => c.Add(It.IsAny<int>(), It.IsAny<Cita>()), Times.Never);
        }

        [Test]
        public void Delete_CitaInexistente_RetornaFallo() {
            // Arrange
            var error = CitaErrors.NotFoundCitasError();

            _repository.Setup(r => r.GetById(66)).Returns(Result.Failure<Cita, DomainError>(error));

            // Act
            var res = _service.Delete(66);

            // Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Should().Contain("No se han encontrado citas");
            
            //Verify
            _repository.Verify(r => r.GetById(66), Times.Once);
            _repository.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
            _cache.Verify(c => c.Remove(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void DeleteAll_SinDatos_RetornaFallo() {
            // Arrange
            _repository.Setup(r => r.DeleteAll()).Returns(false);

            // Act
            var res = _service.DeleteAll();

            // Assert
            res.Should().BeFalse();
            
            //Verify
            _repository.Verify(r => r.DeleteAll(), Times.Once);
        }
    }
}