using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EleSpikeweed : ObjectCanPlaced
{
    public override void Initialize()
    {
        
    }

    public override bool IsCanPlaced(Collider2D collider, ContactFilter2D contactFilter2D, Collider2D[] result)
    {
        bool flag = collider.OverlapCollider(contactFilter2D, result) == 0;
        Vector2 rayFirePos = new Vector2(collider.bounds.center.x, collider.bounds.center.y- collider.bounds.size.y/2);
        RaycastHit2D hit = Physics2D.Raycast(rayFirePos, Vector2.down, RayLength, CanPlacedLayer);
        flag &= hit;
        if (flag) PlacedPosAdjust(hit.point,collider);
        return flag;
    }

    public override void Reset()
    {
       
    }
}
