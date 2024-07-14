using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{

    private static SpriteManager instance;

    private Dictionary<int, Sprite> idToTileSprite;
    private Dictionary<int, Sprite> idToItemSprite;

    private void LoadTileSprites()
    {
        idToTileSprite = new Dictionary<int, Sprite>();
        Sprite[] tiles = Resources.LoadAll<Sprite>("Sprites/tiles");
        for (int i = 0; i < tiles.Length; i++)
        {
            idToTileSprite[i] = tiles[i];
        }
    }

    private void LoadItemSprites()
    {
        idToItemSprite = new Dictionary<int, Sprite>();
        Sprite[] items = Resources.LoadAll<Sprite>("Sprites/items");
        for (int i = 0; i < items.Length; i++)
        {
            idToItemSprite[i] = items[i];
        }
    }

    public static Sprite GetTileByID(int id)
    {
        if (instance.idToTileSprite.ContainsKey(id))
            return instance.idToTileSprite[id];
        return null;
    }

    public static Sprite GetItemByID(int id)
    {
        if (instance.idToItemSprite.ContainsKey(id))
            return instance.idToItemSprite[id];
        return null;
    }

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        LoadTileSprites();
        LoadItemSprites();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
