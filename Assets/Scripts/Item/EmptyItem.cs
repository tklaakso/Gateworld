using UnityEngine;
using UnityEngine.EventSystems;

public class EmptyItem : Item
{

    public EmptyItem() : base(Item.Type.NONE)
    {
        
    }

    public override bool Activate(Vector3 mousePosition)
    {
        return false;
    }

}
