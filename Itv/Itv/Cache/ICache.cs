namespace Itv.Cache;

/// <summary>
/// Contrato generico para la cache.
/// </summary>
/// <typeparam name="TKey">Clave proporcionada.</typeparam>
/// <typeparam name="TValue">Valor proporcionado.</typeparam>
public interface ICache<in TKey, TValue> where TKey : notnull {
    
    /// <summary>
    /// Añade un nuevo elemento a la cache.
    /// </summary>
    /// <param name="key">Clave del elemento.</param>
    /// <param name="value">Elemento.</param>
    void Add(TKey key, TValue value);
    
    /// <summary>
    /// Obtiene un elemento de la cache.
    /// </summary>
    /// <param name="key">Clave del elemento.</param>
    /// <returns>Elemento si lo encuentra o nulo si no.</returns>
    TValue? Get(TKey key);
    
    /// <summary>
    /// Elimina un elemeno de la cache.
    /// </summary>
    /// <param name="key">Clave del elemento.</param>
    /// <returns>Elemento eliminado.</returns>
    bool Remove(TKey key);
    
    /// <summary>
    /// Muestra el status actual de la cache.
    /// </summary>
    void DisplayStatus();
}