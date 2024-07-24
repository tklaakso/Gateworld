using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeEntity : Entity
{
    
    public TreeEntity() : base(Type.TREE)
    {
        SetSize(2.0f, 2.0f);
    }

}
