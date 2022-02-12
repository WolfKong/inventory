using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private T _prefab;
    private Transform _parent;
    private List<T> _availableItems;

    /// <summary>
    /// Creates pool items.
    /// </summary>
    /// <param name="prefab">Prefab to generate items.</param>
    /// <param name="parent">Items parent.</param>
    /// <param name="size">Pool size.</param>
    public void PopulatePool(T prefab, Transform parent, int size)
    {
        _prefab = prefab;
        _parent = parent;
        _availableItems = new List<T>();

        for (var i = 0; i < size; i++)
        {
            var item = CreateItem();
            item.gameObject.SetActive(false);
            _availableItems.Add(item);
        }
    }

    private T CreateItem()
    {
        return GameObject.Instantiate<T>(_prefab, _parent);
    }

    /// <summary>
    /// Gets an object from the pool. If it's empty creates a new object and returns it.
    /// </summary>
    /// <returns>Object from the pool</returns>
    public T GetObject()
    {
        if (_availableItems.Count == 0)
        {
            Debug.LogWarning($"Pool ran out of objects! Creating a new one.");
            return CreateItem();
        }

        var item = _availableItems[0];
        item.gameObject.SetActive(true);
        _availableItems.RemoveAt(0);
        return item;
    }

    /// <summary>
    /// Returns an object to the pool.
    /// </summary>
    /// <param name="component">Object being returned.</param>
    public void ReturnObject(T component)
    {
        _availableItems.Add(component);
        component.gameObject.SetActive(false);
    }
}
