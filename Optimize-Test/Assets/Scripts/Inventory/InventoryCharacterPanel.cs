using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class InventoryCharacterPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _totalFur;
    [SerializeField] private TextMeshProUGUI _totalTeeth;
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
        var totalFur = 0;
        var totalTeeth = 0;

        foreach (var category in _slotsBycategory.Keys)
        {
            if (category.SelectedData != null)
            {
                totalFur += category.SelectedData.Fur;
                totalTeeth += category.SelectedData.Teeth;
            }
        }

        _totalFur.text = $"{totalFur}";
        _totalTeeth.text = $"{totalTeeth}";
    }
}
