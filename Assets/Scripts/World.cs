using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class World : MonoBehaviour
{

    private Dictionary<(int, int), Tile.Type> data;
    private Dictionary<(int, int), GameObject> tiles;

    private Tilemap tilemap;

    public UnityEngine.Tilemaps.Tile blankTile;

    private static World instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        data = new Dictionary<(int, int), Tile.Type>();
        tiles = new Dictionary<(int, int), GameObject>();
        for (int i = -5; i < 5; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (UnityEngine.Random.Range(0, 2) > 0)
                    CreateTile(i, -j, UnityEngine.Random.Range(0, 2) > 0 ? Tile.Type.GRASS : Tile.Type.STONE);
            }
        }
    }

    public static (int, int)[] GetNeighbors(int x, int y)
    {
        (int, int)[] neighbors = {
            (x - 1, y),
            (x + 1, y),
            (x, y + 1),
            (x,  y - 1),
        };
        return neighbors;
    }

    public static (int, int)[] GetCornerNeighbors(int x, int y)
    {
        (int, int)[] neighbors = {
            (x - 1, y - 1),
            (x + 1, y - 1),
            (x + 1, y + 1),
            (x - 1,  y + 1),
        };
        return neighbors;
    }

    public static (int, int)[] GetAllNeighbors(int x, int y)
    {
        (int, int)[] immediateNeighbors = GetNeighbors(x, y);
        (int, int)[] cornerNeighbors = GetCornerNeighbors(x, y);
        (int, int)[] neighbors = new (int, int)[immediateNeighbors.Length + cornerNeighbors.Length];
        immediateNeighbors.CopyTo(neighbors, 0);
        cornerNeighbors.CopyTo(neighbors, immediateNeighbors.Length);
        return neighbors;
    }

    private void RemoveIfIsolated(int x, int y)
    {
        (int, int)[] neighbors = GetNeighbors(x, y);
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (data.TryGetValue(neighbors[i], out var type) && type != Tile.Type.AIR)
            {
                return;
            }
        }
        RemoveTile(x, y, true);
    }

    public static bool TileExists(int x, int y)
    {
        return instance.data.ContainsKey((x, y)) && instance.data[(x, y)] != Tile.Type.AIR;
    }

    public static bool RemoveTile(int x, int y, bool removeAir = false)
    {
        if (!instance.data.ContainsKey((x, y)))
            return false;
        Tile.Type type = instance.data[(x, y)];
        if (type == Tile.Type.AIR && !removeAir)
            return false;
        instance.data.Remove((x, y));
        Destroy(instance.tiles[(x, y)]);
        instance.tiles.Remove((x, y));
        if (type != Tile.Type.AIR)
        {
            instance.tilemap.SetTile(new Vector3Int(x, y), null);
            (int, int)[] neighbors = GetNeighbors(x, y);
            (int, int)[] cornerNeighbors = GetCornerNeighbors(x, y);
            bool isolated = true;
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (instance.data.ContainsKey(neighbors[i]))
                {
                    if (instance.data[neighbors[i]] == Tile.Type.AIR)
                    {
                        instance.RemoveIfIsolated(x, y);
                    }
                    else
                    {
                        isolated = false;
                    }
                }
            }
            if (!isolated)
            {
                CreateTile(x, y, Tile.Type.AIR);
            }
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (instance.data.ContainsKey(neighbors[i]))
                {
                    instance.UpdateNeighbors(neighbors[i].Item1, neighbors[i].Item2);
                }
            }
            for (int i = 0; i < cornerNeighbors.Length; i++)
            {
                if (instance.data.ContainsKey(cornerNeighbors[i]))
                {
                    instance.UpdateNeighbors(cornerNeighbors[i].Item1, cornerNeighbors[i].Item2);
                }
            }
        }
        return true;
    }

    private void UpdateNeighbors(int x, int y)
    {
        if (!tiles.ContainsKey((x, y)))
            return;
        (int, int)[] neighbors = GetAllNeighbors(x, y);
        GameObject tile = tiles[(x, y)];
        Tile tileScript = tile.GetComponent<Tile>();
        tileScript.SetNeighbors(neighbors.Select(x => data.TryGetValue(x, out var value) && value != Tile.Type.AIR ? (Tile.Type?)value : null).ToArray());
    }

    public static void CreateTile(int x, int y, Tile.Type type)
    {
        if (instance.tiles.ContainsKey((x, y)))
        {
            RemoveTile(x, y, true);
        }
        instance.data[(x, y)] = type;
        GameObject tile = GameManager.CreateTile(x, y, type);
        instance.tiles[(x, y)] = tile;
        Tile tileScript = tile.GetComponent<Tile>();
        (int, int)[] neighbors = GetNeighbors(x, y);
        (int, int)[] cornerNeighbors = GetCornerNeighbors(x, y);
        if (type != Tile.Type.AIR)
        {
            instance.tilemap.SetTile(new Vector3Int(x, y), instance.blankTile);
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (!instance.tiles.ContainsKey(neighbors[i]))
                {
                    CreateTile(neighbors[i].Item1, neighbors[i].Item2, Tile.Type.AIR);
                }
                else
                {
                    instance.UpdateNeighbors(neighbors[i].Item1, neighbors[i].Item2);
                }
            }
            for (int i = 0; i < cornerNeighbors.Length; i++)
            {
                if (instance.tiles.ContainsKey(cornerNeighbors[i]))
                {
                    instance.UpdateNeighbors(cornerNeighbors[i].Item1, cornerNeighbors[i].Item2);
                }
            }
        }
        instance.UpdateNeighbors(x, y);
    }

    public static Vector3Int GetTilePosition(Vector3 mousePos)
    {
        return instance.tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(mousePos));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
