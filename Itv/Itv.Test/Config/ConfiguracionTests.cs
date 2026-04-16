using FluentAssertions;
using Itv.Config;

namespace Itv.Test.Config;

[TestFixture]
public class ConfiguracionTests {

    [TestFixture]
    public sealed class Propiedades() {

        [Test]
        public void Locale_RetornaEspaña() {
            //Act
            var locale = Configuracion.Locale;
            
            //Assert
            locale.Should().NotBeNull();
            locale.Name.Should().Be("es-ES");
        }
    }
}