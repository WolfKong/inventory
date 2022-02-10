using UnityEngine;
using TMPro;

public class InventoryInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _stats;

    public void SetData(InventoryItemData data)
    {
        _name.text = data.Name;
        _description.text = data.Description;
        _stats.text = data.Stat.ToString();
    }
}
