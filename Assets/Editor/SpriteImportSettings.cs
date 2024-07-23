using UnityEngine;
using UnityEditor;

public class SpriteImportSettings : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (assetPath.ToLower().Contains("entities"))
            return;

        TextureImporter textureImporter = (TextureImporter)assetImporter;

        // Check if the texture is a sprite
        if (textureImporter.textureType == TextureImporterType.Sprite)
        {
            Debug.Log("Processing texture: " + assetPath);
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            textureImporter.spritePixelsPerUnit = 32;
            textureImporter.maxTextureSize = 1024;
            textureImporter.filterMode = FilterMode.Point;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
        }
    }

    void OnPostprocessTexture(Texture2D texture)
    {
        if (assetPath.ToLower().Contains("entities"))
            return;

        TextureImporter textureImporter = (TextureImporter)assetImporter;

        // Only process sprites with Multiple mode
        if (textureImporter.textureType == TextureImporterType.Sprite && textureImporter.spriteImportMode == SpriteImportMode.Multiple)
        {
            int spriteWidth = 32;
            int spriteHeight = 32;
            int columns = texture.width / spriteWidth;
            int rows = texture.height / spriteHeight;

            // Create a list to hold non-blank sprite metadata
            var spriteMetaDataList = new System.Collections.Generic.List<SpriteMetaData>();

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    if (!IsRegionBlank(texture, x * spriteWidth, texture.height - (y + 1) * spriteHeight, spriteWidth, spriteHeight))
                    {
                        SpriteMetaData smd = new SpriteMetaData
                        {
                            name = "Sprite_" + (y * columns + x),
                            rect = new Rect(x * spriteWidth, texture.height - (y + 1) * spriteHeight, spriteWidth, spriteHeight),
                            pivot = new Vector2(0.5f, 0.5f),
                            alignment = (int)SpriteAlignment.BottomLeft
                        };
                        spriteMetaDataList.Add(smd);
                    }
                }
            }

            // Assign the non-blank spritesheet metadata
            textureImporter.spritesheet = spriteMetaDataList.ToArray();
            EditorUtility.SetDirty(textureImporter);
            AssetDatabase.WriteImportSettingsIfDirty(textureImporter.assetPath);
            textureImporter.SaveAndReimport();

            Debug.Log("Sprite sheet created for texture: " + assetPath);
        }
    }

    bool IsRegionBlank(Texture2D texture, int x, int y, int width, int height)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (texture.GetPixel(x + i, y + j).a > 0)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
