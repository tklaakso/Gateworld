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
    private List<GameObject> entities;

    public GameObject tilePrefab;

    public static Dictionary<int, int> TopLayer { get; private set; }

    private Tilemap tilemap;

    public UnityEngine.Tilemaps.Tile blankTile;

    private TerrainGenerator terrainGen;

    public void Initialize()
    {
        TopLayer = new Dictionary<int, int>();
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        data = new Dictionary<(int, int), Tile.Type>();
        tiles = new Dictionary<(int, int), GameObject>();
        entities = new List<GameObject>();
        terrainGen = new TerrainGenerator();
        foreach (KeyValuePair<(int, int), Tile.Type> item in terrainGen.Data.ToList())
        {
            CreateTile(item.Key.Item1, item.Key.Item2, item.Value);
        }
        foreach (((float, float), Entity.Type) item in terrainGen.Entities.ToList())
        {
            CreateEntity(item.Item1.Item1, item.Item1.Item2, item.Item2);
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

    public bool TileExists(int x, int y)
    {
        return data.ContainsKey((x, y)) && data[(x, y)] != Tile.Type.AIR;
    }

    public Tile.Type GetTile(int x, int y)
    {
        if (!TileExists(x, y))
            return Tile.Type.AIR;
        return data[(x, y)];
    }

    public bool RemoveTile(int x, int y, bool removeAir = false)
    {
        if (!data.ContainsKey((x, y)))
            return false;
        Tile.Type type = data[(x, y)];
        if (type == Tile.Type.AIR && !removeAir)
            return false;
        data.Remove((x, y));
        Destroy(tiles[(x, y)]);
        tiles.Remove((x, y));
        if (type != Tile.Type.AIR)
        {
            tilemap.SetTile(new Vector3Int(x, y), null);
            (int, int)[] neighbors = GetNeighbors(x, y);
            (int, int)[] cornerNeighbors = GetCornerNeighbors(x, y);
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
            }
            for (int i = 0; i < neighbors.Length; i++)
            {
                if (data.ContainsKey(neighbors[i]))
                {
                    UpdateNeighbors(neighbors[i].Item1, neighbors[i].Item2);
                }
            }
            for (int i = 0; i < cornerNeighbors.Length; i++)
            {
                if (data.ContainsKey(cornerNeighbors[i]))
                {
                    UpdateNeighbors(cornerNeighbors[i].Item1, cornerNeighbors[i].Item2);
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

    public void CreateTile(int x, int y, Tile.Type type)
    {
        if (!TopLayer.ContainsKey(x))
            TopLayer[x] = y;
        if (TopLayer[x] < y)
            TopLayer[x] = y;
        if (tiles.ContainsKey((x, y)))
        {
            RemoveTile(x, y, true);
        }
        data[(x, y)] = type;
        GameObject tile = Instantiate(tilePrefab, new Vector2(x * Tile.WIDTH, y * Tile.HEIGHT), Quaternion.identity);
        Tile tileScript = tile.GetComponent<Tile>();
        tileScript.type = type;
        tileScript.Initialize();
        tiles[(x, y)] = tile;
        (int, int)[] neighbors = GetNeighbors(x, y);
        (int, int)[] cornerNeighbors = GetCornerNeighbors(x, y);
        if (type != Tile.Type.AIR)
        {
            tilemap.SetTile(new Vector3Int(x, y), blankTile);
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
            for (int i = 0; i < cornerNeighbors.Length; i++)
            {
                if (tiles.ContainsKey(cornerNeighbors[i]))
                {
                    UpdateNeighbors(cornerNeighbors[i].Item1, cornerNeighbors[i].Item2);
                }
            }
        }
        UpdateNeighbors(x, y);
    }

    public void RemoveEntity(Entity entity)
    {
        entities.Remove(entity.gameObject);
        Destroy(entity.gameObject);
    }

    public GameObject CreateEntity(float x, float y, Entity.Type type, int id = 0)
    {
        GameObject entity = Entity.Create(type, id);
        entity.transform.position = new Vector2(x, y);
        entities.Add(entity);
        return entity;
    }

    public Vector3Int GetTilePosition(Vector3 mousePos)
    {
        return tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(mousePos));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
