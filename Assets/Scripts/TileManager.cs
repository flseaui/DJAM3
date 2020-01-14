using System.Collections.Generic;
using System.Linq;
using Ludiq.PeekCore.TinyJson;
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
        
        InvokeRepeating(nameof(RefreshTiles), 0, .1f);
    }

    public void PlaceTile(Vector3Int tilePos, Tile tile, LitTile oldTile)
    {
        _tilemap.SetTile(tilePos, tile);    
        if (oldTile != null && tile != oldTile)
            _tilemap.GetTile<LitTile>(tilePos).OnTileRemoved(tilePos, oldTile, _tilemap);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _tilemap.RefreshAllTiles();
        }
    }
    
    private void RefreshTiles()
    {
        for (var i = 0; i < _activeTiles.Count; i++)
        {
            // _tilemap.GetTile<LitTile>(_activeTiles[i]).RefreshTile(_activeTiles[i], null);
            _tilemap.RefreshTile(_activeTiles[i]);
           //_tilemap.SetTile(_activeTiles[i], _tilemap.GetTile<LitTile>(_activeTiles[i]));
        }
    }
}