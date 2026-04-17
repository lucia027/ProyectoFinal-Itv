using Itv.Storage.Json;

namespace Itv.Test.Storage.Json;

[TestFixture]
public class VehiculoJsonStorageTests {

    [TestFixture]
    public sealed class CasosPositivos() {
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
        public void Cargar_DatosValidos_CargarDatos() {
        }

    }

}