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
    [SerializeField] private InventorySlotData[] _inventorySlotData;

    [Tooltip("This is used in generating the items list. The number of additional copies to concat the list parsed from ItemJson.")]
    [SerializeField] private int _itemGenerateScale = 10;

    [Tooltip("Icons referenced by ItemData.IconIndex when instantiating new items.")]
    [SerializeField] private Sprite[] _icons;

    private InventoryItemData[] _itemDatas;
    private ScrollPool<InventoryItem> _scrollPool;
    private InventorySlot _selectedSlot;
    private InventoryItem _selectedItem;
    private int _selectedIndex;

    [Serializable]
    private class InventoryItemDatas
    {
        public InventoryItemData[] ItemDatas;
    }

    [Serializable]
    private class InventorySlotData
    {
        public InventorySlot Slot;
        public TextAsset Json;
    }

    IEnumerator Start()
    {
        // Wait for Layout Elements first calculation
        yield return null;

        // Clear existing items already in the list.
        foreach (InventoryItem item in _container.GetComponentsInChildren<InventoryItem>())
            Destroy(item.gameObject);

        foreach (var slotData in _inventorySlotData)
        {
            var slot = slotData.Slot;
            slot.Button.onClick.AddListener(() => { InventorySlotOnClick(slot, slotData.Json); });
        }

        _scrollPool = new ScrollPool<InventoryItem>();
        _scrollPool.Initialize(_scrollRect, _inventoryItemPrefab, InitializeItem);

        InventorySlotOnClick(_inventorySlotData[0].Slot, _inventorySlotData[0].Json);
    }

    /// <summary>
    /// Sets item data, state and listeners.
    /// </summary>
    private void InitializeItem(InventoryItem item, int index)
    {
        var itemData = _itemDatas[index];
        item.Icon.sprite = _icons[itemData.IconIndex];
        item.Name.text = itemData.Name;
        item.Button.onClick.AddListener(() => { InventoryItemOnClick(item, index); });
        item.Highlight(index == _selectedSlot.SelectedIndex);
    }

    /// <summary>
    /// Shows item list from json.
    /// </summary>
    /// <param name="dataJson">JSON to generate items from. JSON must be an array of InventoryItemData.</param>
    private void ShowList(TextAsset dataJson)
    {
        _itemDatas = GenerateItemDatas(dataJson.text, _itemGenerateScale);

        _scrollPool.ClearDisplay();
        _scrollPool.SetItemCount(_itemDatas.Length);
        _scrollPool.PlaceItems();
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

    private void InventorySlotOnClick(InventorySlot slot, TextAsset data)
    {
        if (_selectedSlot == slot)
            return;

        _selectedSlot = slot;
        ShowList(data);

        // Select the first item if none is selected
        if (slot.SelectedIndex < 0)
            InventoryItemOnClick(_scrollPool.GetTopItem(), 0);
    }

    private void InventoryItemOnClick(InventoryItem itemClicked, int index)
    {
        if (_selectedItem != null)
            _selectedItem.Highlight(false);

        _selectedItem = itemClicked;
        itemClicked.Highlight(true);

        _infoPanel.SetData(_itemDatas[index]);
        _selectedSlot.SelectedIndex = index;
        _selectedSlot.Icon.sprite = _icons[_itemDatas[index].IconIndex];
    }
}
