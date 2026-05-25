using FluentAssertions;
using Itv.Enums;
using Itv.Errors;
using Itv.Models;
using Itv.Validator;

namespace Itv.Test.Validator;

[TestFixture]
public class CitaValidatorTests {

    [TestFixture]
    public sealed class CasosValidos {

        [SetUp]
        public void SetUp() {
            _validator = new CitaValidator();
        }

        private CitaValidator _validator = null!;

        [Test]
        public void Validate_CitaValida_RetornaSuccess() {
            //Arrange
            var cita = new Cita {
                Matricula = "1234BBB",
                Marca = "Toyota",
                Modelo = "Corolla",
                Cilindrada = 1800,
                Motor = Motor.Diesel,
                DniDueño = "12345678Z"
            };
            
            //Act
            var res = _validator.Validate(cita);
            
            //Assert
            res.IsSuccess.Should().BeTrue(); 
        }

        [TestCase(Motor.Diesel)]
        [TestCase(Motor.Electrico)]
        [TestCase(Motor.Hibrido)]
        [TestCase(Motor.Gasolina)]
        public void Validate_TodosMotores_RetornaSuccess(Motor motor) {
            //Arrange
            var cita = new Cita {
                Matricula = "1234BBB",
                Marca = "Toyota",
                Modelo = "Corolla",
                Cilindrada = 1800,
                Motor = motor,
                DniDueño = "12345678Z"
            };
            
            //Act
            var res = _validator.Validate(cita);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void Validate_FechaMatriculacionActualValida_RetornaSuccess() {
            //Arrange
            var cita = new Cita {
                Matricula = "1234BBB",
                Marca = "Toyota",
                Modelo = "Corolla",
                Cilindrada = 1800,
                Motor = Motor.Diesel,
                DniDueño = "12345678Z",
                FechaMatriculacion = DateTime.Today,
                FechaInspeccion = DateTime.Today
            };
            
            //Act
            var res = _validator.Validate(cita);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
        }
        
        [Test]
        public void Validate_FechaMatriculacionPasadaValida_RetornaSuccess() {
            //Arrange
            var cita = new Cita {
                Matricula = "1234BBB",
                Marca = "Toyota",
                Modelo = "Corolla",
                Cilindrada = 1800,
                Motor = Motor.Diesel,
                DniDueño = "12345678Z",
                FechaMatriculacion = DateTime.Today.AddDays(-25),
                FechaInspeccion = DateTime.Today
            };
            
            //Act
            var res = _validator.Validate(cita);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
        }

        [Test]
        public void Validate_FechaInspeccionActualValida_RetornaSuccess() {
            //Arrange
            var cita = new Cita {
                Matricula = "1234BBB",
                Marca = "Toyota",
                Modelo = "Corolla",
                Cilindrada = 1800,
                Motor = Motor.Diesel,
                DniDueño = "12345678Z",
                FechaMatriculacion = DateTime.Today,
                FechaInspeccion = DateTime.Today
            };
            
            //Act
            var res = _validator.Validate(cita);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
        }
        
        [Test]
        public void Validate_FechaInspeccion30DiasValida_RetornaSuccess() {
            //Arrange
            var cita = new Cita {
                Matricula = "1234BBB",
                Marca = "Toyota",
                Modelo = "Corolla",
                Cilindrada = 1800,
                Motor = Motor.Diesel,
                DniDueño = "12345678Z",
                FechaMatriculacion = DateTime.Today,
                FechaInspeccion = DateTime.Today.AddDays(30)
            };
            
            //Act
            var res = _validator.Validate(cita);
            
            //Assert
            res.IsSuccess.Should().BeTrue();
        }
    }
    
    [TestFixture]
    public sealed class CasosInvalidos {
        
        [SetUp]
        public void SetUp() {
            _validator = new CitaValidator();
        }

        private CitaValidator _validator = null!;


        [TestCase("XXXXXXX")]
        [TestCase("1111X")]
        [TestCase("LLL1111")]
        [TestCase("XX1111X")]
        public void Validate_MatriculaErronea_RetornaInvalid(string matricula) {
            //Arrange
            var cita = new Cita {
                Matricula = matricula,
                Marca = "Toyota",
                Modelo = "Corolla",
                Cilindrada = 1800,
                Motor = Motor.Diesel,
                DniDueño = "12345678Z"
            };
            
            //Act
            var res = _validator.Validate(cita);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Should().BeOfType<CitaError.Validation>();
            
            var validationError = res.Error as CitaError.Validation;
            validationError!.Errores.Should().Contain("La matricula proporcionada no cumple el formato");
        }

        
        [TestCase(null)]
        [TestCase("")]
        public void Validate_MarcaVaciaONula_RetornaInvalid(string? marca) {
            //Arrange
            var cita = new Cita {
                Matricula = "1234BBB",
                Marca = marca!,
                Modelo = "Corolla",
                Cilindrada = 1800,
                Motor = Motor.Diesel,
                DniDueño = "12345678Z"
            };
            
            //Act
            var res = _validator.Validate(cita);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Should().BeOfType<CitaError.Validation>();
            
            var validationError = res.Error as CitaError.Validation;
            validationError!.Errores.Should().Contain("La marca es nula o esta vacia");
        }
        
