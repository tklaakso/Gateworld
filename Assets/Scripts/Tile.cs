using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public static readonly float WIDTH = 1.0f;
    public static readonly float HEIGHT = 1.0f;

    public Type type;

    public enum Type
    {
        AIR,
        GRASS,
        STONE,
    }

    private SpriteRenderer spriteRenderer;

    public void Initialize()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = SpriteManager.GetTileByID((int)type);
        spriteRenderer.sprite = sprite;
        transform.localScale = new Vector2(WIDTH / sprite.bounds.size.x, HEIGHT / sprite.bounds.size.y);
    }

    public void SetNeighbors(Tile.Type?[] neighbors)
    {
        Material mat = spriteRenderer.material;
        Texture[] neighborTextures = new Texture[neighbors.Length];
        float[] neighborExists = new float[neighbors.Length];
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] != null)
            {
                neighborTextures[i] = SpriteManager.GetTileByID((int)neighbors[i]).texture;
                neighborExists[i] = 1.0f;
            }
            else
            {
                neighborTextures[i] = null;
                neighborExists[i] = 0.0f;
            }
        }
        mat.SetTexture("_LeftTex", neighborTextures[0]);
        mat.SetTexture("_RightTex", neighborTextures[1]);
        mat.SetTexture("_TopTex", neighborTextures[2]);
        mat.SetTexture("_BottomTex", neighborTextures[3]);
        mat.SetFloatArray("_NeighborExists", neighborExists);
        mat.SetFloat("_IsAirTile", type == Type.AIR ? 1.0f : 0.0f);
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
