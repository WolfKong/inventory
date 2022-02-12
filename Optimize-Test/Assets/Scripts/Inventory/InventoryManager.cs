using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private InventoryInfoPanel _infoPanel;
    [SerializeField] private InventoryItem _inventoryItemPrefab;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private GameObject _container;

    [Tooltip(tooltip:"Loads the list using this format.")]
    [Multiline]
    public string ItemJson;

    [Tooltip(tooltip:"This is used in generating the items list. The number of additional copies to concat the list parsed from ItemJson.")]
    public int ItemGenerateScale = 10;

    [Tooltip(tooltip:"Icons referenced by ItemData.IconIndex when instantiating new items.")]
    public Sprite[] Icons;

    private InventoryItem _selectedItem;
    private float _selectedIndex;

    private InventoryItemData[] _itemDatas;

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
        void InitializeItem(InventoryItem item, int index)
        {
            var itemData = _itemDatas[index];
            item.Icon.sprite = Icons[itemData.IconIndex];
            item.Name.text = itemData.Name;
            item.Button.onClick.AddListener(() => { InventoryItemOnClick(item, index); });
            item.Highlight(index == _selectedIndex);
        }

        var scrollPool = new ScrollPool<InventoryItem>();
        scrollPool.Initialize(_scrollRect, _inventoryItemPrefab, InitializeItem, _itemDatas.Length);

        // Select the first item.
        InventoryItemOnClick(scrollPool.GetTopItem(), 0);
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

    private void InventoryItemOnClick(InventoryItem itemClicked, int index)
    {
        if (_selectedItem != null)
            _selectedItem.Highlight(false);

        _selectedIndex = index;
        _selectedItem = itemClicked;
        itemClicked.Highlight(true);

        _infoPanel.SetData(_itemDatas[index]);
    }
}
