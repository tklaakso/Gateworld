using System.CodeDom.Compiler;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class TerrainGenerator
{

    public Dictionary<(int, int), Tile.Type> Data { get; }
    public List<((float, float), Entity.Type)> Entities { get; }

    private int seed;

    public TerrainGenerator()
    {
        System.Random random = new System.Random();
        seed = random.Next(100000);
        Data = new Dictionary<(int, int), Tile.Type>();
        Entities = new List<((float, float), Entity.Type)>();
        Generate();
    }

    private Dictionary<(int, int), Tile.Type> GenerateIsland((int, int) center, int width, int height)
    {
        Random.InitState(seed + center.Item1 * 17 + center.Item2 * 41);
        Dictionary<(int, int), Tile.Type> islandData = new Dictionary<(int, int), Tile.Type>();
        for (int i = center.Item1 - width; i <= center.Item1 + width; i++)
        {
            float edgeDistance = 2 * Mathf.Min(Mathf.Abs(i - (center.Item1 - width)), Mathf.Abs(i - (center.Item1 + width))) / (float)width;
            float edgeSmoothing = Mathf.Sqrt(edgeDistance);
            int noise = (int)((Mathf.PerlinNoise1D(seed + i / 10.0f) + 0.4f) * height * edgeSmoothing);
            for (int j = center.Item2; j <= center.Item2 + noise; j++)
            {
                if (j < center.Item2 + noise)
                {
                    islandData[(i, j)] = Tile.Type.DIRT;
                }
                else
                {
                    islandData[(i, j)] = Tile.Type.GRASS;
                }
            }
            int rockDepth = center.Item2 - (int)((Random.value + 3) * edgeDistance * (height / 2));
            int rockStart = center.Item2 - (int)(Random.value * 3);
            for (int j = rockDepth; j < rockStart; j++)
            {
                islandData[(i, j)] = Tile.Type.STONE;
            }
            for (int j = rockStart; j < center.Item2; j++)
            {
                islandData[(i, j)] = Tile.Type.DIRT;
            }
        }
        return islandData;
    }

    private List<((float, float), Entity.Type)> DecorateIsland(Dictionary<(int, int), Tile.Type> islandData)
    {
        List<((float, float), Entity.Type)> entityData = new List<((float, float), Entity.Type)>();
        SortedDictionary<int, int> topLayer = new SortedDictionary<int, int>();
        foreach ((int, int) key in islandData.Keys)
        {
            if (!topLayer.ContainsKey(key.Item1) || topLayer[key.Item1] < key.Item2)
            {
                topLayer[key.Item1] = key.Item2;
            }
        }
        foreach (int x in topLayer.Keys)
        {
            int y = topLayer[x];
            Vector2 spawn = EntityManager.GetSpawnPoint(x, y);
            if (Random.value > 0.9f)
                entityData.Add(((spawn.x, spawn.y), Entity.Type.TREE));
        }
        return entityData;
    }

    private void Generate()
    {
        Dictionary<(int, int), Tile.Type> islandData = GenerateIsland((0, 0), 100, 10);
        List<((float, float), Entity.Type)> entityData = DecorateIsland(islandData);
        Data.AddRange(islandData);
        Entities.AddRange(entityData);
    }

}
