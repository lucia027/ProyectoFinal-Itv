using FluentAssertions;
using Itv.Enums;
using Itv.Models;
using Itv.Storage.Json;

namespace Itv.Test.Storage.Json;

[TestFixture]
public class VehiculoJsonStorageTests {

    [TestFixture]
    public sealed class CasosValidos() {
        [SetUp]
        public void SetUp() {
            _storage = new VehiculoJsonStorage();
            _tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
        }

        [TearDown]
        public void TearDown() {
            if(File.Exists(_tempPath)) File.Delete(_tempPath);
        }
        
        private VehiculoJsonStorage _storage;
        private string _tempPath;

        [Test]
        public void Salvar_DatosExistentes_SalvarDatos() {
            //Arrange
            var vehiculos = new List<Vehiculo>() {
                new Vehiculo { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false },
                new Vehiculo { Id = 2, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false }
            };
            
            //Act
            var res = _storage.Salvar(vehiculos,  _tempPath);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            File.Exists(_tempPath).Should().BeTrue();
        }
        
        
        [Test]
        public void Cargar_DatosValidos_CargarDatos() {
            //Arrange
            var vehiculos = new List<Vehiculo>() {
                new Vehiculo { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false },
                new Vehiculo { Id = 2, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false }
            };
            _storage.Salvar(vehiculos, _tempPath);
            
            //Act
            var res =  _storage.Cargar(_tempPath);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().HaveCount(vehiculos.Count);
            res.Value.First().Should().BeOfType<Vehiculo>();
        }

        [Test]
        public void Salvar_ArchivoVacio_ColeccionVacia() {
            //Assert
            var vacio = new List<Vehiculo>();
            
            //Act
            var res = _storage.Salvar(vacio, _tempPath);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            File.Exists(_tempPath).Should().BeTrue(); 
        }
    }
    

    [TestFixture]
    public sealed class CasosInvalidos {
        [SetUp]
        public void SetUp() {
            _storage = new VehiculoJsonStorage();
            _tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
        }

        [TearDown]
        public void TearDown() {
            if(File.Exists(_tempPath)) File.Delete(_tempPath);
        }
        
        private VehiculoJsonStorage _storage;
        private string _tempPath;

        [Test]
        public void Salvar_PathInvalida_RetornaFallo() {
            //Arrange
            var vehiculos = new List<Vehiculo>();
            var rutaMala = "ruta/invalida/x/y/aa";
            
            //Act
            var res = _storage.Salvar(vehiculos, rutaMala);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            File.Exists(rutaMala).Should().BeFalse();
        }

        [Test]
        public void Cargar_ArchivoInexistente_RetornaFallo() {
            //Arrange
            var vehiculos = new List<Vehiculo>();
            var rutaMala = "ruta/invalida/x/y/aa";
            _storage.Salvar(vehiculos, rutaMala);
            
            //Act
            var res = _storage.Cargar(rutaMala);
            
            //Assert
            res.IsFailure.Should().BeTrue();
        }
    }
}