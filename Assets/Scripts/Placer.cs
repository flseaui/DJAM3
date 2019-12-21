using UnityEngine;
using UnityEngine.Tilemaps;

public class Placer : MonoBehaviour
{

    [SerializeField] private ObjectPalette _objectPalette;
    
    [SerializeField]
    private Tilemap _tilemap;

    [SerializeField]
    private SpriteRenderer _ghostTile;

    private Tile _prevTile;
    
    private void Awake()
    {
        
    }
    
    void Update()
    {
        var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var tilePos =  _tilemap.LocalToCell(worldPos);
        tilePos.z = 0;
        tilePos.x -= 6;
        tilePos.y -= 6;
        Debug.Log(tilePos);
        _tilemap.SetTile(tilePos, null);
    }
}
