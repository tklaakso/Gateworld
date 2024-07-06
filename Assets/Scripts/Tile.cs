using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public static float WIDTH = 1.0f;
    public static float HEIGHT = 1.0f;

    public Type type;

    public enum Type
    {
        GRASS,
    }

    private SpriteRenderer spriteRenderer;

    public void initialize()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = SpriteManager.getByTileID((int)type);
        spriteRenderer.sprite = sprite;
        transform.localScale = new Vector2(WIDTH / sprite.bounds.size.x, HEIGHT / sprite.bounds.size.y);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
