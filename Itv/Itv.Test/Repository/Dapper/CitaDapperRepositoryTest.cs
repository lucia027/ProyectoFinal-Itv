using System.Data;
using FluentAssertions;
using Itv.Enums;
using Itv.Models;
using Itv.Repository.Dapper;
using Microsoft.Data.Sqlite;

namespace Itv.Test.Repository.Dapper;

[TestFixture]
public class CitaDapperRepositoryTest {

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
        public void GetAll_DatosValidos_RetornaDatosValidos() {
            //Arrange
            _repository.Create(new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = new DateTime(2022, 05, 10), FechaInspeccion = DateTime.Today, CreateAt = DateTime.Now, UpdateAt = null, IsDelete = false });
            _repository.Create(new Cita { Id = 2, Matricula = "9999XYZ", Marca = "Audi", Modelo = "A3", Cilindrada = 2000, Motor = Motor.Gasolina, DniDueño = "87654321X", FechaMatriculacion = new DateTime(2020, 01, 01), FechaInspeccion = DateTime.Today, CreateAt = DateTime.Now, UpdateAt = null, IsDelete = false });            
            
            //Act
            var res = _repository.GetAll();
            
            //Assert
            res.Should().NotBeNull();
            res.Should().HaveCount(2);
        }

        [Test]
        public void GetById_DatoValido_RetornaSucces() {
            //Arrange
            var c1 = _repository.Create( new Cita { Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today });
            
            //Act
            var res = _repository.GetById(c1.Value.Id);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().NotBeNull();
            res.Value.Matricula.Should().Be("1234BBB");
        }
    }

    [TestFixture]
    public sealed class CasosInvalidos {
    }
    
    
    
}