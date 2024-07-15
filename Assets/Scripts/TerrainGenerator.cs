using System.CodeDom.Compiler;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class TerrainGenerator
{

    public Dictionary<(int, int), Tile.Type> Data { get; }

    private int seed;

    public TerrainGenerator()
    {
        System.Random random = new System.Random();
        seed = random.Next(100000);
        Data = new Dictionary<(int, int), Tile.Type>();
        Generate();
    }

    private void GenerateIsland((int, int) center, int width, int height)
    {
        Random.InitState(seed + center.Item1 * 17 + center.Item2 * 41);
        for (int i = center.Item1 - width; i <= center.Item1 + width; i++)
        {
            float edgeDistance = 2 * Mathf.Min(Mathf.Abs(i - (center.Item1 - width)), Mathf.Abs(i - (center.Item1 + width))) / (float)width;
            float edgeSmoothing = Mathf.Sqrt(edgeDistance);
            int noise = (int)((Mathf.PerlinNoise1D(seed + i / 10.0f) + 0.4f) * height * edgeSmoothing);
            for (int j = center.Item2; j <= center.Item2 + noise; j++)
            {
                if (j < center.Item2 + noise)
                {
                    Data[(i, j)] = Tile.Type.DIRT;
                }
                else
                {
                    Data[(i, j)] = Tile.Type.GRASS;
                }
            }
            int rockDepth = center.Item2 - (int)((Random.value + 3) * edgeDistance * (height / 2));
            int rockStart = center.Item2 - (int)(Random.value * 3);
            for (int j = rockDepth; j < rockStart; j++)
            {
                Data[(i, j)] = Tile.Type.STONE;
            }
            for (int j = rockStart; j < center.Item2; j++)
            {
                Data[(i, j)] = Tile.Type.DIRT;
            }
        }
    }

    private void Generate()
    {
        GenerateIsland((0, 0), 100, 10);
    }

}
