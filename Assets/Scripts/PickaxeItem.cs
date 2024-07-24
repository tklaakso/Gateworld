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
        Tile.Type tileType = Game.World.GetTile(tilePos.x, tilePos.y);
        if (Game.World.RemoveTile(tilePos.x, tilePos.y))
        {
            GameObject entity = Game.World.CreateEntity(tilePos.x + Tile.WIDTH / 2.0f - Game.EntityManager.ItemEntityPrefab.GetComponent<ItemEntity>().width / 2.0f, tilePos.y + Tile.HEIGHT / 2.0f - Game.EntityManager.ItemEntityPrefab.GetComponent<ItemEntity>().height / 2.0f, Entity.Type.ITEM);
            entity.GetComponent<ItemEntity>().SetItem(Create(tileType));
            return true;
        }
        return false;
    }

}
