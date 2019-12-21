using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PaletteSlot : MonoBehaviour
{
    public Tile Tile;

    [SerializeField] private Sprite _unselectedSprite;
    [SerializeField] private Sprite _selectedSprite;

    private Image _slotSprite;

    private void Awake()
    {
        _slotSprite = GetComponent<Image>();
    }

    public void SetSelected(bool selected)
    {
        _slotSprite.sprite = selected ? _selectedSprite : _unselectedSprite;
    }
    
    public void Select()
    {
        ObjectPalette.SlotSelected?.Invoke(this);
    }
}