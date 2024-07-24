using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEntity : Entity
{

    private Item item;

    public ItemEntity() : base(Type.ITEM)
    {
        
    }

    public void SetItem(Item item)
    {
        this.item = item;
        SetSprite(item.GetSprite());
    }

    public override void OnPlayerCollision()
    {
        Game.InventoryManager.AddItem(item);
        Game.World.RemoveEntity(this);
    }

}
