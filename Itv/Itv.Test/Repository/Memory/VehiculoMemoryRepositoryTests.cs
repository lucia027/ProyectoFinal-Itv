using FluentAssertions;
using Itv.Enums;
using Itv.Models;
using Itv.Repository.Memory;

namespace Itv.Test.Repository.Memory;

[TestFixture]
public class VehiculoMemoryRepositoryTests {

    [TestFixture]
    public class CasosValidos {
        [SetUp]
        public void SetUp() {
            _repository = new VehiculoMemoryRepository(true, false);
        }

        private VehiculoMemoryRepository _repository = null!;

        [Test]
        public void GetAll_ConEliminados_RetornaListadoEliminados() {
            //Arrange
            var v = _repository.Create(new Vehiculo { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false});
            _repository.Delete(v.Value.Id);
            _repository.Create(new Vehiculo { Id = 2, Matricula = "5432BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false});
            
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
            var vehiculo = _repository.Create(new Vehiculo { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false});
            _repository.Delete(vehiculo.Value.Id);
            _repository.Create(new Vehiculo { Id = 2, Matricula = "5432BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false});
            
            //Act
            var res = _repository.GetAll(1, 1, false);
            
            //Assert
            res.Should().NotBeNull();
            res.Should().HaveCount(1);
            res.Where(v => v.IsDelete).Any().Should().BeFalse();
        }

        [Test]
        public void GetById_VehiculoYaCreado_RetornaVehiculo() {
            //Arrange
            var vehiculo = new Vehiculo { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = true };
            vehiculo = _repository.Create(vehiculo).Value;
            
            //Act
            var res = _repository.GetById(vehiculo.Id);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Should().NotBeNull();
            res.Value.Matricula.Should().Be("1234BBB");
        }

        [Test]
        public void Create_VehiculoValido_CreaElVehiculo() {
            //Arrange
            var vehiculo = new Vehiculo { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = true };
            
            //Act
            var res = _repository.Create(vehiculo);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Should().NotBeNull();
            res.Value.Matricula.Should().Be("1234BBB");
            res.Value.IsDelete.Should().BeFalse();
            res.Value.UpdateAt.Should().BeNull();
            res.Value.CreateAt.Should().Be(DateTime.Today);
        }

        [Test]
        public void Update_VehiculoValido_ActualizaVehiculo() {
            //Arrange
            var vehiculo = new Vehiculo { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false }; 
            vehiculo  = _repository.Create(vehiculo).Value;
            var vehiculoNuevo = new Vehiculo { Id = 1, Matricula = "4321BBB", Marca = "nuevo", Modelo = "nuevo", Cilindrada = 500, Motor = Motor.Hibrido, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false }; 


            //Act
            var res = _repository.Update(vehiculo.Id, vehiculoNuevo);
            
            //Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeTrue();
            res.Value.Id.Should().Be(vehiculo.Id);
            res.Value.Matricula.Should().Be(vehiculoNuevo.Matricula);
            res.Value.UpdateAt.Should().Be(DateTime.Today);
            res.Value.IsDelete.Should().BeFalse();
        }

        [Test]
        public void Delete_VehiculoValido_MarcaVehiculoEliminado() {
            var vehiculo = new Vehiculo { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false }; 
            vehiculo  = _repository.Create(vehiculo).Value;
            
            //Act
            var res = _repository.Delete(vehiculo.Id);
            var vehiculoEncontrado = _repository.GetById(vehiculo.Id);

            
            //Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeTrue();
            res.Value.IsDelete.Should().BeTrue();
            vehiculoEncontrado.IsSuccess.Should().BeTrue();
        }
        
        [Test]
        public void Delete_VehiculoValido_EliminaVehiculo() {
            var vehiculo = new Vehiculo { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false }; 
            vehiculo  = _repository.Create(vehiculo).Value;
            
            //Act
            var res = _repository.DeleteHard(vehiculo.Id);
            var vehiculoEncontrado = _repository.GetById(vehiculo.Id);

            
            //Assert
            res.Should().NotBeNull();
            res.IsSuccess.Should().BeTrue();
            vehiculoEncontrado.IsFailure.Should().BeTrue();
        }

        [Test]
        public void DeleteAll_VehiculosValidos_EliminaTodos() {
            //Arrange
            var vehiculo1 = new Vehiculo { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            var vehiculo2 = new Vehiculo { Id = 2, Matricula = "4321BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            _repository.Create(vehiculo1);
            _repository.Create(vehiculo2);

            //Act
            var res = _repository.DeleteAll();
            var vehiculosEncontrados = _repository.GetAll();
            
            //Assert
            res.Should().BeTrue();
            vehiculosEncontrados.Should().BeEmpty();
        }
    }

    [TestFixture]
    public class CasosInvalidos {
        [SetUp]
        public void SetUp() {
            _repository = new VehiculoMemoryRepository(true, false);
        }

        private VehiculoMemoryRepository _repository;

        [Test]
        public void GetAll_AlmacenVacio_RetornaVacio() {
            //Act
            var res = _repository.GetAll();
            
            //Assert
            res.Should().BeEmpty();
        }

        [Test]
        public void GetById_VehiculoInexistente_RetornaFallo() {
            //Act
            var res = _repository.GetById(66);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Contains("No se puede encontrar el vehiculo con el id").Should().BeTrue();
        }

        [Test]
        public void Create_MatriculaInvalida_RetornaFallo() {
            //Arrange
            var vehiculo = new Vehiculo { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            _repository.Create(vehiculo);
            
            //Act
            var res = _repository.Create(vehiculo);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Contains("no es valida por que ya existe").Should().BeTrue();
        }

        [Test]
        public void Create_DniDueñoRepetido_RetornaFallo() {
            //Assert
            var vehiculo1 = new Vehiculo { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            var vehiculo2 = new Vehiculo { Id = 1, Matricula = "4321BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            var vehiculo3 = new Vehiculo { Id = 1, Matricula = "1324BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            var vehiculoFallo = new Vehiculo { Id = 1, Matricula = "2413BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            _repository.Create(vehiculo1);
            _repository.Create(vehiculo2);
            _repository.Create(vehiculo3);
            
            
            //Act
            var res = _repository.Create(vehiculoFallo);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Contains("ha superado el maximo de vehiculos").Should().BeTrue();
        }

        [Test]
        public void Update_VehiculoInexistente_RetornaFallo() {
            //Arrange
            var vehiculo = new Vehiculo { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            
            //Act
            var res = _repository.Update(66, vehiculo);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Contains("No se puede encontrar el vehiculo con el id").Should().BeTrue();
        }
        
        [Test]
        public void Update_VehiculoMatriculaRepetida_RetornaFallo() {
            //Arrange
            var vehiculo1 = new Vehiculo { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            var vehiculo2 = new Vehiculo { Id = 1, Matricula = "4321BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false };
            _repository.Create(vehiculo1);
            vehiculo2 = _repository.Create(vehiculo2).Value;
            
            
            //Act
            var res = _repository.Update(vehiculo2.Id, vehiculo1);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Contains("no es valida por que ya existe").Should().BeTrue();
        }

        [Test]
        public void Delete_VehiculoInexistente_RetornaFallo() {
            //Act
            var res = _repository.Delete(66);

            res.IsFailure.Should().BeTrue();
            res.Error.Message.Contains("No se puede encontrar el vehiculo con el id").Should().BeTrue();
        }
        
        [Test]
        public void DeleteHard_VehiculoInexistente_RetornaFallo() {
            //Act
            var res = _repository.DeleteHard(66);

            res.IsFailure.Should().BeTrue();
            res.Error.Message.Contains("No se puede encontrar el vehiculo con el id").Should().BeTrue();
        }
    }
}