using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HammerItem : Item
{

    public HammerItem() : base(Type.HAMMER)
    {
        MaxStackSize = 1;
    }

    public override bool Activate(Vector3 mousePosition)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePosition);
        Game.World.CreateEntity(worldPos.x, worldPos.y, Entity.Type.BUILD, (int)BuildEntity.Type.WOOD_FLOOR);
        return true;
    }

}
