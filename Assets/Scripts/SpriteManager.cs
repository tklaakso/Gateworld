using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{

    private Dictionary<int, Sprite> idToTileSprite;
    private Dictionary<int, Sprite> idToItemSprite;
    private Dictionary<int, Sprite> idToEntitySprite;

    public void Initialize()
    {
        LoadTileSprites();
        LoadItemSprites();
        LoadEntitySprites();
    }

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

    private void LoadEntitySprites()
    {
        idToEntitySprite = new Dictionary<int, Sprite>();
        Sprite[] items = Resources.LoadAll<Sprite>("Sprites/entities");
        for (int i = 0; i < items.Length; i++)
        {
            idToEntitySprite[i] = items[i];
        }
    }

    public Sprite GetTileByID(int id)
    {
        if (idToTileSprite.ContainsKey(id))
            return idToTileSprite[id];
        return null;
    }

    public Sprite GetItemByID(int id)
    {
        if (idToItemSprite.ContainsKey(id))
            return idToItemSprite[id];
        return null;
    }

    public Sprite GetEntityByID(int id)
    {
        if (idToEntitySprite.ContainsKey(id))
            return idToEntitySprite[id];
        return null;
    }

}
