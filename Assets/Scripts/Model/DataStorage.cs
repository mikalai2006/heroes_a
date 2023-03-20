using System.Collections.Generic;
using UnityEngine;

// More at: https://www.patrykgalach.com/2019/04/04/singleton-in-unity-love-or-hate/

/// <summary>
/// Data storage that uses one of the singleton implementations.
/// Object is used to store and get data through game life.
/// </summary>
public class DataStorage : Singleton<DataStorage>
{
    // References to all stored data
    private Dictionary<string, object> storage = new Dictionary<string, object>();

    /// <summary>
    /// Method used to save data in storage.
    /// </summary>
    /// <param name="key">Key.</param>
    /// <param name="data">Data.</param>
    public void SaveData(string key, object data)
    {
        if (storage.ContainsKey(key)) // If something under key exist already we are printing warning.
        {
            Debug.LogWarningFormat("[{0}] Overriding value in: {1}.", typeof(DataStorage), key);
        }

        storage[key] = data;
    }

    /// <summary>
    /// Method used to verify if storage has data under provided key.
    /// </summary>
    /// <returns><c>true</c>, if storage contains data, <c>false</c> otherwise.</returns>
    /// <param name="key">Key.</param>
    /// <typeparam name="T">Expected data type.</typeparam>
    public bool HasData<T>(string key)
    {
        if (!storage.ContainsKey(key)) // If storage doesn't has key then return false.
        {
            return false;
        }

        return ((T)storage[key]) != null; // If storage has data but we need to verify type.
    }

    /// <summary>
    /// Method used to get data from storage.
    /// </summary>
    /// <returns>Data.</returns>
    /// <param name="key">Key.</param>
    /// <typeparam name="T">Expected data type.</typeparam>
    public T GetData<T>(string key)
    {
        if (!storage.ContainsKey(key)) // Check is storage has data under provided key.
        {
            Debug.LogWarningFormat("[{0}] No value under key: {1}. Returning default", typeof(DataStorage), key);
            return default(T); // Return default value for type.
        }

        return (T)storage[key];
    }

    /// <summary>
    /// Method used to remove data from storage.
    /// </summary>
    /// <param name="key">Key.</param>
    public void RemoveData(string key)
    {
        if (storage.ContainsKey(key)) // If data under provided key exist, we are removing it.
        {
            storage.Remove(key);
        }
    }
}