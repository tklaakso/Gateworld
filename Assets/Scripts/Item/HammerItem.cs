using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class HammerItem : Item, ISlotSelectListener
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
        UpdateGhostBuildEntity(worldPos);
        GameObject hammerOverlay = Game.InventoryManager.hammerOverlay;
        Transform buildTypeSelector = hammerOverlay.transform.Find("BuildTypeSelector");
        int numBuildTypes = Enum.GetValues(typeof(BuildEntity.Type)).Length;
        for (int i = 0; i < numBuildTypes; i++)
        {
            SelectionSlot slot = buildTypeSelector.GetChild(i).GetComponent<SelectionSlot>();
            slot.SetSelectionID(i);
            slot.SetSprite(Game.SpriteManager.GetBuildByID(i));
        }
        Game.InventoryManager.SetHammerOverlayListener(this);
    }

    private void UpdateGhostBuildEntity(Vector3 pos)
    {
        if (ghostBuildEntity != null)
        {
            Game.Destroy(ghostBuildEntity);
        }
        ghostBuildEntity = Game.World.CreateEntity(pos.x, pos.y, Entity.Type.BUILD_GHOST);
        ghostBuildEntity.GetComponent<BuildEntityGhost>().buildType = buildType;
    }

    public override void OnDeselected(Vector3 mousePosition)
    {
        Game.World.RemoveEntity(ghostBuildEntity.GetComponent<Entity>());
        ghostBuildEntity = null;
        Game.InventoryManager.SetHammerOverlayListener(null);
    }

    public override void SelectedUpdate(Vector3 mousePosition)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 buildPos = GetBuildPosition(worldPos);
        ghostBuildEntity.transform.position = buildPos;
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Game.InventoryManager.SetHammerOverlayOpen(true);
        }
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
            case BuildEntity.Type.WOOD_WALL:
            case BuildEntity.Type.STONE_WALL:
                if (closestEntity is FloorBuildEntity)
                {
                    if (worldPos.x < closest.transform.position.x)
                    {
                        return new Vector2(closest.transform.position.x - closestEntity.width / 2, closest.transform.position.y + closestEntity.height);
                    }
                    else
                    {
                        return new Vector2(closest.transform.position.x + closestEntity.width / 2, closest.transform.position.y + closestEntity.height);
                    }
                }
                return worldPos;
            default:
                return worldPos;
        }
    }

    public void OnSlotSelected(int id)
    {
        Game.InventoryManager.SetHammerOverlayOpen(false);
        buildType = (BuildEntity.Type)id;
        UpdateGhostBuildEntity(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public override bool Activate(Vector3 mousePosition)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 buildPos = GetBuildPosition(worldPos);
        Game.BuildManager.CreateBuild(buildPos.x, buildPos.y, buildType);
        return true;
    }

}
