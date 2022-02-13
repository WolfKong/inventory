using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image Icon;
    public Button Button;
    public int SelectedIndex = -1;
    public InventoryItem SelectedItem;
    public TabButton Tab;
}
