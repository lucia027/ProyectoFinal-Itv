using System.IO;
using FluentAssertions;
using Itv.Config;
using NUnit.Framework;

namespace Itv.Test.Config;

[TestFixture]
public class ConfiguracionTests {

    [TestFixture]
    public sealed class Propiedades {

        [Test]
        public void Locale_RetornaEspanya() {
            //Act
            var locale = Configuracion.Locale;
            
            //Assert
            locale.Should().NotBeNull();
            locale.Name.Should().Be("es-ES");
        }

        [Test]
        public void TipoAlmacenamiento_RetornaTipoValido() {
            //Act
            var tipoAlmacenamiento = Configuracion.StorageType;
            
            //Assert
            tipoAlmacenamiento.Should().NotBeNull();
            tipoAlmacenamiento.Should().BeOneOf("json", "csv", "xml", "binary");
        }
        
        [Test]
        public void TipoRepositorio_RetornaTipoValido() {
            //Act
            var tipoRepo = Configuracion.RepositoryType;
            
            //Assert
            tipoRepo.Should().NotBeNull();
            tipoRepo.Should().BeOneOf("json", "memory", "efcore", "binary", "dapper", "ado");
        }

        [Test]
        public void TipoBackup_RetornaTipoValido() {
            //Act
            var tipoBackup = Configuracion.BackupFormat;
            
            //Assert
            tipoBackup.Should().NotBeNull();
            tipoBackup.Should().BeOneOf("json", "csv", "xml", "bin");
        }
        
        [Test]
        public void ConecctionString_RetornaNoNulo() {
            //Act
            var conecctionString = Configuracion.ConnectionString;
            
            //Assert
            conecctionString.Should().NotBeNull();
            conecctionString.Should().Contain("Data Source");
        }

        [Test]
        public void TamañoCache_RetornaMayorCero() {
            //Act
            var tamCache = Configuracion.CacheSize;
            
            //Assert
            tamCache.Should().BeGreaterThan(0);
        }

        [Test]
        public void DropData_RetornaBoleano() {
            //Act
            var dropData = Configuracion.DropData;
            
            //Assert
            (dropData || !dropData).Should().BeTrue();
        }

        [Test]
        public void SeedData_RetornaBoleano() {
            //Act
            var seedData = Configuracion.SeedData;
            
            //Arrange
            seedData.Should().BeTrue();
        }
    }

    [TestFixture]
    public sealed class DirectoriosArchivos {

        [Test]
        public void DataFolder_RetornaRutaValida() {
            //Act
            var dataFolder = Configuracion.DataFolder;
            
            //Assert
            dataFolder.Should().NotBeNull();
            Path.IsPathRooted(dataFolder).Should().BeTrue();
        }

        [Test]
        public void BackupDirectory_RetornaRutaValida() {
            //Act
            var backupDirectory = Configuracion.BackupDirectory;
            
            //Assert
            backupDirectory.Should().NotBeNull();
            Path.IsPathRooted(backupDirectory).Should().BeTrue();
        }

        [Test]
        public void ItvFile_RetornaArchivoValido() {
            //Act
            var file = Configuracion.ItvFile;
            
            //Assert
            file.Should().NotBeNull();
            file.Should().EndWith(".json");
        }
    }
}