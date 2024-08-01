using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEntity : Entity
{

    private Item _item;

    public Item item
    {
        get
        {
            return _item;
        }

        set
        {
            _item = value;
            UpdateSprite();
        }
    }

    private long cooldownStartTime = 0;

    public int PickupCooldown;

    public ItemEntity() : base(Type.ITEM)
    {
        
    }

    public void ActivateCooldown()
    {
        cooldownStartTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }

    public override Sprite GetSprite()
    {
        if (item == null)
            return null;
        return item.GetSprite();
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
