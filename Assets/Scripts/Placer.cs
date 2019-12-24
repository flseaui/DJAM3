using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class Placer : MonoBehaviour
{
    [SerializeField] private ObjectPalette _objectPalette;
    
    [SerializeField]
    private Tilemap _tilemap;

    private Tile _prevTile;

    [SerializeField]
    private Tile _selectorTile;

    private Vector3Int _selectorPos;

    private Vector3Int _placedOn;

    private void Awake()
    {
        _selectorPos = new Vector3Int(1000, 1000, 1000);
    }
    
    void Update()
    {
        var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0;
        var tilePos =  _tilemap.WorldToCell(worldPos);
        
        if (_tilemap.GetTile<Tile>(tilePos) == null) return;
        
        tilePos.z = 1;

        if (Input.GetMouseButton(0))
        {
            _tilemap.SetTile(tilePos, _objectPalette.SelectedObject);
            _placedOn = tilePos;
            _prevTile = null;
            _selectorPos = new Vector3Int(1000, 1000, 1000);
        }
        else
        {
            if (tilePos == _placedOn)
                return;

            _placedOn = new Vector3Int(1000, 1000, 1000);
                
            _tilemap.SetTile(_selectorPos, _prevTile);
            _prevTile = _tilemap.GetTile<Tile>(tilePos);
            _tilemap.SetTile(tilePos, _selectorTile);
            _selectorPos = tilePos;

        }
    }
}
