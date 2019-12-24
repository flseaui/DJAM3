using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using static WorldExpander.WorldSide;
using Random = UnityEngine.Random;

public class WorldExpander : MonoBehaviour
{
    [SerializeField]
    private Tilemap _tilemap;

    [SerializeField]
    private Tile _groundTile;
    
    private void Awake()
    {
        TimeManager.NextYear += OnNextYear;
    }

    private void OnNextYear()
    {
        ExpandWorld((WorldSide) Random.Range(0, 3));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) ExpandWorld(TopLeft);
        if (Input.GetKeyDown(KeyCode.UpArrow)) ExpandWorld(TopRight);
        if (Input.GetKeyDown(KeyCode.DownArrow)) ExpandWorld(BottomLeft);
        if (Input.GetKeyDown(KeyCode.RightArrow)) ExpandWorld(BottomRight);
    }
    
    public enum WorldSide
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
    
    private void ExpandWorld(WorldSide side)
    {
        
        _tilemap.CompressBounds();

        var bounds = _tilemap.cellBounds;
        var edge = new Vector3Int(0, 0, 0);
        
        switch (side)
        {
            case TopLeft:
                edge = new Vector3Int(0, bounds.yMax, 0);
        
                for (var i = bounds.xMin; i < bounds.xMax; i++)
                {
                    edge.x = i;
                    _tilemap.SetTile(edge, _groundTile);   
                }
                break;
            case TopRight:
                edge = new Vector3Int(bounds.xMax, 0, 0);
        
                for (var i = bounds.yMin; i < bounds.yMax; i++)
                {
                    edge.y = i;
                    _tilemap.SetTile(edge, _groundTile);   
                }
                break;
            case BottomLeft:
                edge = new Vector3Int(bounds.xMin - 1, 0, 0);
        
                for (var i = bounds.yMin; i < bounds.yMax; i++)
                {
                    edge.y = i;
                    _tilemap.SetTile(edge, _groundTile);   
                }
                break;
            case BottomRight:
                edge = new Vector3Int(0, bounds.yMin - 1, 0);
        
                for (var i = bounds.xMin; i < bounds.xMax; i++)
                {
                    edge.x = i;
                    _tilemap.SetTile(edge, _groundTile);   
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(side), side, null);
        }
    }
}