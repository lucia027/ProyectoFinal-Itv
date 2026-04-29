using Itv.Models;
using Itv.Storage.Common;

namespace Itv.Storage.Csv;

/// <summary>
/// Contrato que conextualiza la interfaz de IStorage a Cita.
/// </summary>
public interface ICitaCsvStorage : IStorage<Cita> { }