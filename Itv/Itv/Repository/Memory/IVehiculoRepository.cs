using Itv.Models;
using Itv.Repository.Common;

namespace Itv.Repository.Memory;

public interface IVehiculoRepository : ICrudRepository<int, Vehiculo> {
    /// <summary>
    /// Elimina permanentemente un vehiculo del almacen.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Vehiculo? DeleteHard(int id);

    /// <summary>
    /// Elimina todos los vehiculos del alamcen;
    /// </summary>
    /// <returns>Verdadero al eliminarlos.</returns>
    bool DeleteAll();
}