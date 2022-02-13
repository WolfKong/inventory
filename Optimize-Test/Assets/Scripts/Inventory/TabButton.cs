using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    public Image Background;
    public TextMeshProUGUI Label;
    public Button Button;

    public void Highlight(bool selected)
    {
        Background.color = selected ? Color.red : Color.white;
    }
}
