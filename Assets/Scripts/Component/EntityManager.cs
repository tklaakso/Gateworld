using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EntityManager : MonoBehaviour
{

    public GameObject ItemEntityPrefab;
    public GameObject TreeEntityPrefab;
    public GameObject FloorBuildEntityPrefab;
    public GameObject WallBuildEntityPrefab;
    public GameObject CeilingBuildEntityPrefab;

    public void Initialize()
    {

    }

    public Vector2 FindPlayerSpawn()
    {
        Dictionary<int, int> topLayer = World.TopLayer;
        System.Random rand = new System.Random();
        KeyValuePair<int, int> item = topLayer.ElementAt(rand.Next(0, topLayer.Count));
        return new Vector2(item.Key + Tile.WIDTH / 2.0f, item.Value + Player.HEIGHT / 2.0f);
    }

    public static Vector2 GetSpawnPoint(int x, int y)
    {
        return new Vector2(x + Tile.WIDTH / 2.0f, y + 1);
    }

}
