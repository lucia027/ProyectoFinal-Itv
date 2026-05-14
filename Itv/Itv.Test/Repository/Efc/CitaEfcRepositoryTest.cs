using System.Runtime.InteropServices.JavaScript;
using FluentAssertions;
using Itv.Entity;
using Itv.Enums;
using Itv.Models;
using Itv.Repository.Efc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Itv.Test.Repository;

[TestFixture]
public class CitaEfcRepositoryTest {

    [TestFixture]
    public class CasosValidos {
        private SqliteConnection _connection = null!;
        
        [SetUp]
        public void SetUp() {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;
            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();
            _repository = new CitaEfcRepository(_context, dropData: true, seedData: false);        
        }

        [TearDown]
        public void TearDown() {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _connection.Close();
            _connection.Dispose();
        }

        private AppDbContext _context = null!;
        private CitaEfcRepository _repository = null!;

        [Test]
        public void GetAll_ConEliminados_RetornaListadoEliminados() {
            //Arrange
            var v = _repository.Create(new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false});
            _repository.Delete(v.Value.Id);
            _repository.Create(new Cita { Id = 2, Matricula = "5432BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false});
            
            //Act
            var res = _repository.GetAll();
            
            //Assert
            res.Should().NotBeNull();
            res.Should().HaveCount(2);
            res.First(ve => ve.Matricula == "1234BBB").IsDelete.Should().BeTrue();
            res.First(ve => ve.Matricula == "5432BBB").IsDelete.Should().BeFalse();
        }

        [Test]
        public void GetAll_SinEliminados_RetornaNoEliminados() {
            //Arrange
            var cita = _repository.Create(new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false});
            _repository.Delete(cita.Value.Id);
            _repository.Create(new Cita { Id = 2, Matricula = "5432BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false});
            
            //Act
            var res = _repository.GetAll(1, 1, false);
            
            //Assert
            res.Should().NotBeNull();
            res.Should().HaveCount(1);
            res.Where(v => v.IsDelete).Any().Should().BeTrue();
        }

        [Test]
        public void GetById_CitaYaCreada_RetornaCita() {
            //Arrange
            var cita = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = true };
            cita = _repository.Create(cita).Value;
            
            //Act
            var res = _repository.GetById(cita.Id);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Should().NotBeNull();
            res.Value.Matricula.Should().Be("1234BBB");
        }

        [Test]
        public void Create_CitaValida_CreaCita() {
            //Arrange
            var cita = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = true };
            
            //Act
            var res = _repository.Create(cita);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Should().NotBeNull();
            res.Value.Matricula.Should().Be("1234BBB");
            res.Value.IsDelete.Should().BeFalse();
            res.Value.UpdateAt.Should().BeNull();
            res.Value.CreateAt.Should().Be(DateTime.Today);
        }

        [Test]
        public void Update_CitaValida_ActualizaCita() {
            //Arrange
            var cita = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false }; 
            cita  = _repository.Create(cita).Value;
            var citaNuevo = new Cita { Id = 1, Matricula = "4321BBB", Marca = "nuevo", Modelo = "nuevo", Cilindrada = 500, Motor = Motor.Hibrido, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false }; 


            //Act
            var res = _repository.Update(cita.Id, citaNuevo);
            
            //Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeTrue();
            res.Value.Id.Should().Be(cita.Id);
            res.Value.Matricula.Should().Be(citaNuevo.Matricula);
            res.Value.UpdateAt.Should().Be(DateTime.Today);
            res.Value.IsDelete.Should().BeFalse();
        }

        [Test]
        public void Delete_CitaValida_MarcaCitaEliminada() {
            var cita = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false }; 
            cita  = _repository.Create(cita).Value;
            
