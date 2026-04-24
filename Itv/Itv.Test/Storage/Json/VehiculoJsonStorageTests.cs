using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Itv.Enums;
using Itv.Models;
using Itv.Storage.Json;
using NUnit.Framework;

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
        public void Salvar_VehiculoValido_RetornaMismoVehiculo() {
            //Arrange
            var vehiculos = new List<Vehiculo>() {
                new Vehiculo { Id = 1, Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", CreateAt = new DateTime(2026, 04, 16), UpdateAt = new DateTime(2026, 04, 16), IsDelete = false }
            };
            _storage.Salvar(vehiculos, _tempPath);
            
            //Act
            var res = _storage.Cargar(_tempPath);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            res.Value.First().Should().BeOfType<Vehiculo>();
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
            var vehiculos = new List<Vehiculo>();
            
            //Act
            var res = _storage.Salvar(vehiculos, _tempPath);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
            File.Exists(_tempPath).Should().BeTrue();
        }
        
        [Test]
        public void Cargar_ListaVacia_RetornaListaVacia() {
            //Arrange
            var vehiculos = new List<Vehiculo>();
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
        public void Cargar_JsonInvalido_RetornaFallo() {
            //Assert
            using var writer = new StreamWriter(_tempPath, false, Encoding.UTF8);
            writer.Write("fallando..");
            
            //Act
            var res = _storage.Cargar(_tempPath);
            
            //Assert
            res.IsFailure.Should().BeTrue();
        }
        
        [Test]
        public void Cargar_DatosNulos_RetornaFallo() {
            //Assert
            using var writer = new StreamWriter(_tempPath, false, Encoding.UTF8);
            writer.Write("null");
            
            //Act
            var res = _storage.Cargar(_tempPath);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Message.Contains("Los dtos no se han podido deserializar.");
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