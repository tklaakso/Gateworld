using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public abstract class Entity : MonoBehaviour
{

    private Type _type;

    public Type type {
        get
        {
            return _type;
        }

        set
        {
            _type = value;
            UpdateSprite();
        }
    }

    public float width = 1.0f, height = 1.0f;

    public enum Type
    {
        BUILD,
        ITEM,
        TREE,
    }

    public Entity(Type type)
    {
        _type = type;
    }

    protected void UpdateSprite()
    {
        Sprite sprite = GetSprite();
        GetComponent<SpriteRenderer>().sprite = sprite;
        transform.localScale = new Vector2(width / sprite.bounds.size.x, height / sprite.bounds.size.y);
    }

    void Start()
    {
        UpdateSprite();
    }

    protected void SetSize(float width, float height)
    {
        this.width = width;
        this.height = height;
    }

    public virtual Sprite GetSprite()
    {
        return Game.SpriteManager.GetEntityByID((int)type);
    }

    public static GameObject Create(Type type, int id = 0)
    {
        switch (type)
        {
            case Type.ITEM:
                return Instantiate(Game.EntityManager.ItemEntityPrefab);
            case Type.TREE:
                return Instantiate(Game.EntityManager.TreeEntityPrefab);
            case Type.BUILD:
                return BuildEntity.Create((BuildEntity.Type)id);
            default:
                return null;
        }
    }

    public virtual void OnPlayerCollision()
    {

    }

}
