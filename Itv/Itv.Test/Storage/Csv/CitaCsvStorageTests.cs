using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Itv.Enums;
using Itv.Models;
using Itv.Storage.Csv;
using NUnit.Framework;

namespace Itv.Test.Storage.Csv;

[TestFixture]
public class CitaCsvStorageTests {

    [TestFixture]
    public sealed class CasosValidos {

        [TearDown]
        public void TearDown() {
            if (File.Exists(_tempPath)) {
                File.Delete(_tempPath);
            }
        }

        [SetUp]
        public void Setup() {
            _storage = new CitaCsvStorage();
            _tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");
        }

        private CitaCsvStorage _storage = null!;
        private string _tempPath;

        [Test]
        public void Salvar_DatosExistentes_SalvaDatos() {
            //Arrange
            var vehiculos = new List<Cita>() {
                new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false },
                new Cita { Id = 2, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false }
            };
            
            //Act
            var res = _storage.Salvar(vehiculos, _tempPath);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            File.Exists(_tempPath).Should().BeTrue();
        }

        [Test]
        public void Salvar_VehiculoValido_RetornaMismoVehiculo() {
            //Arrange
            var vehiculos = new List<Cita>() {
                new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false }
            };
            _storage.Salvar(vehiculos, _tempPath);
            
            //Act
            var res = _storage.Cargar(_tempPath);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.First().Should().BeOfType<Cita>();
            res.Value.First().Id.Should().Be(1);
            res.Value.First().Matricula.Should().Be("1234BBB");
            res.Value.First().Marca.Should().Be("Toyota");
            res.Value.First().Modelo.Should().Be("Corolla");
            res.Value.First().Cilindrada.Should().Be(1800);
            res.Value.First().Motor.Should().Be(Motor.Diesel);
            res.Value.First().DniDueño.Should().Be("12345678Z");
            res.Value.First().IsDelete.Should().BeFalse();
        }

        [Test]
        public void Salvar_ListaVacia_SalvaListaVacia() {
            //Assert
            var vehiculos = new List<Cita>();
            
            //Act
            var res = _storage.Salvar(vehiculos, _tempPath);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            File.Exists(_tempPath).Should().BeTrue();
        }

        [Test]
        public void Cargar_ListaVacia_RetornaListaVacia() {
            //Arrange
            var vehiculos = new List<Cita>();
            _storage.Salvar(vehiculos, _tempPath);
            
            //Act
            var res = _storage.Cargar(_tempPath);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().BeEmpty();
        }

        [Test]
        public void Cargar_DatosValidos_CargarDatos() {
            //Arrange
            var vehiculos = new List<Cita>() {
                new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false },
                new Cita { Id = 2, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false }
            };
            _storage.Salvar(vehiculos, _tempPath);
            
            //Act
            var res =  _storage.Cargar(_tempPath);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().HaveCount(vehiculos.Count);
            res.Value.First().Should().BeOfType<Cita>();
        }
    }

    [TestFixture]
    public sealed class CasosInvaidos {
        
        [TearDown]
        public void TearDown() {}

        [SetUp]
        public void SetUp() {
            _storage = new CitaCsvStorage();
            _tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");
        }
        
        private CitaCsvStorage _storage;
        private string _tempPath;

        [Test]
        public void Salvar_PathInvalido_RetornaFallo() {
            //Arrange
            var vehiculos = new List<Cita>() {
                new Cita { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false },
                new Cita { Id = 2, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false }
            };
            var pathInvalido = "/ruta/invalida/va/a/dar/falllo";
            
            //Act
            var res = _storage.Salvar(vehiculos, pathInvalido);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            File.Exists(_tempPath).Should().BeFalse();
        }
        
        [Test]
        public void Cargar_ArchivoInexistente_RetornaFallo() {
            //Assert
            var vehiculos = new List<Cita>();
            var rutaInvalida = "hola/soy/una/ruta/invalida/doy/fallo";
            _storage.Salvar(vehiculos, rutaInvalida);
            
            //Act
            var res = _storage.Cargar(rutaInvalida);
            
            //Assert
            res.IsFailure.Should().BeTrue();
        }
    }
}