using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : Singleton<TileManager>
{
    private List<Vector3Int> _activeTiles;

    [SerializeField] private Tilemap _tilemap;
    
    public void RegisterActiveTile(Vector3Int tilePos)
    {
        _activeTiles.Add(tilePos);
    }

    protected sealed override void OnAwake()
    {
        _activeTiles = new List<Vector3Int>();
        
        InvokeRepeating(nameof(RefreshTiles), 2, 1);
    }

    public void PlaceTile(Vector3Int tilePos, Tile tile)
    {
        var oldTile = _tilemap.GetTile<LitTile>(tilePos);
        if (oldTile != null)
        {
            Debug.Log("WOO YEAH WOO");
            Debug.Log(oldTile);
            Debug.Log(tilePos);
            _tilemap.GetTile<LitTile>(tilePos).OnTileRemoved(tilePos, oldTile, _tilemap);
        }
        
        _tilemap.SetTile(tilePos, tile);


    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(_activeTiles.Count);
            _tilemap.RefreshTile(_activeTiles[0]);
        }
    }
    
    private void RefreshTiles()
    {
        for (var i = 0; i < _activeTiles.Count - 1; i++)
        {
            _tilemap.RefreshTile(_activeTiles[i]);
        }
    }
}