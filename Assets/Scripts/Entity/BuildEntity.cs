using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildEntity : Entity
{

    private Type _buildType;

    public Type buildType
    {
        get
        {
            return _buildType;
        }

        set
        {
            _buildType = value;
            UpdateSprite();
        }
    }

    public new enum Type
    {
        WOOD_FLOOR,
        WOOD_WALL,
        WOOD_CEILING,
        STONE_FLOOR,
        STONE_WALL,
        STONE_CEILING,
    }

    public BuildEntity() : base(Entity.Type.BUILD)
    {

    }

    public static GameObject Create(Type type)
    {
        GameObject build = null;
        switch (type)
        {
            case Type.WOOD_FLOOR:
            case Type.STONE_FLOOR:
                build = Instantiate(Game.EntityManager.FloorBuildEntityPrefab);
                break;
            case Type.WOOD_WALL:
            case Type.STONE_WALL:
                build = Instantiate(Game.EntityManager.WallBuildEntityPrefab);
                break;
            case Type.WOOD_CEILING:
            case Type.STONE_CEILING:
                build = Instantiate(Game.EntityManager.CeilingBuildEntityPrefab);
                break;
        }
        build.GetComponent<BuildEntity>().buildType = type;
        return build;
    }

    public override Sprite GetSprite()
    {
        return Game.SpriteManager.GetBuildByID((int)buildType);
    }

}
