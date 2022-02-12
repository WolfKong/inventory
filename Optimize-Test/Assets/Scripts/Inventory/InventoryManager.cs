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
    [SerializeField] private InventorySlot _rightSlot;
    [SerializeField] private InventorySlot _leftSlot;

    [Tooltip("Loads the list using this format.")]
    [SerializeField] private TextAsset _allDataJson;
    [SerializeField] private TextAsset _leftHandDataJson;
    [SerializeField] private TextAsset _rightHandDataJson;

    [Tooltip("This is used in generating the items list. The number of additional copies to concat the list parsed from ItemJson.")]
    [SerializeField] private int _itemGenerateScale = 10;

    [Tooltip("Icons referenced by ItemData.IconIndex when instantiating new items.")]
    [SerializeField] private Sprite[] _icons;

    private InventoryItemData[] _itemDatas;
    private ScrollPool<InventoryItem> _scrollPool;
    private InventoryItem _selectedItem;
    private float _selectedIndex;

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

        _rightSlot.Button.onClick.AddListener(() => { ShowList(_rightHandDataJson); });
        _leftSlot.Button.onClick.AddListener(() => { ShowList(_leftHandDataJson); });

        _itemDatas = GenerateItemDatas(_allDataJson.text, _itemGenerateScale);

        _scrollPool = new ScrollPool<InventoryItem>();
        _scrollPool.Initialize(_scrollRect, _inventoryItemPrefab, InitializeItem);

        ShowList(_allDataJson);
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
        item.Highlight(index == _selectedIndex);
    }

    /// <summary>
    /// Shows item list from json.
    /// </summary>
    /// <param name="dataJson">JSON to generate items from. JSON must be an array of InventoryItemData.</param>
    private void ShowList(TextAsset dataJson)
    {
        _itemDatas = GenerateItemDatas(dataJson.text, _itemGenerateScale);
        _selectedIndex = -1;

        _scrollPool.ClearDisplay();
        _scrollPool.SetItemCount(_itemDatas.Length);
        _scrollPool.ResetDisplay();

        // Select the first item.
        InventoryItemOnClick(_scrollPool.GetTopItem(), 0);
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
