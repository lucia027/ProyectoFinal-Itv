using FluentAssertions;
using Itv.Cache;

namespace Itv.Test.Cache;

[TestFixture]
public class CacheLruTests {

    [TestFixture]
    public sealed class CasosPositivos {
        [SetUp]
        public void SetUp() { 
            _cache = new CacheLru<int, string>(3);
        }
        
        private CacheLru<int, string> _cache = null!;

        [Test]
        public void Add_ElementoValido_GuardarElemento() {
            //Act
            _cache.Add(5, "cinco");
            
            //Assert
            _cache.Get(5).Should().Be("cinco");
        }

        [Test]
        public void Add_VariosElementosValdos_GuardarElementos() {
            //Act
            _cache.Add(4, "cuatro");
            _cache.Add(5, "cinco");
            _cache.Add(6, "seis");

            //Assert
            _cache.Get(4).Should().Be("cuatro");
            _cache.Get(5).Should().Be("cinco");
            _cache.Get(6).Should().Be("seis");
        }

        [Test]
        public void Add_VariosElementosValidos_EliminarMasAntiguo() {
            //Act
            _cache.Add(4, "cuatro");
            _cache.Add(5, "cinco");
            _cache.Add(6, "seis");
            _cache.Add(7, "siete");
            
            //Assert
            _cache.Get(4).Should().BeNull();
            _cache.Get(5).Should().Be("cinco");
            _cache.Get(6).Should().Be("seis");
            _cache.Get(7).Should().Be("siete");
        }

        [Test]
        public void Add_ElementoClaveDuplicada_DeberiaReemplazarlo() {
            //Act
            _cache.Add(5, "cinco");
            _cache.Add(5, "SOY OTRO ELEMENTO");

            //Assert
            _cache.Get(5).Should().Be("SOY OTRO ELEMENTO");
        }
        
        [Test]
        public void Add_ElementoNulo_GuardarNulo() {
            //Act
            _cache.Add(5, null!);
            
            //Assert
            _cache.Get(5).Should().BeNull();
        }

        [Test]
        public void Get_ElementoExistente_ActualizarOrden() {
            //Act
            _cache.Add(4, "cuatro");
            _cache.Add(5, "cinco");
            _cache.Add(6, "seis");
            _cache.Get(5);
            
            //Assert
            _cache.Get(5).Should().Be("cinco");
            _cache.Get(4).Should().Be("cuatro");
            _cache.Get(6).Should().Be("seis");        
        }

        [Test]
        public void Get_ElementoNoExistente_RetornaNulo() {
            //Act
            var res = _cache.Get(5);
            
            //Assert
            res.Should().BeNull();
        }

        [Test]
        public void Remove_ElementoExistente_RetornaVerdadero() {
            //Act
            _cache.Add(4, "cuatro");
            _cache.Add(5, "cinco");

            var res = _cache.Remove(4);
            
            //Assert
            res.Should().BeTrue();
            _cache.Get(5).Should().Be("cinco");
            _cache.Get(4).Should().BeNull();
        }

        [Test]
        public void Remove_ElementoNoExistente_RetornaFalse() {
            //Act
            var res = _cache.Remove(5);
            
            //Assert
            res.Should().BeFalse();
        }
    }

    [TestFixture]
    public sealed class CasosNegativos {

        [Test]
        public void ConstructorTamañoCero_LanzarExcepcion() {
            //Arrange + Act
            var accion = () => new CacheLru<int, string>(0);
            
            //Assert
            accion.Should().Throw<ArgumentException>();
        }
    }
}