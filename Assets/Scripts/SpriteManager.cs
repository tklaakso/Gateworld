using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{

    private static SpriteManager instance;

    private Dictionary<int, Sprite> idToTileSprite;

    private void LoadTileSprites()
    {
        idToTileSprite = new Dictionary<int, Sprite>();
        Sprite[] tiles = Resources.LoadAll<Sprite>("Tile");
        for (int i = 0; i < tiles.Length; i++)
        {
            try
            {
                int id = int.Parse(tiles[i].name);
                idToTileSprite[id] = tiles[i];
            }
            catch
            {
                Debug.LogError("Unable to read id from tile with name: " + tiles[i].name);
            }
        }
    }

    public static Sprite GetTileByID(int id)
    {
        if (instance.idToTileSprite.ContainsKey(id))
            return instance.idToTileSprite[id];
        return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        LoadTileSprites();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
