using FluentAssertions;
using Itv.Enums;
using Itv.Models;
using Itv.Storage.Csv;

namespace Itv.Test.Storage.Csv;

[TestFixture]
public class VehiculoCsvStorageTests {

    [TestFixture]
    public sealed class CasosValidos {

        [TearDown]
        public void TearDown() {
            if(File.Exists(_tempPath)) File.Delete(_tempPath);
        }

        [SetUp]
        public void Setup() {
            _storage = new VehiculoCsvStorage();
            _tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");
        }

        private VehiculoCsvStorage _storage = null!;
        private string _tempPath;

        [Test]
        public void Salvar_DatosExistentes_SalvaDatos() {
            //Arrange
            var vehiculos = new List<Vehiculo>() {
                new Vehiculo { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false },
                new Vehiculo { Id = 2, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false }
            };
            
            //Act
            var res = _storage.Salvar(vehiculos, _tempPath);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            File.Exists(_tempPath).Should().BeTrue();
        }

        [Test]
        public void Cargar_DatosExistentes_CargaDatos() {
            //Arrange
            var vehiculos = new List<Vehiculo>() {
                new Vehiculo { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false },
                new Vehiculo { Id = 2, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false }
            };
            _storage.Salvar(vehiculos, _tempPath);
            
            //Act
            var res = _storage.Cargar(_tempPath);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().HaveCount(2);
        }
    }
}