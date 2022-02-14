using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class InventoryCharacterPanel : MonoBehaviour
{
    [SerializeField] private InventorySlot[] _slots;
    [SerializeField] private StatsText _statsTextPrefab;
    [SerializeField] private Transform _statsTextParent;
    [SerializeField] private TabButton _topButtonsPrefab;
    [SerializeField] private Transform _topButtonsParent;

    public event Action<InventoryCategory> ClickedSlot;
    public event Action Optimize;

    private Dictionary<InventoryCategory, InventorySlot> _slotsBycategory;
    private StatsText[] _statsTexts;

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

        // Creates a statsText for each name and one more for their sum
        var names = StatsNames.Names;
        _statsTexts = new StatsText[names.Length + 1];

        for (int i = 0; i <= names.Length; i++)
        {
            var statsText = Instantiate(_statsTextPrefab, _statsTextParent);
            var name = i < names.Length ? names[i].ToString() : "Sum";
            statsText.Name.text = $"{name}:";

            _statsTexts[i] = statsText;

            var topButton = Instantiate<TabButton>(_topButtonsPrefab, _topButtonsParent);
            topButton.Label.text = $"Optimize {name}";
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
        var optimizeSum = statsIndex >= StatsNames.Names.Length;

        foreach (var category in _slotsBycategory.Keys)
        {
            var itemsData = category.ItemsData;
            var maxValue = 0;
            var maxIndex = 0;

            for (int i = 0; i < itemsData.Length; i++)
            {
                var item = itemsData[i];
                var value = optimizeSum ? item.Stats.Sum() : item.Stats[statsIndex];

                if (value > maxValue)
                {
                    maxValue = value;
                    maxIndex = i;
                }
            }

            category.SelectedIndex = maxIndex;
        }

        Optimize?.Invoke();
    }

    private void UpdateTotalStats()
    {
        var totals = new int[_statsTexts.Length];

        foreach (var category in _slotsBycategory.Keys)
        {
            if (category.SelectedData != null)
            {
                var stats = category.SelectedData.Stats;
                for (int i = 0; i < stats.Length; i++)
                    totals[i] += stats[i];
            }
        }

        for (int i = 0; i < _statsTexts.Length - 1; i++)
            _statsTexts[i].Value.text = $"{totals[i]}";

        _statsTexts[_statsTexts.Length - 1].Value.text = $"{totals.Sum()}";
    }
}
