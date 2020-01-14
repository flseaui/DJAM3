using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;
using Random = System.Random;

public static class BrightHolder
{
	public static readonly Dictionary<Vector3Int, Dictionary<Vector3Int, double>> BrightDict = new Dictionary<Vector3Int, Dictionary<Vector3Int, double>>();
}

public class LitTile : Tile
{
	public int Radius;
	public int Intensity;

	[NonSerialized]
	public float Brightness;

	private bool _startedUp;

	public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
	{
		if (Radius > 0 && Intensity > 0)
		{
			if (!BrightHolder.BrightDict.ContainsKey(position))
			{
				BrightHolder.BrightDict[position] = new Dictionary<Vector3Int, double> {{position, Intensity}};
				TileManager.Instance.RegisterActiveTile(position);
			}
		}
		
		return base.StartUp(position, tilemap, go);
	}

	public void OnTileRemoved(Vector3Int position, LitTile oldTile, Tilemap tilemap)
	{
		var radius = oldTile.Radius;
		var intensity = oldTile.Intensity;
		
		if (radius <= 0 || intensity <= 0) return;

		Debug.Log("GAMING");
		
		for (var x = -radius; x <= radius; x++)
		{
			for (var y = -radius; y <= radius; y++)
			{
				var rad = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
					
				if (rad > radius) continue;
					
				var pos = new Vector3Int(x + position.x, y + position.y, 0);
                					
				var tile = tilemap.GetTile<LitTile>(pos);
				if (tile != null)
				{
					var brightness = Mathf.Clamp(1 - (float) (rad / radius), .2f, 1);
					
					if (BrightHolder.BrightDict.ContainsKey(pos))
					{
						BrightHolder.BrightDict.Remove(pos);
					}
					
					tilemap.RefreshTile(pos);
				}
			}
		}
	}
	
	public override void RefreshTile(Vector3Int position, ITilemap tilemap)
	{
		base.RefreshTile(position, tilemap);
		
		if (Radius <= 0 || Intensity <= 0) return;
		
		for (var x = -Radius; x <= Radius; x++)
		{
			for (var y = -Radius; y <= Radius; y++)
			{
				var rad = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
					
				if (rad > Radius) continue;
					
				var pos = new Vector3Int(x + position.x, y + position.y, 0);
                					
				var tile = tilemap.GetTile<LitTile>(pos);
				if (tile != null)
				{
					var brightness = Mathf.Clamp(1 - (float) (rad / Radius), .2f, 1);
					//Debug.Log($"mountain dew ice: {pos}, {brightness}");
					if (!BrightHolder.BrightDict.ContainsKey(pos))
					{
						BrightHolder.BrightDict[pos] = new Dictionary<Vector3Int, double> {{position, brightness}};
					}
					else
					{
						if (!BrightHolder.BrightDict[pos].ContainsKey(position))
							BrightHolder.BrightDict[pos].Add(position, brightness);
					}

					tilemap.RefreshTile(pos);
				}
			}
		}
	}

	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
	{
		tileData.flags = TileFlags.None;
		tileData.sprite = sprite;
		tileData.transform = transform;
		if (Application.isPlaying)
		{
			float bright;
			if (!BrightHolder.BrightDict.ContainsKey(position))
				bright = .2f;
			else
			{
				var brightSum = 0.0;
				foreach (var brightness in BrightHolder.BrightDict[position])
				{
					brightSum += brightness.Value;
				}
				brightSum = Math.Min(1, brightSum);
				bright = (float) brightSum;
			}

			tilemap.GetComponent<Tilemap>()
				.SetColor(position, Color.HSVToRGB(0, 0, bright));
		}
		else
			tileData.color = Color.white;
	}

#if UNITY_EDITOR
	[MenuItem("Assets/Create/LitTile", priority = 0)]
	public static void CreateLitTile()
	{
		var path = EditorUtility.SaveFilePanelInProject("Save Lit Tile", "New Lit Tile", "Asset", "Save Lit Tile", "Assets");
		if (path == "")
			return;
		AssetDatabase.CreateAsset(CreateInstance<LitTile>(), path);
	}

#endif
}

[CustomEditor(typeof(LitTile))]
public class LitTileEditor : Editor
{
	/*public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
	{
		var tile = AssetDatabase.LoadAssetAtPath<Tile>(assetPath);
		
		if (tile.sprite == null) return null;
		
		var spritePreview = AssetPreview.GetAssetPreview(tile.sprite); // Get sprite texture
     
		var pixels = spritePreview.GetPixels();
		for (var i = 0; i < pixels.Length; i++)
		{
			pixels[i] = pixels[i] * tile.color; // Tint
		}
		spritePreview.SetPixels(pixels);
		spritePreview.Apply();
     
		var preview = new Texture2D(width, height);
		EditorUtility.CopySerialized(spritePreview, preview); // Returning the original texture causes an editor crash
		return preview;
	}*/
}