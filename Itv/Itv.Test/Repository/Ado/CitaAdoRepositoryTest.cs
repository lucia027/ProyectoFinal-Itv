using System.Data;
using FluentAssertions;
using Itv.Enums;
using Itv.Models;
using Itv.Repository.Dapper;
using Microsoft.Data.Sqlite;

namespace Itv.Test.Repository.Ado;

[TestFixture]
public class CitaAdoRepositoryTest {

    [TestFixture]
    public sealed class CasosValidos {

        [SetUp]
        public void SetUp() {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            _repository = new CitaDapperRepository(_connection);
        }

        [TearDown]
        public void TearDown() {
            _connection.Close();
            _connection.Dispose();
        }
        
        private IDbConnection _connection = null!;
        private CitaDapperRepository _repository = null!;


                
        [Test]
        public void GetById_CitaYaCreada_RetornaSuccess() {
            //Arrange
            var cita = _repository.Create( new Cita { Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today });
            
            //Act
            var res = _repository.GetById(cita.Value.Id);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().NotBeNull();
            res.Value.Matricula.Should().Be("1234BBB");
        }
        
        [Test]
        public void GetById_CitaYaCreada_RetornaSuccss() {
            //Arrange
            var cita = _repository.Create( new Cita { Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today });
            
            //Act
            var res = _repository.GetById(cita.Value.Id);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().NotBeNull();
            res.Value.Matricula.Should().Be("1234BBB");
        }

        [Test]
        public void Create_CitaValida_RetornaSuccess() {
            //Arrange
            var cita = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = DateTime.Today.AddDays(8), UpdateAt = DateTime.Now, IsDelete = true };
            
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
        public void Update_CitaValida_RetornaSuccess() {
            //Arrange
            var cita = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = new DateTime(2022, 05, 10), FechaInspeccion = DateTime.Today, CreateAt = DateTime.Now, UpdateAt = null, IsDelete = false }; 
            cita  = _repository.Create(cita).Value;
            var citaNuevo = new Cita { Id = 1, Matricula = "4321BBB", Marca = "nuevo", Modelo = "nuevo", Cilindrada = 500, Motor = Motor.Hibrido, DniDueño = "12345678Z", FechaMatriculacion = new DateTime(2023, 05, 10), FechaInspeccion = DateTime.Today, CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false }; 
            
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
            var cita = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today, CreateAt = DateTime.Today, UpdateAt = DateTime.Now, IsDelete = false };
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
    public sealed class CasosInvalidos {
    }
}