            //Act
            var res = _repository.Delete(cita.Id);
            var citaEncontrado = _repository.GetById(cita.Id);

            
            //Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeTrue();
            res.Value.IsDelete.Should().BeTrue();
            citaEncontrado.IsSuccess.Should().BeTrue();
        }
        
        [Test]
        public void Delete_CitaValida_EliminaCita() {
            var cita = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false }; 
            cita  = _repository.Create(cita).Value;
            
            //Act
            var res = _repository.DeleteHard(cita.Id);
            var citaEncontrado = _repository.GetById(cita.Id);

            
            //Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeTrue();
            citaEncontrado.IsFailure.Should().BeTrue();
        }

        [Test]
        public void DeleteAll_CitasValidas_EliminaTodas() {
            //Arrange
            var cita1 = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            var cita2 = new Cita { Id = 2, Matricula = "4321BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            _repository.Create(cita1);
            _repository.Create(cita2);

            //Act
            var res = _repository.DeleteAll();
            var citasEncontrados = _repository.GetAll();
            
            //Assert
            res.Should().BeTrue();
            citasEncontrados.Should().BeEmpty();
        }
    }

    [TestFixture]
    public class CasosInvalidos {

        private SqliteConnection _connection = null!;
        
        [SetUp]
        public void SetUp() {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;
            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();
            _repository = new CitaEfcRepository(_context, dropData: true, seedData: false);        
        }

        [TearDown]
        public void TearDown() {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _connection.Close();
            _connection.Dispose();
        }

        private AppDbContext _context = null!;
        private CitaEfcRepository _repository = null!;

        [Test]
        public void GetAll_AlmacenVacio_RetornaVacio() {
            //Act
            var res = _repository.GetAll();
            
            //Assert
            res.Should().BeEmpty();
        }

        [Test]
        public void GetById_CitaInexistente_RetornaFallo() {
            //Act
            var res = _repository.GetById(66);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Contains("No se puede encontrar el cita con el id").Should().BeTrue();
        }

        [Test]
        public void Create_MatriculaInvalida_RetornaFallo() {
            //Arrange
            var cita1 = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), FechaMatriculacion = DateTime.Today };
            var cita2 = new Cita { Id = 2, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), FechaMatriculacion = DateTime.Today};
            _repository.Create(cita1);
            
            //Act
            var res = _repository.Create(cita2);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Contains("el vehiculo proporcionado ya tiene una fecha de matriculacion").Should().BeTrue();
        }

        [Test]
        public void Create_DniDueñoRepetido_RetornaFallo() {
            //Assert
            var cita1 = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today.AddDays(2), CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            var cita2 = new Cita { Id = 1, Matricula = "4321BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today.AddDays(2), CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            var cita3 = new Cita { Id = 1, Matricula = "1324BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today.AddDays(2), CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            var citaFallo = new Cita { Id = 1, Matricula = "2413BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today.AddDays(2), CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            _repository.Create(cita1);
            _repository.Create(cita2);
            _repository.Create(cita3);
            
            
            //Act
            var res = _repository.Create(citaFallo);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Contains("ha superado el maximo de vehiculos").Should().BeTrue();
        }

        [Test]
        public void Update_CitaInexistente_RetornaFallo() {
            //Arrange
            var cita = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            
            //Act
            var res = _repository.Update(66, cita);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Contains("No se puede encontrar el cita con el id").Should().BeTrue();
        }
        
        [Test]
        public void Update_CitaMatriculaRepetida_RetornaFallo() {
            //Arrange
            var cita1 = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            var cita2 = new Cita { Id = 1, Matricula = "4321BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            _repository.Create(cita1);
            cita2 = _repository.Create(cita2).Value;
            
            
            //Act
            var res = _repository.Update(cita2.Id, cita1);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Contains("no es valida por que ya existe").Should().BeTrue();
        }

        [Test]
        public void Delete_CitaInexistente_RetornaFallo() {
            //Act
            var res = _repository.Delete(66);

            res.IsFailure.Should().BeTrue();
            res.Error.Message.Contains("No se puede encontrar el cita con el id").Should().BeTrue();
        }
        
        [Test]
        public void DeleteHard_CitaInexistente_RetornaFallo() {
            //Act
            var res = _repository.DeleteHard(66);

            res.IsFailure.Should().BeTrue();
            res.Error.Message.Contains("No se puede encontrar el cita con el id").Should().BeTrue();
        }
    }
}