using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private InventoryInfoPanel _infoPanel;
    [SerializeField] private InventoryItem _inventoryItemPrefab;
    [SerializeField] private TabButton _tabPrefab;
    [SerializeField] private Transform _tabsParent;
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
    private TabButton _selectedTab;

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

        // Clear existing tabs already in the list.
        foreach (TabButton tab in _tabsParent.GetComponentsInChildren<TabButton>())
            Destroy(tab.gameObject);

        foreach (var slotData in _inventorySlotData)
        {
            var slot = slotData.Slot;
            slot.Button.onClick.AddListener(() => { InventorySlotOnClick(slot, slotData.Json); });

            var tab = Instantiate<TabButton>(_tabPrefab, _tabsParent);
            tab.Label.text = slotData.Json.name;
            tab.Button.onClick.AddListener(() => { InventorySlotOnClick(slot, slotData.Json); });
            slot.Tab = tab;
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
        item.Button.onClick.RemoveAllListeners();
        item.Button.onClick.AddListener(() => { InventoryItemOnClick(item, index); });

        var isSelected = index == _selectedSlot.SelectedIndex;
        item.Highlight(index == _selectedSlot.SelectedIndex);

        if (isSelected)
            _selectedSlot.SelectedItem = item;
    }

    /// <summary>
    /// Shows item list from json.
    /// </summary>
    /// <param name="dataJson">JSON to generate items from. JSON must be an array of InventoryItemData.</param>
    /// <param name="index">Index of selected cell.</param>
    private void ShowList(TextAsset dataJson, int index)
    {
        _itemDatas = GenerateItemDatas(dataJson.text, _itemGenerateScale);

        _scrollPool.ClearDisplay();
        _scrollPool.SetItemCount(_itemDatas.Length);
        _scrollPool.PlaceItems(index);
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

        if (_selectedSlot != null)
            _selectedSlot.Tab.Highlight(false);

        slot.Tab.Highlight(true);

        _selectedSlot = slot;
        ShowList(data, slot.SelectedIndex);

        // Select the first item if none is selected
        if (slot.SelectedIndex < 0)
            InventoryItemOnClick(_scrollPool.GetTopItem(), 0);
        else
            _infoPanel.SetData(_itemDatas[slot.SelectedIndex]);
    }

    private void InventoryItemOnClick(InventoryItem itemClicked, int index)
    {
        if (_selectedSlot.SelectedItem != null)
            _selectedSlot.SelectedItem.Highlight(false);

        _selectedSlot.SelectedItem = itemClicked;
        itemClicked.Highlight(true);

        _infoPanel.SetData(_itemDatas[index]);
        _selectedSlot.SelectedIndex = index;
        _selectedSlot.Icon.sprite = _icons[_itemDatas[index].IconIndex];
    }
}
