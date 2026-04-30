using Serilog;

namespace Itv.Cache;

/// <summary>
/// Caché Lru con capacidad fija.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class CacheLru<TKey, TValue> : ICache<TKey, TValue> where TKey: notnull {

    private readonly ILogger _logger = Log.ForContext<CacheLru<TKey, TValue>>();
    
    private readonly int _capacidad;
    private readonly Dictionary<TKey, TValue> _datos = new();
    private readonly LinkedList<TKey> _ordenUso = new();

    public CacheLru(int capacidad) {
        if (capacidad <= 0) {
            _logger.Error("La capacidad de la cache no puede ser menor o igual a cero, capacidad {Capacidad}",
                capacidad);
            throw new ArgumentException("La capacidad de la cache no puede ser menor o igual a 0.");
        }
        _capacidad = capacidad;
    }
    
    /// <inheritdoc cref="ICache{TKey,TValue}.Add" />
    public void Add(TKey key, TValue value) {
        _logger.Debug("Se esta intentando añadir un nuevo elemento en la cache.");
        
        if (_datos.TryGetValue(key, out var valor)) {
            _logger.Debug("La clave proporcionada ya existe, se actualizara el valor para la clave: {Key}",
                key);
            
            _datos[key] = value;
            RefreshUsage(key);
            return;
        }
        
        if (_datos.Count >= _capacidad) {
            var claveVieja = _ordenUso.First!.Value;
            var valorViejo = _datos[claveVieja];
            
            _logger.Debug("La cache esta llena, se esta eliminando el elemento mas viejo con clave: {ClaveVieja} y valor: {ValorViejo}",
                claveVieja, valorViejo);
            
            _ordenUso.RemoveFirst();
            _datos.Remove(claveVieja);
        }

        _datos.Add(key, value);
        _ordenUso.AddLast(key);
        _logger.Debug("Se ha añadido un elemento nuevo, lista de orden de uso: {_OrdenUso}",
            _ordenUso);
    }

    /// <inheritdoc cref="ICache{TKey,TValue}.Get" />
    public TValue? Get(TKey key) {
        _logger.Debug("Se esta intentando acceder un elemento en la cache.");

        if (!_datos.TryGetValue(key, out var value)) {
            _logger.Debug("No se ha encontrado la clave: {Key} en la cache.",
                key);
            return default;
        }

        _logger.Debug("Se ha enconctrado la clave: {Key} en la cache.",
            key);
        RefreshUsage(key);

        return value;
    }

    /// <inheritdoc cref="ICache{TKey,TValue}.Remove" />
    public bool Remove(TKey key) {
        _logger.Debug("Se esta intentando eliminar un elemento en la cache.");

        if (!_datos.Remove(key)) {
            _logger.Debug("No se ha encontrado la clave: {Key} en la cache.",
                key);
            return false;
        }

        _ordenUso.Remove(key);
        _logger.Debug("Se ha eliminado la clave: {Key} correctamente de la cache.",
            key);
        return true;    
    }

    /// <inheritdoc cref="ICache{TKey,TValue}.DisplayStatus" />
    public void DisplayStatus() {
        _logger.Information("Capacidad disponible en la cache: {_Datos.Count - _Capacidad}",
            _datos.Count - _capacidad);
        _logger.Information("Uso de la cache del menos reciente al mas reciente: {_OrdenUso}",
            _ordenUso);    
    }
    
    /// <summary>
    /// Desplaza la clave existente a la ultima posicion de la lista de orden de uso.
    /// </summary>
    /// <param name="key">Clave del elemento.</param>
    private void RefreshUsage(TKey key) {
        _logger.Verbose("Se esta moviendo la entidad con la clave: {Key} al final de la lista.",
            key);
        _ordenUso.Remove(key);
        _ordenUso.AddLast(key);
    }
}