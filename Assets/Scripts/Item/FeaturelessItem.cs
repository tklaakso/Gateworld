using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FeaturelessItem : Item
{

    public FeaturelessItem(Type type) : base(type)
    {

    }

    public override bool Activate(Vector3 mousePosition)
    {
        return false;
    }

}
