using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EleFan : ObjectCanPlaced
{
    public override void Initialize()
    {
        
    }

    public override void Reset()
    {

    }

    public override bool IsCanPlaced(Collider2D collider,ContactFilter2D contactFilter2D, Collider2D[] result)
    {
        return collider.OverlapCollider(contactFilter2D, result) == 0;
    }
}
