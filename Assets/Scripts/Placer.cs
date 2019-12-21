using UnityEngine;
using UnityEngine.Tilemaps;

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
    
    void Update()
    {
        var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var tilePos =  _tilemap.LocalToCell(worldPos);

        tilePos.z = 0;
        tilePos.x -= 7;
        tilePos.y -= 5;
        
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
            {
                return;
            }
            else
            {
                
                _placedOn = new Vector3Int(1000, 1000, 1000);
                
                _tilemap.SetTile(_selectorPos, _prevTile);
                _prevTile = _tilemap.GetTile<Tile>(tilePos);
                _tilemap.SetTile(tilePos, _selectorTile);
                _selectorPos = tilePos;
            }
            
        }

    }
}
