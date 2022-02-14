using UnityEngine;
using TMPro;

public class InventoryInfoPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _statsFur;
    [SerializeField] private TextMeshProUGUI _statsTeeth;

    public void SetData(InventoryItemData data)
    {
        _name.text = data.Name;
        _description.text = data.Description;
        _statsFur.text = data.Fur.ToString();
        _statsTeeth.text = data.Teeth.ToString();
    }
}
