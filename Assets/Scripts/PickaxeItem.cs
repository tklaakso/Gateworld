using UnityEngine;
using UnityEngine.EventSystems;

public class PickaxeItem : Item
{

    public PickaxeItem(Item.Type itemType) : base(itemType)
    {
        
    }

    public override void Activate(Vector3 mousePosition)
    {
        Vector3Int tilePos = World.GetTilePosition(mousePosition);
        World.RemoveTile(tilePos.x, tilePos.y);
    }

}
