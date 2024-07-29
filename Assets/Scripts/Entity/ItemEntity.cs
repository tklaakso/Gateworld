using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEntity : Entity
{

    private Item item;
    private long cooldownStartTime = 0;

    public int PickupCooldown;

    public ItemEntity() : base(Type.ITEM)
    {
        
    }

    public void ActivateCooldown()
    {
        cooldownStartTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }

    public void SetItem(Item item)
    {
        this.item = item;
        SetSprite(item.GetSprite());
    }

    public override void OnPlayerCollision()
    {
        if (cooldownStartTime + PickupCooldown > DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond)
            return;
        Item remainder = Game.InventoryManager.AddItem(item);
        if (remainder.type == Item.Type.NONE)
        {
            Game.World.RemoveEntity(this);
        }
        else
        {
            item.quantity = remainder.quantity;
        }
    }

}
