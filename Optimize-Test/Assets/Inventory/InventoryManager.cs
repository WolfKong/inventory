using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private InventoryInfoPanel _infoPanel;
    [SerializeField] private InventoryItem _inventoryItemPrefab;
    [SerializeField] private GameObject _container;

    [Tooltip(tooltip:"Loads the list using this format.")]
    [Multiline]
    public string ItemJson;

    [Tooltip(tooltip:"This is used in generating the items list. The number of additional copies to concat the list parsed from ItemJson.")]
    public int ItemGenerateScale = 10;

    [Tooltip(tooltip:"Icons referenced by ItemData.IconIndex when instantiating new items.")]
    public Sprite[] Icons;

    [Serializable]
    private class InventoryItemDatas
    {
        public InventoryItemData[] ItemDatas;
    }

    private InventoryItemData[] _itemDatas;

    private List<InventoryItem> _items;

    void Start()
    {
        // Clear existing items already in the list.
        var items = _container.GetComponentsInChildren<InventoryItem>();
        foreach (InventoryItem item in items)
        {
            item.gameObject.transform.SetParent(null);
        }

        _itemDatas = GenerateItemDatas(ItemJson, ItemGenerateScale);

        // Instantiate items in the Scroll View.
        _items = new List<InventoryItem>();
        foreach (InventoryItemData itemData in _itemDatas)
        {
            var newItem = GameObject.Instantiate<InventoryItem>(_inventoryItemPrefab);
            newItem.Icon.sprite = Icons[itemData.IconIndex];
            newItem.Name.text = itemData.Name;
            newItem.transform.SetParent(_container.transform);
            newItem.Button.onClick.AddListener(() => { InventoryItemOnClick(newItem, itemData); });
            _items.Add(newItem);
        }

        // Select the first item.
        InventoryItemOnClick(_items[0], _itemDatas[0]);
    }

    /// <summary>
    /// Generates an item list.
    /// </summary>
    /// <param name="json">JSON to generate items from. JSON must be an array of InventoryItemData.</param>
    /// <param name="scale">Concats additional copies of the array parsed from json.</param>
    /// <returns>An array of InventoryItemData</returns>
    private InventoryItemData[] GenerateItemDatas(string json, int scale)
    {
        var itemDatas = JsonUtility.FromJson<InventoryItemDatas>(json).ItemDatas;
        var finalItemDatas = new InventoryItemData[itemDatas.Length * scale];
        for (var i = 0; i < itemDatas.Length; i++)
        {
            for (var j = 0; j < scale; j++)
            {
                finalItemDatas[i + j*itemDatas.Length] = itemDatas[i];
            }
        }

        return finalItemDatas;
    }

    private void InventoryItemOnClick(InventoryItem itemClicked, InventoryItemData itemData)
    {
        foreach (var item in _items)
        {
            item.Background.color = Color.white;
        }
        itemClicked.Background.color = Color.red;

        _infoPanel.SetData(itemData);
    }
}
