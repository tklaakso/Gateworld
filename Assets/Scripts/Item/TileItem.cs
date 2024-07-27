using UnityEngine;
using UnityEngine.EventSystems;

public class TileItem : Item
{

    private Tile.Type tileType;

    public TileItem(Tile.Type tileType) : base(Item.Type.TILE)
    {
        this.tileType = tileType;
        disposable = true;
    }

    public override bool Activate(Vector3 mousePosition)
    {
        Vector3Int tilePos = Game.World.GetTilePosition(mousePosition);
        if (Game.World.TileExists(tilePos.x, tilePos.y))
            return false;
        Game.World.CreateTile(tilePos.x, tilePos.y, tileType);
        return true;
    }

    public override Item Clone()
    {
        Item clone = new TileItem(tileType);
        clone.quantity = quantity;
        return clone;
    }

    public override Sprite GetSprite()
    {
        return Game.SpriteManager.GetTileByID((int)tileType);
    }

    public override bool Matches(Item other)
    {
        return base.Matches(other) && tileType == ((TileItem)other).tileType;
    }

}
