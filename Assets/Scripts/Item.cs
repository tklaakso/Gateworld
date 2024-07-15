using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Item
{

    public static readonly int MAX_STACK = 64;

    public Type type;
    public int quantity;
    public bool disposable;

    public enum Type
    {
        NONE,
        GRASS,
        STONE,
        PICKAXE,
    }

    public Item(Type type, int quantity = 1)
    {
        this.type = type;
        this.quantity = quantity;
    }

    private static Item CreateSingleItem(Type type)
    {
        switch (type)
        {
            case Type.NONE:
                return new EmptyItem();
            case Type.GRASS:
                return new TileItem(Type.GRASS, Tile.Type.GRASS);
            case Type.STONE:
                return new TileItem(Type.STONE, Tile.Type.STONE);
            case Type.PICKAXE:
                return new PickaxeItem(Type.PICKAXE);
            default:
                return new EmptyItem();
        }
    }

    public static Item Create(Type type, int quantity = 1)
    {
        Item item = CreateSingleItem(type);
        item.quantity = quantity;
        return item;
    }

    public abstract bool Activate(Vector3 mousePosition);

}
