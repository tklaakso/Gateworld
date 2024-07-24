using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        TILE,
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

    public static Item Create(Tile.Type type, int quantity = 1)
    {
        Item item = new TileItem(type);
        item.quantity = quantity;
        return item;
    }

    public static Item Create(Item item, int quantity = 1)
    {
        Item clone = item.Clone();
        clone.quantity = quantity;
        return clone;
    }

    public abstract bool Activate(Vector3 mousePosition);

    public virtual Item Clone()
    {
        Item clone = CreateSingleItem(type);
        clone.quantity = quantity;
        return clone;
    }

    public virtual Sprite GetSprite()
    {
        return Game.SpriteManager.GetItemByID((int)type);
    }

    public virtual bool Matches(Item other)
    {
        return type == other.type;
    }

}
