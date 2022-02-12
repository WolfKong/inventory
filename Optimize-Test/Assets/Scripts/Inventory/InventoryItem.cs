using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public Image Background;
    public Image Icon;
    public TextMeshProUGUI Name;
    public Button Button;

    public void Highlight(bool selected)
    {
        Background.color = selected ? Color.red : Color.white;
    }
}
