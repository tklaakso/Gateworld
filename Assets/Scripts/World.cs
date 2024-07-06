using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

    public GameObject tilePrefab;

    private Dictionary<Tuple<int, int>, Tile.Type> data;

    // Start is called before the first frame update
    void Start()
    {
        data = new Dictionary<Tuple<int, int>, Tile.Type>();
        data[new Tuple<int, int>(0, 0)] = 0;
        data[new Tuple<int, int>(1, 1)] = 0;
        foreach (Tuple<int, int> key in data.Keys)
        {
            Tile.Type type = data[key];
            GameManager.createTile(key.Item1, key.Item2, type);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
