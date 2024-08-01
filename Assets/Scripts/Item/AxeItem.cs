using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AxeItem : Item
{

    public AxeItem() : base(Type.AXE)
    {
        MaxStackSize = 1;
    }

    public override bool Activate(Vector3 mousePosition)
    {
        RaycastHit2D[] results = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero);
        for (int i = 0; i < results.Length; i++)
        {
            TreeEntity entity = results[i].collider.gameObject.GetComponent<TreeEntity>();
            if (entity != null)
            {
                Vector2 treePos = entity.transform.position;
                Game.World.RemoveEntity(entity);
                GameObject itemEntity = Game.World.CreateEntity(treePos.x, treePos.y, Entity.Type.ITEM);
                itemEntity.GetComponent<ItemEntity>().item = Create(Type.WOOD, 8);
                Util.ApplyItemEntityDispersalForce(itemEntity);
                return true;
            }
        }
        return false;
    }

}
