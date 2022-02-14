using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private InventoryInfoPanel _infoPanel;
    [SerializeField] private InventoryCharacterPanel _characterPanel;
    [SerializeField] private InventoryItem _inventoryItemPrefab;
    [SerializeField] private TabButton _tabPrefab;
    [SerializeField] private Transform _tabsParent;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private GameObject _container;
    [SerializeField] private InventoryCategory _startingCategory;
    [SerializeField] private InventoryCategory[] _inventoryCategories;

    [Tooltip("This is used in generating the items list. The number of additional copies to concat the list parsed from ItemJson.")]
    [SerializeField] private int _itemGenerateScale = 10;

    [Tooltip("Icons referenced by ItemData.IconIndex when instantiating new items.")]
    [SerializeField] private Sprite[] _icons;

    private Dictionary<InventoryCategory, InventoryItemData[]> _itemsDataByCategory;
    private Dictionary<InventoryCategory, TabButton> _tabsByCategory;
    private ScrollPool<InventoryItem> _scrollPool;
    private InventoryCategory _selectedCategory;
    private InventoryItem _selectedItem;

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

        // Clear existing tabs already in the list.
        foreach (TabButton tab in _tabsParent.GetComponentsInChildren<TabButton>())
            Destroy(tab.gameObject);

        _characterPanel.ClickedSlot += SelectCategory;

        _itemsDataByCategory = new Dictionary<InventoryCategory, InventoryItemData[]>();
        _tabsByCategory = new Dictionary<InventoryCategory, TabButton>();

        foreach (var category in _inventoryCategories)
        {
            var tab = Instantiate<TabButton>(_tabPrefab, _tabsParent);
            tab.Label.text = category.Name;
            tab.Button.onClick.AddListener(() => { SelectCategory(category); });

            _tabsByCategory[category] = tab;
            _itemsDataByCategory[category] = GenerateItemDatas(category.ItemsDataJson.text, _itemGenerateScale);
        }

        _scrollPool = new ScrollPool<InventoryItem>();
        _scrollPool.Initialize(_scrollRect, _inventoryItemPrefab, InitializeItem);

        SelectCategory(_startingCategory);
    }

    private void OnDestroy()
    {
        _characterPanel.ClickedSlot -= SelectCategory;
    }

    /// <summary>
    /// Sets item data, state and listeners.
    /// </summary>
    private void InitializeItem(InventoryItem item, int index)
    {
        var itemData = _itemsDataByCategory[_selectedCategory][index];
        item.Icon.sprite = _icons[itemData.IconIndex];
        item.Name.text = itemData.Name;
        item.Button.onClick.RemoveAllListeners();
        item.Button.onClick.AddListener(() => { SelectItem(item, index); });

        var isSelected = index == _selectedCategory.SelectedIndex;
        item.Highlight(isSelected);

        if (isSelected)
            _selectedItem = item;
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

    /// <summary>
    /// Shows list of items for selected category.
    /// </summary>
    /// <param name="category">Selected category.</param>
    private void SelectCategory(InventoryCategory category)
    {
        if (_selectedCategory == category)
            return;

        if (_selectedItem != null)
            _selectedItem.Highlight(false);

        if (_selectedCategory != null)
            _tabsByCategory[_selectedCategory].Highlight(false);

        _selectedCategory = category;
        _tabsByCategory[category].Highlight(true);

        _scrollPool.ClearDisplay();
        _scrollPool.SetItemCount(_itemsDataByCategory[category].Length);
        _scrollPool.PlaceItems(category.SelectedIndex);

        // Select the first item if none is selected
        if (category.SelectedIndex < 0)
            SelectItem(_scrollPool.GetTopItem(), 0);
        else
            _infoPanel.SetData(_itemsDataByCategory[category][category.SelectedIndex]);
    }

    private void SelectItem(InventoryItem item, int index)
    {
        if (_selectedItem != null)
            _selectedItem.Highlight(false);

        _selectedItem = item;
        item.Highlight(true);

        var itemData = _itemsDataByCategory[_selectedCategory][index];
        _infoPanel.SetData(itemData);
        _selectedCategory.SelectedIndex = index;
        _selectedCategory.SelectedData = itemData;
        _characterPanel.UpdateCategory(_selectedCategory, itemData, _icons);
    }
}
