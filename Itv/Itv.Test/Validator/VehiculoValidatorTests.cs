using FluentAssertions;
using Itv.Enums;
using Itv.Models;
using Itv.Validator;

namespace Itv.Test.Validator;

[TestFixture]
public class VehiculoValidatorTests {

    [TestFixture]
    public sealed class CasosValidos {

        [SetUp]
        public void SetUp() {
            _validator = new VehiculoValidator();
        }

        private VehiculoValidator _validator = null!;

        [Test]
        public void Validate_VehiculoValido_RetornaSuccess() {
            //Arrange
            var vehiculo = new Vehiculo {
                Id = 1,
                Matricula = "1234BBB",
                Marca = "marca",
                Modelo = "modelo",
                Cilindrada = 4,
                Motor = Motor.Diesel,
                DniDueño = "12345678Z",
                IsDelete = false
            };
            
            //Act
            var res = _validator.Validate(vehiculo);
            
            //Assert
            res.IsSuccess.Should().BeTrue(); 
        }

        [TestCase(Motor.Diesel)]
        [TestCase(Motor.Electrico)]
        [TestCase(Motor.Hibrido)]
        [TestCase(Motor.Gasolina)]
        public void Validate_TodosMotores_RetornaSuccess(Motor motor) {
            
        }
    }
    
    [TestFixture]
    public sealed class CasosInvalidos {
        
        [SetUp]
        public void SetUp() {
            _validator = new VehiculoValidator();
        }

        private VehiculoValidator _validator = null!;

    }

}