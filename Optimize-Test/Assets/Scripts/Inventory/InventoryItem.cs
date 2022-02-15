using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private SpriteProvider _spriteProvider;
    public Button Button;

    public void SetData(InventoryItemData data)
    {
        _icon.sprite = _spriteProvider.GetIcon(data.IconIndex);
        _name.text = data.Name;
    }

    public void Highlight(bool selected)
    {
        _background.color = selected ? Color.red : Color.white;
    }
}
