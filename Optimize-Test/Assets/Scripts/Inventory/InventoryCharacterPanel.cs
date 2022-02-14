using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class InventoryCharacterPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _stats;
    [SerializeField] private InventorySlot[] _slots;

    public event Action<InventoryCategory> ClickedSlot;

    private Dictionary<InventoryCategory, InventorySlot> _slotsBycategory;

    private void Awake()
    {
        _slotsBycategory = new Dictionary<InventoryCategory, InventorySlot>();

        foreach (var slot in _slots)
        {
            _slotsBycategory[slot.Category] = slot;
            slot.Button.onClick.AddListener(() => { ClickedSlot?.Invoke(slot.Category); });
        }
    }

    public void UpdateCategory(InventoryCategory category, Sprite sprite)
    {
        if (_slotsBycategory.ContainsKey(category))
            _slotsBycategory[category].Icon.sprite = sprite;
    }
}
