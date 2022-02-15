using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Reflection;

public class InventoryInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private StatsText _statsTextPrefab;
    [SerializeField] private Transform _statsTextParent;

    private Dictionary<FieldInfo, StatsText> _statsByField;

    private void Awake()
    {
        _statsByField = new Dictionary<FieldInfo, StatsText>();

        foreach (var fieldInfo in typeof(ItemStats).GetFields())
        {
            var statsText = Instantiate(_statsTextPrefab, _statsTextParent);
            statsText.Name.text = fieldInfo.Name;

            _statsByField.Add(fieldInfo, statsText);
        }
    }

    /// <summary>
    /// Sets information of given item.
    /// </summary>
    /// <param name="data">Data to be displayed.</param>
    public void SetData(InventoryItemData data)
    {
        _name.text = data.Name;
        _description.text = data.Description;

        foreach (var pair in _statsByField)
            pair.Value.Value.text = pair.Key.GetValue(data.Stats).ToString();
    }
}
