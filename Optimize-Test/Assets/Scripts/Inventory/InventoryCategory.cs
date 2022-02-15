using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "InventoryCategory", menuName = "ScriptableObjects/InventoryCategory")]
public class InventoryCategory : ScriptableObject
{
    public string Name;

    [Tooltip("JSON to generate items from. JSON must be an array of InventoryItemData.")]
    [SerializeField] private TextAsset _itemsDataJson;

    [NonSerialized] public int SelectedIndex = -1;
    [NonSerialized] public InventoryItemData[] ItemsData = new InventoryItemData[0];

    public InventoryItemData SelectedData =>
        (SelectedIndex >= 0 && SelectedIndex < ItemsData.Length) ? ItemsData[SelectedIndex] : null;

    [Serializable]
    private class InventoryItemDatas
    {
        public InventoryItemData[] ItemDatas;
    }

    /// <summary>
    /// Generates category item list.
    /// </summary>
    /// <param name="scale">Concats additional copies of the array parsed from json.</param>
    public void GenerateItemDatas(int scale)
    {
        var itemDatas = JsonUtility.FromJson<InventoryItemDatas>(_itemsDataJson.text).ItemDatas;
        var finalItemDatas = new InventoryItemData[itemDatas.Length * scale];
        for (var i = 0; i < itemDatas.Length; i++)
        {
            for (var j = 0; j < scale; j++)
            {
                finalItemDatas[i + j*itemDatas.Length] = itemDatas[i];
            }
        }

        ItemsData = finalItemDatas;
    }

    public override string ToString() => $"Name: {Name}, DataJson: {_itemsDataJson}, SelectedIndex: {SelectedIndex}";
}
