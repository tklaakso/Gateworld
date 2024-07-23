using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{

    protected Type type;

    public enum Type
    {
        ITEM,
        TREE,
    }

    public Entity(Type type)
    {
        this.type = type;
    }

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = Game.SpriteManager.GetEntityByID((int)type);
    }

    public static GameObject Create(Type type)
    {
        switch (type)
        {
            case Type.ITEM:
                return Instantiate(Game.EntityManager.ItemEntityPrefab);
            case Type.TREE:
                return Instantiate(Game.EntityManager.TreeEntityPrefab);
            default:
                return null;
        }
    }

}
