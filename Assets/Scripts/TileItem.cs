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

    public override void Activate(Vector3 mousePosition)
    {
        Vector3Int tilePos = World.GetTilePosition(mousePosition);
        World.CreateTile(tilePos.x, tilePos.y, tileType);
    }

}
