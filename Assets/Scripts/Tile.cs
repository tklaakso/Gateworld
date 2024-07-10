using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

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
        List<float[]> neighborCoords = new List<float[]>();
        float[] neighborExists = new float[neighbors.Length];
        for (int i = 0; i < neighbors.Length; i++)
        {
            
            if (neighbors[i] != null)
            {
                Sprite sprite = SpriteManager.GetTileByID((int)neighbors[i]);
                Texture texture = sprite.texture;
                neighborCoords.Add(new float[] {sprite.rect.x / texture.width,
                                                sprite.rect.y / texture.width,
                                                sprite.rect.width / texture.width,
                                                sprite.rect.height / texture.width});
                neighborExists[i] = 1.0f;
            }
            else
            {
                neighborCoords.Add(new float[] { 0, 0, 0, 0 });
                neighborExists[i] = 0.0f;
            }
        }
        Sprite mainSprite = SpriteManager.GetTileByID((int)type);
        Texture mainTexture = mainSprite.texture;
        mat.SetFloatArray("_MainCoords", new float[]
        {
            mainSprite.rect.x / mainTexture.width,
            mainSprite.rect.y / mainTexture.width,
            mainSprite.rect.width / mainTexture.width,
            mainSprite.rect.height / mainTexture.width
        });
        mat.SetFloatArray("_LeftCoords", neighborCoords[0]);
        mat.SetFloatArray("_RightCoords", neighborCoords[1]);
        mat.SetFloatArray("_TopCoords", neighborCoords[2]);
        mat.SetFloatArray("_BottomCoords", neighborCoords[3]);
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
