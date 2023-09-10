using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Unreal.Core;

public class KeyList<K, V>
{
    private readonly List<V> _vals = new();
    private readonly Dictionary<K, int> _keys = new();
    public int Length => _vals.Count;

    public int Count(Func<V, bool> func) => _vals.Count(func);

    public void Add(K key, V val)
    {
        if (_keys.TryAdd(key, _vals.Count))
        {
            _vals.Add(val);
        }
    }

    public bool TryGetIndex(K key, [NotNullWhen(returnValue: true)] out int index) => _keys.TryGetValue(key, out index);

    public bool TryGetValue(int keyId, [NotNullWhen(returnValue: true)] out V? val)
    {
        val = default;

        if (keyId >= 0 && keyId < _vals.Count)
        {
            val = _vals[keyId];
            return true;
        }

        return false;
    }

    public bool TryGetValue(K key, [NotNullWhen(returnValue: true)] out V? val)
    {
        if (_keys.TryGetValue(key, out var id))
        {
            val = _vals[id];
            return true;
        }

        val = default;

        return false;
    }

    public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<K, TKey> key, Func<V, TElement> val)
    {
        var vals = new Dictionary<TKey, TElement>();

        foreach (var kvp in _keys)
        {
            var itemVal = _vals[kvp.Value];

            vals.Add(key(kvp.Key), val(itemVal));
        }

        return vals;
    }

    public V this[int index] => _vals[index];
}
