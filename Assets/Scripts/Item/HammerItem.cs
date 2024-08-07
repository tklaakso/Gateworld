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
        Vector2 buildPos = GetBuildPosition(worldPos);
        ghostBuildEntity.transform.position = buildPos;
    }

    private Vector2 GetBuildPosition(Vector2 worldPos)
    {
        List<GameObject> neighbors = Game.BuildManager.GetNeighbors(worldPos.x, worldPos.y);
        GameObject closest = null;
        for (int i = 0; i < neighbors.Count; i++)
        {
            if (closest == null || (neighbors[i].transform.position - new Vector3(worldPos.x, worldPos.y, 0)).magnitude < (closest.transform.position - new Vector3(worldPos.x, worldPos.y, 0)).magnitude)
            {
                closest = neighbors[i];
            }
        }
        if (closest == null)
            return worldPos;
        Entity closestEntity = closest.GetComponent<Entity>();
        Vector2 diff = (worldPos - new Vector2(closest.transform.position.x, closest.transform.position.y)).normalized;
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            diff.y = 0;
        }
        else
        {
            diff.x = 0;
        }
        Vector2 side = diff.normalized;
        switch (buildType)
        {
            case BuildEntity.Type.WOOD_FLOOR:
            case BuildEntity.Type.STONE_FLOOR:
                return new Vector2(closest.transform.position.x + side.x * closestEntity.width, closest.transform.position.y + side.y * closestEntity.height);
            default:
                return worldPos;
        }
    }

    public override bool Activate(Vector3 mousePosition)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 buildPos = GetBuildPosition(worldPos);
        Game.BuildManager.CreateBuild(buildPos.x, buildPos.y, BuildEntity.Type.WOOD_FLOOR);
        return true;
    }

}
