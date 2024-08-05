using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HammerItem : Item
{

    private GameObject ghostBuildEntity;
    private BuildEntity.Type buildType = BuildEntity.Type.WOOD_FLOOR;

    public HammerItem() : base(Type.HAMMER)
    {
        MaxStackSize = 1;
    }

    public override void OnSelected(Vector3 mousePosition)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        ghostBuildEntity = Game.World.CreateEntity(worldPos.x, worldPos.y, Entity.Type.BUILD_GHOST);
        ghostBuildEntity.GetComponent<BuildEntityGhost>().buildType = buildType;
    }

    public override void OnDeselected(Vector3 mousePosition)
    {
        Game.World.RemoveEntity(ghostBuildEntity.GetComponent<Entity>());
        ghostBuildEntity = null;
    }

    public override void SelectedUpdate(Vector3 mousePosition)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        ghostBuildEntity.transform.position = worldPos;
    }

    public override bool Activate(Vector3 mousePosition)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        Game.World.CreateEntity(worldPos.x, worldPos.y, Entity.Type.BUILD, (int)BuildEntity.Type.WOOD_FLOOR);
        return true;
    }

}
