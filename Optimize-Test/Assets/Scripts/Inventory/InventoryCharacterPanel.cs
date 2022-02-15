using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;

public class InventoryCharacterPanel : MonoBehaviour
{
    [SerializeField] private InventorySlot[] _slots;
    [SerializeField] private StatsText _statsTextPrefab;
    [SerializeField] private Transform _statsTextParent;
    [SerializeField] private TabButton _topButtonsPrefab;
    [SerializeField] private Transform _topButtonsParent;
    [SerializeField] private SpriteProvider _spriteProvider;

    public event Action<InventoryCategory> ClickedSlot;
    public event Action RanOptimization;

    private Dictionary<InventoryCategory, InventorySlot> _slotsBycategory;
    private Dictionary<FieldInfo, StatsText> _statsByField;
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

        // Creates a statsText and topButton for each stats and one more for their sum
        _statsByField = new Dictionary<FieldInfo, StatsText>();

        foreach (var fieldInfo in typeof(ItemStats).GetFields())
            _statsByField[fieldInfo] = CreateStatsText(fieldInfo, fieldInfo.Name);

        _sumText = CreateStatsText(null, "Sum", true);
    }

    /// <summary>
    /// Creates a statsText and topButton using given parameters.
    /// </summary>
    /// <param name="fieldInfo">FieldInfo of stats.</param>
    /// <param name="name">Name of stats.</param>
    /// <param name="isSum">Are components for displaying sum of stats?</param>
    /// <returns>StatsText</returns>
    private StatsText CreateStatsText(FieldInfo fieldInfo, string name, bool isSum = false)
    {
        var statsText = Instantiate(_statsTextPrefab, _statsTextParent);
        statsText.Name.text = $"{name}:";

        var topButton = Instantiate<TabButton>(_topButtonsPrefab, _topButtonsParent);
        topButton.Label.text = $"Optimize {name}";
        topButton.Button.onClick.AddListener(() => { OptimizeStats(fieldInfo, isSum); });

        return statsText;
    }

    public void UpdateCategory(InventoryCategory category, InventoryItemData itemData)
    {
        if (_slotsBycategory.ContainsKey(category))
            _slotsBycategory[category].Icon.sprite = _spriteProvider.GetIcon(itemData.IconIndex);

        UpdateStats();
    }

    /// <summary>
    /// Calculates which are the best items to maximize given stats or stats sum.
    /// </summary>
    /// <param name="fieldInfo">FieldInfo of stats to be optimized.</param>
    /// <param name="optimizeSum">Should optimize total of stats sum.</param>
    private void OptimizeStats(FieldInfo fieldInfo, bool optimizeSum)
    {
        foreach (var category in _slotsBycategory.Keys)
        {
            var itemsData = category.ItemsData;
            var maxValue = 0;
            var maxIndex = 0;

            for (int i = 0; i < itemsData.Length; i++)
            {
                var item = itemsData[i];
                var value = optimizeSum ? item.Stats.Sum : (int)fieldInfo.GetValue(item.Stats);

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
        var totals = new Dictionary<FieldInfo, int>();
        var fieldInfos = typeof(ItemStats).GetFields();

        foreach (var fieldInfo in fieldInfos)
            totals[fieldInfo] = 0;

        foreach (var category in _slotsBycategory.Keys)
        {
            if (category.SelectedData != null)
            {
                foreach (var fieldInfo in fieldInfos)
                    totals[fieldInfo] += (int)fieldInfo.GetValue(category.SelectedData.Stats);
            }
        }

        foreach (var pair in _statsByField)
            pair.Value.Value.text = $"{totals[pair.Key]}";

        _sumText.Value.text = $"{totals.Values.Sum()}";
    }
}
