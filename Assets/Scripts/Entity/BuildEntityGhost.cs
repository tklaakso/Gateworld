
using UnityEngine;

public class BuildEntityGhost : Entity
{

    private BuildEntity.Type _buildType;

    public BuildEntity.Type buildType
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

    public BuildEntityGhost() : base(Type.BUILD_GHOST)
    {

    }

    public override Sprite GetSprite()
    {
        return Game.SpriteManager.GetBuildByID((int)buildType);
    }

}
