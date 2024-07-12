using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{

    public static readonly int MAX_STACK = 64;

    public Type type;
    public int quantity;

    public enum Type
    {
        NONE,
        GRASS,
        STONE
    }

    public Item(Type type, int quantity = 1)
    {
        this.type = type;
        this.quantity = quantity;
    }

}
