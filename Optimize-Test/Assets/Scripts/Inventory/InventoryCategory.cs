using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "InventoryCategory", menuName = "ScriptableObjects/InventoryCategory")]
public class InventoryCategory : ScriptableObject
{
    public string Name;
    public TextAsset ItemsDataJson;
    [NonSerialized] public int SelectedIndex = -1;

    public override string ToString() => $"Name: {Name}, DataJson: {ItemsDataJson}, SelectedIndex: {SelectedIndex}";
}
