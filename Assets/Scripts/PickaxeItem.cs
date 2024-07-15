using UnityEngine;
using UnityEngine.EventSystems;

public class PickaxeItem : Item
{

    public PickaxeItem(Item.Type itemType) : base(itemType)
    {
        
    }

    public override bool Activate(Vector3 mousePosition)
    {
        Vector3Int tilePos = Game.World.GetTilePosition(mousePosition);
        return Game.World.RemoveTile(tilePos.x, tilePos.y);
    }

}
