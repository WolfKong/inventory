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
    [SerializeField] private InventoryCategory _startingCategory;
    [SerializeField] private InventoryCategory[] _inventoryCategories;

    [Tooltip("This is used in generating the items list. The number of additional copies to concat the list parsed from ItemJson.")]
    [SerializeField] private int _itemGenerateScale = 10;

    [Tooltip("Icons referenced by ItemData.IconIndex when instantiating new items.")]
    [SerializeField] private Sprite[] _icons;

    private Dictionary<InventoryCategory, TabButton> _tabsByCategory;
    private ScrollPool<InventoryItem> _scrollPool;
    private InventoryCategory _selectedCategory;
    private InventoryItem _selectedItem;

    IEnumerator Start()
    {
        // Wait for Layout Elements first calculation
        yield return null;

        // Clear existing items already in the list.
        foreach (InventoryItem item in _scrollRect.content.GetComponentsInChildren<InventoryItem>())
            Destroy(item.gameObject);

        // Clear existing tabs already in the list.
        foreach (TabButton tab in _tabsParent.GetComponentsInChildren<TabButton>())
            Destroy(tab.gameObject);

        _characterPanel.ClickedSlot += SelectCategory;
        _characterPanel.RanOptimization += UpdateAllDisplays;

        _tabsByCategory = new Dictionary<InventoryCategory, TabButton>();

        foreach (var category in _inventoryCategories)
        {
            var tab = Instantiate<TabButton>(_tabPrefab, _tabsParent);
            tab.Label.text = category.Name;
            tab.Button.onClick.AddListener(() => { SelectCategory(category); });

            _tabsByCategory[category] = tab;
            category.GenerateItemDatas(_itemGenerateScale);
        }

        _scrollPool = new ScrollPool<InventoryItem>();
        _scrollPool.Initialize(_scrollRect, _inventoryItemPrefab, InitializeItem);

        SelectCategory(_startingCategory);
    }

    private void OnDestroy()
    {
        _characterPanel.ClickedSlot -= SelectCategory;
        _characterPanel.RanOptimization -= UpdateAllDisplays;
    }

    /// <summary>
    /// Sets item data, state and listeners.
    /// </summary>
    private void InitializeItem(InventoryItem item, int index)
    {
        var itemData = _selectedCategory.ItemsData[index];
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
    /// Updates list, characterPanel and infoPanel
    /// </summary>
    private void UpdateAllDisplays()
    {
        foreach (var category in _inventoryCategories)
            _characterPanel.UpdateCategory(category, category.SelectedData, _icons);

        UpdateList();

        _infoPanel.SetData(_selectedCategory.SelectedData);
    }

    /// <summary>
    /// Highlights tabs and updates list of items for selected category.
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

        UpdateList();

        // Select the first item if none is selected
        if (category.SelectedIndex < 0)
            SelectItem(_scrollPool.GetTopItem(), 0);
        else
            _infoPanel.SetData(category.SelectedData);
    }

    /// <summary>
    /// Display list of items for selected category.
    /// </summary>
    private void UpdateList()
    {
        _scrollPool.ClearDisplay();
        _scrollPool.SetItemCount(_selectedCategory.ItemsData.Length);
        _scrollPool.PlaceItems(_selectedCategory.SelectedIndex);
    }

    /// <summary>
    /// Updates panels with information for selected item.
    /// </summary>
    /// <param name="item">Selected item.</param>
    /// <param name="index">Selected item index.</param>
    private void SelectItem(InventoryItem item, int index)
    {
        if (_selectedItem != null)
            _selectedItem.Highlight(false);

        _selectedItem = item;
        item.Highlight(true);

        var itemData = _selectedCategory.ItemsData[index];
        _infoPanel.SetData(itemData);
        _selectedCategory.SelectedIndex = index;
        _characterPanel.UpdateCategory(_selectedCategory, itemData, _icons);
    }
}
