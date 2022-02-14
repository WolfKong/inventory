using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private Image _background;
    public Image Icon;
    public TextMeshProUGUI Name;
    public Button Button;

    public void Highlight(bool selected)
    {
        _background.color = selected ? Color.red : Color.white;
    }
}
