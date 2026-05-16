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
        public void GetAll_DeleteInclude_RetornaDatosValidos() {
            //Arrange
            _repository.Create(new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = new DateTime(2022, 05, 10), FechaInspeccion = DateTime.Today, CreateAt = DateTime.Now, UpdateAt = null, IsDelete = false });
            _repository.Create(new Cita { Id = 2, Matricula = "9999XYZ", Marca = "Audi", Modelo = "AAA3", Cilindrada = 2000, Motor = Motor.Gasolina, DniDueño = "87654321X", FechaMatriculacion = new DateTime(2020, 01, 01), FechaInspeccion = DateTime.Today, CreateAt = DateTime.Now, UpdateAt = null, IsDelete = false });
            _repository.Delete(1);
            
            //Act
            var res = _repository.GetAll();
            
            //Assert
            res.Should().NotBeNull();
            res.Should().HaveCount(2);
        }
        
        [Test]
        public void GetAll_DeleteNoInclude_RetornaDatosValidos() {
            //Arrange
            _repository.Create(new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = new DateTime(2022, 05, 10), FechaInspeccion = DateTime.Today, CreateAt = DateTime.Now, UpdateAt = null, IsDelete = false });
            _repository.Create(new Cita { Id = 2, Matricula = "9999XYZ", Marca = "Audi", Modelo = "AAA3", Cilindrada = 2000, Motor = Motor.Gasolina, DniDueño = "87654321X", FechaMatriculacion = new DateTime(2020, 01, 01), FechaInspeccion = DateTime.Today, CreateAt = DateTime.Now, UpdateAt = null, IsDelete = false });            
            _repository.Delete(1);

            //Act
            var res = _repository.GetAll(1, 5, false);
            
            //Assert
            res.Should().NotBeNull();
            res.Should().HaveCount(1);
        }
                
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
        public void GetByDateMatricula_DeleteInclude_RetornaSuccess() {
            //Arrange
            var cita = _repository.Create( new Cita { Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today });
            
            //Act
            var res = _repository.GetByDateMatricula(DateTime.Today.AddDays(-5), null);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().NotBeNull();
            res.Value.Select(c => c.Matricula == "1234BBB").Any().Should().BeTrue();
        }
        
        [Test]
        public void GetByDateMatricula_DeleteNoInclude_RetornaSuccess() {
            //Arrange
            var cita = _repository.Create( new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today });
            _repository.Delete(1);
            
            //Act
            var res = _repository.GetByDateMatricula(DateTime.Today.AddDays(-5), null, false);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().BeEmpty();
        }
        
        [Test]
        public void GetByMotor_DeleteInclude_RetornaSuccess() {
            //Arrange
            var c = _repository.Create( new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Hibrido, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today });
            _repository.Delete(c.Value.Id);
            _repository.Create( new Cita { Matricula = "1235BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Hibrido, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today });
            _repository.Create( new Cita { Matricula = "433BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today });

            
            //Act
            var res = _repository.GetByTipoMotor(Motor.Hibrido);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().NotBeNull();
            res.Value.Should().HaveCount(2);
        }
        
        [Test]
        public void GetByMotor_DeleteNoInclude_RetornaSuccess() {
            //Arrange
            var c = _repository.Create( new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Hibrido, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today });
            _repository.Delete(c.Value.Id);
            _repository.Create( new Cita { Matricula = "1235BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Hibrido, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today });
            _repository.Create( new Cita { Matricula = "433BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today });

            
            //Act
            var res = _repository.GetByTipoMotor(Motor.Hibrido, false);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().NotBeNull();
            res.Value.Should().HaveCount(1);
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
        public void Create_CitaRepetida_RetornaFallo() {
            //Arrange
            var cita = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = true };
            var cita2 = new Cita { Id = 2, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = true };
            _repository.Create(cita);
            
            //Act
            var res = _repository.Create(cita2);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Contains($"El cita no se puede crear, el vehiculo proporcionado ya tiene una fecha de matriculacion({cita.FechaMatriculacion}) el mismo dia.").Should().BeTrue();
        }

        [Test]
        public void Create_DniDueñoRepetido_RetornaFallo() {
            //Assert
            var cita1 = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today.AddDays(2), CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            var cita2 = new Cita { Id = 2, Matricula = "4321BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today.AddDays(2), CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            var cita3 = new Cita { Id = 3, Matricula = "1324BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today.AddDays(2), CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            var citaFallo = new Cita { Id = 4, Matricula = "2413BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today.AddDays(2), CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
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
            var cita2 = new Cita { Id = 2, Matricula = "4321BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            _repository.Create(cita1);
            cita2 = _repository.Create(cita2).Value;
            
            
            //Act
            var res = _repository.Update(cita2.Id, cita1);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Contains("no es valida por que ya existe").Should().BeTrue();
        }
        
        [Test]
        public void Update_CitaDniDueño_RetornaFallo() {
            //Assert
            var cita1 = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today.AddDays(2), CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            var cita2 = new Cita { Id = 2, Matricula = "4321BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today.AddDays(2), CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            var cita3 = new Cita { Id = 3, Matricula = "1324BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today.AddDays(2), CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            var citaFallo = new Cita { Id = 4, Matricula = "2413BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = DateTime.Today, FechaInspeccion = DateTime.Today.AddDays(2), CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            _repository.Create(cita1);
            _repository.Create(cita2);
            _repository.Create(cita3);
            
            
            //Act
            var res = _repository.Update(1, citaFallo);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Contains("ha superado el maximo de vehiculos").Should().BeTrue();
        }
        
        [Test]
        public void Update_CitaRepetida_RetornaFallo() {
            //Arrange
            var cita = new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = true };
            var cita2 = new Cita { Id = 2, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = true };
            _repository.Create(cita);
            
            //Act
            var res = _repository.Update(1, cita2);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Contains($"El cita no se puede crear, el vehiculo proporcionado ya tiene una fecha de matriculacion({cita.FechaMatriculacion}) el mismo dia.").Should().BeTrue();
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