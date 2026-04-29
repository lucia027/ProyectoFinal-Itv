using System;
using FluentAssertions;
using Itv.Dto;
using Itv.Enums;
using Itv.Mappers;
using Itv.Models;
using NUnit.Framework;

namespace Itv.Test.Mappers;

[TestFixture]
public class CitaMapperTests {
    
    [TestFixture]
    public sealed class CasosValidos() {

        [SetUp]
        public void Setup() {
            _cita = new Cita {
                Id= 1,
                Matricula = "1234BBB",
                Marca = "Toyota",
                Modelo = "Corolla",
                Cilindrada = 1800,
                Motor = Motor.Diesel,
                DniDueño = "12345678Z",
                CreateAt = new DateTime(2026, 04, 16),
                UpdateAt = new DateTime(2026, 04, 16),
                IsDelete = false
            };

            _citaDto = new CitaDto {
                Id= 2,
                Matricula = "1234BBB",
                Marca = "Toyota",
                Modelo = "Corolla",
                Cilindrada = 1800,
                Motor = "Diesel",
                DniDueño = "12345678Z",
                CreateAt = "16/04/2026",
                UpdateAt = "16/04/2026",
                IsDelete = false
            };
        }

        private Cita _cita = null!;
        private CitaDto _citaDto = null!;

        [Test]
        public void ToModel_VehiculoDto_ConvierteCorrectamente() {
            //Act
            var res = _citaDto.ToModel();
            
            //Assert
            res.Should().NotBeNull();
            res.Id.Should().Be(2);
            res.Matricula.Should().Be("1234BBB");
            res.Marca.Should().Be("Toyota");
            res.Modelo.Should().Be("Corolla");
            res.Cilindrada.Should().Be(1800);
            res.Motor.Should().Be(Motor.Diesel);
            res.DniDueño.Should().Be("12345678Z");
        }

        [Test]
        public void ToDto_Vehiculo_ConvierteCorrectamente() {
            //Act
            var res = _cita.ToDto();
            
            //Arrange
            res.Should().NotBeNull();
            res.Id.Should().Be(1);
            res.Matricula.Should().Be("1234BBB");
            res.Marca.Should().Be("Toyota");
            res.Modelo.Should().Be("Corolla");
            res.Cilindrada.Should().Be(1800);
            res.Motor.Should().Be("Diesel");
            res.DniDueño.Should().Be("12345678Z");
        }
    }

    [TestFixture]
    public sealed class CasosInvalidos() {

        [Test]
        public void ToModel_VehiculoDtoInvalido_ConvierteValoresPorDefecto() {
            //Arrange
            var vehiculoDto = new CitaDto {
                Id = 1,
                Matricula = "1234BBB",
                Marca = "Toyota",
                Modelo = "Corolla",
                Cilindrada = -10,
                Motor = "JAJAJAJ",
                DniDueño = "12345678Z",
                CreateAt = "16/04/2026",
                UpdateAt = "16/04/2026",
                IsDelete = false
            };
            
            //Act
            var res = vehiculoDto.ToModel();
            
            //Assert
            res.Should().NotBeNull();
            res.Cilindrada.Should().Be(0);
            res.Motor.Should().Be(Motor.Diesel);
        }
    }
}