        [TestCase(null)]
        [TestCase("")]
        public void Validate_ModeloVacioONulo_RetornaInvalid(string? modelo) {
            //Arrange
            var cita = new Cita {
                Matricula = "1234BBB",
                Marca = "Toyota",
                Modelo = modelo!,
                Cilindrada = 1800,
                Motor = Motor.Diesel,
                DniDueño = "12345678Z"
            };
            
            //Act
            var res = _validator.Validate(cita);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Should().BeOfType<CitaError.Validation>();
            
            var validationError = res.Error as CitaError.Validation;
            validationError!.Errores.Should().Contain("El modelo es nulo o esta vacio");
        }

        [Test]
        public void Validate_CilindradaNegativa_RetornaInvalid() {
            //Arrange
            var cita = new Cita {
                Matricula = "1234BBB",
                Marca = "Toyota",
                Modelo = "Corolla",
                Cilindrada = -1,
                Motor = Motor.Diesel,
                DniDueño = "12345678Z"
            };
            
            //Act
            var res = _validator.Validate(cita);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Should().BeOfType<CitaError.Validation>();
            
            var validationError = res.Error as CitaError.Validation;
            validationError!.Errores.Should().Contain("La cilindrada no puede ser negativa");
            
        }

        [Test]
        public void Validate_MotorInvalido_RetornaInvalid() {
            //Arrange
            var cita = new Cita {
                Matricula = "1234BBB",
                Marca = "Toyota",
                Modelo = "Corolla",
                Cilindrada = 1800,
                Motor = (Motor)66,
                DniDueño = "12345678Z"
            };
            
            //Act
            var res = _validator.Validate(cita);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Should().BeOfType<CitaError.Validation>();
            
            var validationError = res.Error as CitaError.Validation;
            validationError!.Errores.Should().Contain("El tipo de motor no es valido");
        }

        [TestCase("XXXXXXXXX")]
        [TestCase("Z12345678")]
        [TestCase("12345678z")]
        [TestCase("1234567Z")]
        public void Validate_DniInvalido_RetornaInvalid(string dni) {
            //Arrange
            var cita = new Cita {
                Matricula = "1234BBB",
                Marca = "Toyota",
                Modelo = "Corolla",
                Cilindrada = 1800,
                Motor = Motor.Diesel,
                DniDueño = dni
            };
            
            //Act
            var res = _validator.Validate(cita);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Should().BeOfType<CitaError.Validation>();
            
            var validationError = res.Error as CitaError.Validation;
            validationError!.Errores.Should().Contain("El dni del dueño no cumple el formato");
        }
        
        [Test]
        public void Validate_FechaMatriculacionFutura_RetornaFallo() {
            //Arrange
            var cita = new Cita {
                Matricula = "1234BBB",
                Marca = "Toyota",
                Modelo = "Corolla",
                Cilindrada = 1800,
                Motor = Motor.Diesel,
                DniDueño = "12345678Z",
                FechaMatriculacion = DateTime.Today.AddDays(1),
                FechaInspeccion = DateTime.Today
            };
            
            //Act
            var res = _validator.Validate(cita);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Should().BeOfType<CitaError.Validation>();

            var validationError = res.Error as CitaError.Validation;
            validationError!.Errores.Should().Contain("La fecha de matriculacion no puede estar en futuro.");
        }
        
        [Test]
        public void Validate_FechaInspeccionMas30Dias_RetornaFallo() {
            //Arrange
            var cita = new Cita {
                Matricula = "1234BBB",
                Marca = "Toyota",
                Modelo = "Corolla",
                Cilindrada = 1800,
                Motor = Motor.Diesel,
                DniDueño = "12345678Z",
                FechaMatriculacion = DateTime.Today,
                FechaInspeccion = DateTime.Today.AddDays(31)
            };
            
            //Act
            var res = _validator.Validate(cita);
            
            //Assert
            res.IsFailure.Should().BeTrue();
            res.Error.Should().BeOfType<CitaError.Validation>();

            var validationError = res.Error as CitaError.Validation;
            validationError!.Errores.Should().Contain("La fecha de inspeccion no puede ser superior a 30 dias.");
        }
    }
}