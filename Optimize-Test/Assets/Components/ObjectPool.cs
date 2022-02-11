using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private GameObject _prefab;
    private Transform _parent;
    private List<GameObject> _availableItems;

    /// <summary>
    /// Creates pool items.
    /// </summary>
    /// <param name="prefab">Prefab to generate items.</param>
    /// <param name="parent">Items parent.</param>
    /// <param name="size">Pool size.</param>
    public void PopulatePool(GameObject prefab, Transform parent, int size)
    {
        _prefab = prefab;
        _parent = parent;
        _availableItems = new List<GameObject>();

        for (var i = 0; i < size; i++)
        {
            var item = CreateItem();
            item.SetActive(false);
            _availableItems.Add(item);
        }
    }

    private GameObject CreateItem()
    {
        return GameObject.Instantiate(_prefab, _parent);
    }

    /// <summary>
    /// Gets an object from the pool. If it's empty creates a new oject and returns it.
    /// </summary>
    /// <returns>Object from the pool</returns>
    public GameObject GetObject()
    {
        if (_availableItems.Count == 0)
        {
            Debug.LogWarning($"Pool ran out of objects! Creating a new one.");
            return CreateItem();
        }

        var item = _availableItems[0];
        item.SetActive(true);
        _availableItems.RemoveAt(0);
        return item;
    }

    /// <summary>
    /// Returns an object to the pool.
    /// </summary>
    /// <param name="gameObject">Object being returned.</param>
    public void ReturnObject(GameObject gameObject)
    {
        _availableItems.Add(gameObject);
        gameObject.SetActive(false);
    }
}
