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
    public event Action RanOptimization;

    private Dictionary<InventoryCategory, InventorySlot> _slotsBycategory;
    private StatsText[] _statsTexts;
    private StatsText _sumText;

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

        // Creates a statsText and topButton for each name and one more for their sum
        var names = StatsNames.Names;
        _statsTexts = new StatsText[names.Length];

        for (int i = 0; i < names.Length; i++)
            _statsTexts[i] = CreateStatsText(i, names[i]);

        _sumText = CreateStatsText(-1, "Sum", true);
    }

    /// <summary>
    /// Creates a statsText and topButton using given parameters.
    /// </summary>
    /// <param name="index">Index of stats.</param>
    /// <param name="name">Name of stats.</param>
    /// <param name="isSum">Are components for displaying sum of stats?</param>
    private StatsText CreateStatsText(int index, string name, bool isSum = false)
    {
        var statsText = Instantiate(_statsTextPrefab, _statsTextParent);
        statsText.Name.text = $"{name}:";

        var topButton = Instantiate<TabButton>(_topButtonsPrefab, _topButtonsParent);
        topButton.Label.text = $"Optimize {name}";
        topButton.Button.onClick.AddListener(() => { OptimizeStats(index, isSum); });

        return statsText;
    }

    public void UpdateCategory(InventoryCategory category, InventoryItemData itemData, Sprite[] _icons)
    {
        if (_slotsBycategory.ContainsKey(category))
            _slotsBycategory[category].Icon.sprite = _icons[itemData.IconIndex];

        UpdateStats();
    }

    /// <summary>
    /// Calculates which are the best items to maximize given stats or stats sum.
    /// </summary>
    /// <param name="statsIndex">Index of stats to be optimized.</param>
    /// <param name="optimizeSum">Should optimize total of stats sum.</param>
    private void OptimizeStats(int statsIndex, bool optimizeSum)
    {
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

        RanOptimization?.Invoke();
    }

    /// <summary>
    /// Sums each stats for all categories and displays them.
    /// </summary>
    private void UpdateStats()
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

        for (int i = 0; i < _statsTexts.Length; i++)
            _statsTexts[i].Value.text = $"{totals[i]}";

        _sumText.Value.text = $"{totals.Sum()}";
    }
}
