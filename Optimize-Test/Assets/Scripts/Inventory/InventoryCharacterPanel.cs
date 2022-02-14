using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class InventoryCharacterPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] _statsTotals;
    [SerializeField] private InventorySlot[] _slots;
    [SerializeField] private StatsText _statsTextPrefab;
    [SerializeField] private Transform _statsTextParent;

    public event Action<InventoryCategory> ClickedSlot;

    private Dictionary<InventoryCategory, InventorySlot> _slotsBycategory;
    private StatsText[] _stats;

    private void Awake()
    {
        _slotsBycategory = new Dictionary<InventoryCategory, InventorySlot>();

        foreach (var slot in _slots)
        {
            _slotsBycategory[slot.Category] = slot;
            slot.Button.onClick.AddListener(() => { ClickedSlot?.Invoke(slot.Category); });
        }

        var names = StatsNames.Names;
        _stats = new StatsText[names.Length];

        for (int i = 0; i < names.Length; i++)
        {
            var statsText = Instantiate(_statsTextPrefab, _statsTextParent);
            statsText.Name.text = names[i].ToString();

            _stats[i] = statsText;
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
        var totals = new int[_stats.Length];

        foreach (var category in _slotsBycategory.Keys)
        {
            if (category.SelectedData != null)
            {
                var stats = category.SelectedData.Stats;
                for (int i = 0; i < stats.Length; i++)
                    totals[i] += stats[i];
            }
        }

        for (int i = 0; i < _statsTotals.Length; i++)
            _stats[i].Value.text = $"{totals[i]}";
    }
}
