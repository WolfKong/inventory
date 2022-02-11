using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    public List<GameObject> Items;

    private static Vector3 vector3Zero = Vector3.zero;

    public void PopulatePool(GameObject prefab, int size)
    {
        Items = new List<GameObject>();

        for (var i = 0; i < size; i++)
        {
            var item = GameObject.Instantiate(prefab);
            item.gameObject.SetActive(false);
            Items.Add(item);
        }
    }

    public GameObject GetObject()
    {
        for (var i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            if (!item.activeInHierarchy)
            {
                item.SetActive(true);
                return item;
            }
        }
        return null;
    }

    public void ReturnObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
        gameObject.transform.SetParent(null);
        gameObject.transform.localPosition = vector3Zero;
    }
}
