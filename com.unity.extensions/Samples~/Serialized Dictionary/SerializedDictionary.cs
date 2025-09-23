using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class SerializedDictionary<TKey, TValue>
    : ISerializationCallbackReceiver,
        ICollection<KeyValuePair<TKey, TValue>>,
        IEnumerable<KeyValuePair<TKey, TValue>>,
        IEnumerable,
        IDictionary<TKey, TValue>,
        IReadOnlyCollection<KeyValuePair<TKey, TValue>>,
        IReadOnlyDictionary<TKey, TValue>,
        ICollection,
        IDictionary,
        IDeserializationCallback,
        ISerializable
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

    public Dictionary<TKey, TValue> Dictionary => dictionary;

    public SerializedDictionary()
    {
        dictionary = new Dictionary<TKey, TValue>();
    }

    public SerializedDictionary(Dictionary<TKey, TValue> initialValues)
    {
        dictionary = new Dictionary<TKey, TValue>(initialValues);
    }

    public SerializedDictionary(int capacity)
    {
        dictionary = new Dictionary<TKey, TValue>(capacity);
    }

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (var pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();

        for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
        {
            this[keys[i]] = values[i];
        }
    }

    public void Add(TKey key, TValue value) => dictionary.Add(key, value);

    public bool Remove(TKey key) => dictionary.Remove(key);

    public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);

    public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);

    public void Clear() => dictionary.Clear();

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        dictionary.Add(item.Key, item.Value);
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return dictionary.TryGetValue(item.Key, out var value)
            && EqualityComparer<TValue>.Default.Equals(value, item.Value);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (Contains(item))
        {
            return dictionary.Remove(item.Key);
        }
        return false;
    }

    public void CopyTo(Array array, int index)
    {
        ((ICollection)dictionary).CopyTo(array, index);
    }

    public void Add(object key, object value)
    {
        if (key is TKey tKey && value is TValue tValue)
        {
            dictionary.Add(tKey, tValue);
        }
        else
        {
            throw new ArgumentException("Invalid key or value type.");
        }
    }

    public bool Contains(object key)
    {
        return key is TKey tKey && dictionary.ContainsKey(tKey);
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
        return ((IDictionary)dictionary).GetEnumerator();
    }

    public void Remove(object key)
    {
        if (key is TKey tKey)
        {
            dictionary.Remove(tKey);
        }
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ((ISerializable)dictionary).GetObjectData(info, context);
    }

    public void OnDeserialization(object sender)
    {
        ((IDeserializationCallback)dictionary).OnDeserialization(sender);
    }

    public int Count => dictionary.Count;

    public IEnumerable<TKey> Keys => dictionary.Keys;

    public IEnumerable<TValue> Values => dictionary.Values;

    public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).IsReadOnly;

    public bool IsSynchronized => ((ICollection)dictionary).IsSynchronized;

    public object SyncRoot => ((ICollection)dictionary).SyncRoot;

    public bool IsFixedSize => ((IDictionary)dictionary).IsFixedSize;

    ICollection IDictionary.Keys => ((IDictionary)dictionary).Keys;

    ICollection IDictionary.Values => ((IDictionary)dictionary).Values;

    ICollection<TKey> IDictionary<TKey, TValue>.Keys => dictionary.Keys;

    ICollection<TValue> IDictionary<TKey, TValue>.Values => dictionary.Values;

    public object this[object key]
    {
        get =>
            key is TKey tKey ? dictionary[tKey] : throw new ArgumentException("Invalid key type.");
        set
        {
            if (key is TKey tKey && value is TValue tValue)
            {
                dictionary[tKey] = tValue;
            }
            else
            {
                throw new ArgumentException("Invalid key or value type.");
            }
        }
    }

    public TValue this[TKey key]
    {
        get => dictionary[key];
        set => dictionary[key] = value;
    }
}
