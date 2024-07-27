using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public abstract class Entity : MonoBehaviour
{

    protected Type type;

    public float width = 1.0f, height = 1.0f;

    private Sprite sprite = null;

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
        if (sprite == null)
            sprite = Game.SpriteManager.GetEntityByID((int)type);
        GetComponent<SpriteRenderer>().sprite = sprite;
        transform.localScale = new Vector2(width / sprite.bounds.size.x, height / sprite.bounds.size.y);
    }

    protected void SetSize(float width, float height)
    {
        this.width = width;
        this.height = height;
    }

    protected void SetSprite(Sprite sprite)
    {
        this.sprite = sprite;
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

    public virtual void OnPlayerCollision()
    {

    }

}
