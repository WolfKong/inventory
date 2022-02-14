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
    [SerializeField] private TabButton _topButtonsPrefab;
    [SerializeField] private Transform _topButtonsParent;

    public event Action<InventoryCategory> ClickedSlot;
    public event Action Optimize;

    private Dictionary<InventoryCategory, InventorySlot> _slotsBycategory;
    private StatsText[] _stats;

    private void Awake()
    {
        // Clear existing topButtons already in the list.
        foreach (TabButton topButton in _topButtonsParent.GetComponentsInChildren<TabButton>())
            Destroy(topButton.gameObject);

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

            var topButton = Instantiate<TabButton>(_topButtonsPrefab, _topButtonsParent);
            topButton.Label.text = $"Optimize {names[i]}";
            topButton.Index = i;
            topButton.Button.onClick.AddListener(() => { OptimizeStats(topButton.Index); });
        }
    }

    public void UpdateCategory(InventoryCategory category, InventoryItemData itemData, Sprite[] _icons)
    {
        if (_slotsBycategory.ContainsKey(category))
            _slotsBycategory[category].Icon.sprite = _icons[itemData.IconIndex];

        UpdateTotalStats();
    }

    private void OptimizeStats(int statsIndex)
    {
        foreach (var category in _slotsBycategory.Keys)
        {
            var itemsData = category.ItemsData;
            var maxValue = 0;
            var maxIndex = 0;

            for (int i = 0; i < itemsData.Length; i++)
            {
                var item = itemsData[i];
                if (item.Stats[statsIndex] > maxValue)
                {
                    maxValue = item.Stats[statsIndex];
                    maxIndex = i;
                }
            }

            category.SelectedIndex = maxIndex;
        }

        Optimize?.Invoke();
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
