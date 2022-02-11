using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private InventoryInfoPanel _infoPanel;
    [SerializeField] private InventoryItem _inventoryItemPrefab;
    [SerializeField] private ScrollPool _scrollPool;
    [SerializeField] private GameObject _container;

    [Tooltip(tooltip:"Loads the list using this format.")]
    [Multiline]
    public string ItemJson;

    [Tooltip(tooltip:"This is used in generating the items list. The number of additional copies to concat the list parsed from ItemJson.")]
    public int ItemGenerateScale = 10;

    [Tooltip(tooltip:"Icons referenced by ItemData.IconIndex when instantiating new items.")]
    public Sprite[] Icons;

    private InventoryItem _selectedItem;

    private InventoryItemData[] _itemDatas;

    private List<InventoryItem> _items;

    [Serializable]
    private class InventoryItemDatas
    {
        public InventoryItemData[] ItemDatas;
    }

    IEnumerator Start()
    {
        // Wait for Layout Elements first calculation
        yield return null;

        // Clear existing items already in the list.
        foreach (InventoryItem item in _container.GetComponentsInChildren<InventoryItem>())
            Destroy(item.gameObject);

        _itemDatas = GenerateItemDatas(ItemJson, ItemGenerateScale);

        // Instantiate items in the Scroll View.
        _items = new List<InventoryItem>();

        void InitializeItem(GameObject go, int index)
        {
            var item = go.GetComponent<InventoryItem>();
            var itemData = _itemDatas[index];
            item.Icon.sprite = Icons[itemData.IconIndex];
            item.Name.text = itemData.Name;
            item.Button.onClick.AddListener(() => { InventoryItemOnClick(item, itemData); });
            item.Background.color = Color.white;
            _items.Add(item);
        }

        _scrollPool.Initialize<InventoryItem>(_inventoryItemPrefab, InitializeItem, _itemDatas.Length);

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
        if (_selectedItem != null)
            _selectedItem.Background.color = Color.white;

        _selectedItem = itemClicked;
        itemClicked.Background.color = Color.red;

        _infoPanel.SetData(itemData);
    }
}
