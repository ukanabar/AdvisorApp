using System.Collections.Generic;

public class MRUCache<K, V>
{
    private readonly int _capacity;
    private readonly Dictionary<K, V> _cache;
    private readonly LinkedList<K> _recencyList;

    public MRUCache(int capacity = 5)
    {
        _capacity = capacity;
        _cache = new Dictionary<K, V>(capacity);
        _recencyList = new LinkedList<K>();
    }

    public V Get(K key)
    {
        if (_cache.TryGetValue(key, out V value))
        {
            _recencyList.Remove(key);
            _recencyList.AddFirst(key);
            return value;
        }
        return default(V);
    }

    public void Put(K key, V value)
    {
        if (_cache.ContainsKey(key))
        {
            _cache[key] = value;
            _recencyList.Remove(key);
        }
        else
        {
            if (_cache.Count >= _capacity)
            {
                K leastRecentKey = _recencyList.Last.Value;
                _recencyList.RemoveLast();
                _cache.Remove(leastRecentKey);
            }
            _cache[key] = value;
        }
        _recencyList.AddFirst(key);
    }

    public void Delete(K key)
    {
        if (_cache.Remove(key))
        {
            _recencyList.Remove(key);
        }
    }
}
