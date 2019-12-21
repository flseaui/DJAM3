using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectPalette : MonoBehaviour
{
    private PaletteSlot[] _slots;

    public Tile SelectedObject;

    public static Action<PaletteSlot> SlotSelected;
    
    private void Awake()
    {
        _slots = GetComponentsInChildren<PaletteSlot>();
        SlotSelected += OnSlotSelected;
    }

    private void OnSlotSelected(PaletteSlot slot)
    {
        SelectedObject = slot.Tile;
        foreach (var slotObj in _slots)
        {
            slotObj.SetSelected(false);
        }

        slot.SetSelected(true);
    }
    
}