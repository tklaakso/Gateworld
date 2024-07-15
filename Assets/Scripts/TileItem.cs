using UnityEngine;
using UnityEngine.EventSystems;

public class TileItem : Item
{

    private Tile.Type tileType;

    public TileItem(Item.Type itemType, Tile.Type tileType) : base(itemType)
    {
        this.tileType = tileType;
        disposable = true;
    }

    public override bool Activate(Vector3 mousePosition)
    {
        Vector3Int tilePos = World.GetTilePosition(mousePosition);
        if (World.TileExists(tilePos.x, tilePos.y))
            return false;
        World.CreateTile(tilePos.x, tilePos.y, tileType);
        return true;
    }

}
