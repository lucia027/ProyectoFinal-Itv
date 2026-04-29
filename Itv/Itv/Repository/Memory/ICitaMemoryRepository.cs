using Itv.Models;
using Itv.Repository.Common;

namespace Itv.Repository.Memory;

/// <summary>
/// Contrato que contextualiza la interfaz de ICitaRepository para int y Cita.
/// </summary>
public interface ICitaMemoryRepository : ICitaRepository<int, Cita> { }