using UnityEngine;
using TMPro;

public class InventoryInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private StatsText _statsTextPrefab;
    [SerializeField] private Transform _statsTextParent;

    private StatsText[] _stats;

    private void Awake()
    {
        var names = StatsNames.Names;
        _stats = new StatsText[names.Length];

        for (int i = 0; i < names.Length; i++)
        {
            var statsText = Instantiate(_statsTextPrefab, _statsTextParent);
            statsText.Name.text = names[i].ToString();

            _stats[i] = statsText;
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

        for (int i = 0; i < _stats.Length; i++)
            _stats[i].Value.text = data.Stats[i].ToString();
    }
}
