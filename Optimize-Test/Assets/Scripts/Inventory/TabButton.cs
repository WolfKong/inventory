using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    [SerializeField] private Image _background;
    public TextMeshProUGUI Label;
    public int Index;
    public Button Button;

    public void Highlight(bool selected)
    {
        _background.color = selected ? Color.red : Color.white;
    }
}
