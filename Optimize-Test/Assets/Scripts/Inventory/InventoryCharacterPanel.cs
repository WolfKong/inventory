using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class InventoryCharacterPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _stats;
    [SerializeField] private InventorySlot[] _slots;

    public event Action<InventoryCategory> ClickedSlot;

    private Dictionary<InventoryCategory, InventorySlot> _slotsBycategory;

    private void Awake()
    {
        _slotsBycategory = new Dictionary<InventoryCategory, InventorySlot>();

        foreach (var slot in _slots)
        {
            _slotsBycategory[slot.Category] = slot;
            slot.Button.onClick.AddListener(() => { ClickedSlot?.Invoke(slot.Category); });
        }
    }

    public void UpdateCategory(InventoryCategory category, InventoryItemData itemData, Sprite[] _icons)
    {
        if (_slotsBycategory.ContainsKey(category))
            _slotsBycategory[category].Icon.sprite = _icons[itemData.IconIndex];

        UpdateTotalStats();
    }

    private void UpdateTotalStats()
    {
        var total = 0;

        foreach (var category in _slotsBycategory.Keys)
            total += category.SelectedData != null ? category.SelectedData.Stat : 0;

        _stats.text = $"{total}";
    }
}
