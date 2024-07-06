using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World : MonoBehaviour
{

    public GameObject tilePrefab;

    private Dictionary<(int, int), Tile.Type> data;
    private Dictionary<(int, int), GameObject> tiles;

    // Start is called before the first frame update
    void Start()
    {
        data = new Dictionary<(int, int), Tile.Type>();
        tiles = new Dictionary<(int, int), GameObject>();
        CreateTile(0, 0, Tile.Type.GRASS);
        CreateTile(1, 1, Tile.Type.GRASS);
        CreateTile(1, 0, Tile.Type.STONE);
        CreateTile(2, 0, Tile.Type.STONE);
        CreateTile(1, -1, Tile.Type.STONE);
    }

    private (int, int)[] GetNeighbors(int x, int y)
    {
        (int, int)[] neighbors = {
            (x - 1, y),
            (x + 1, y),
            (x, y + 1),
            (x,  y - 1),
        };
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
        RemoveTile(x, y);
    }

    public void RemoveTile(int x, int y)
    {
        Tile.Type type = data[(x, y)];
        data.Remove((x, y));
        Destroy(tiles[(x, y)]);
        tiles.Remove((x, y));
        if (type != Tile.Type.AIR)
        {
            (int, int)[] neighbors = GetNeighbors(x, y);
            bool isolated = true;
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (data.ContainsKey(neighbors[i]))
                {
                    if (data[neighbors[i]] == Tile.Type.AIR)
                    {
                        RemoveIfIsolated(x, y);
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
                for (int i = 0; i < neighbors.Length; i++)
                {
                    if (data.ContainsKey(neighbors[i]) && data[neighbors[i]] != Tile.Type.AIR)
                    {
                        UpdateNeighbors(neighbors[i].Item1, neighbors[i].Item2);
                    }
                }
            }
        }
    }

    private void UpdateNeighbors(int x, int y)
    {
        if (!tiles.ContainsKey((x, y)))
            return;
        (int, int)[] neighbors = GetNeighbors(x, y);
        GameObject tile = tiles[(x, y)];
        Tile tileScript = tile.GetComponent<Tile>();
        tileScript.SetNeighbors(neighbors.Select(x => data.TryGetValue(x, out var value) && value != Tile.Type.AIR ? (Tile.Type?)value : null).ToArray());
    }

    public void CreateTile(int x, int y, Tile.Type type)
    {
        if (tiles.ContainsKey((x, y)))
        {
            RemoveTile(x, y);
        }
        data[(x, y)] = type;
        GameObject tile = GameManager.CreateTile(x, y, type);
        tiles[(x, y)] = tile;
        Tile tileScript = tile.GetComponent<Tile>();
        (int, int)[] neighbors = GetNeighbors(x, y);
        if (type != Tile.Type.AIR)
        {
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (!tiles.ContainsKey(neighbors[i]))
                {
                    CreateTile(neighbors[i].Item1, neighbors[i].Item2, Tile.Type.AIR);
                }
                else
                {
                    UpdateNeighbors(neighbors[i].Item1, neighbors[i].Item2);
                }
            }
        }
        UpdateNeighbors(x, y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
