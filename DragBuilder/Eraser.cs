using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eraser : ObjectCanPlaced
{
    public override void Initialize()
    {
        throw new System.NotImplementedException();
    }

    public override bool IsCanPlaced(Collider2D collider, ContactFilter2D contactFilter2D, Collider2D[] result)
    {
        bool flag = false;
        collider.OverlapCollider(contactFilter2D, result);
        int i = 0;
        for(;i< result.Length;i++)
        {
            if (result[i]!=null&&result[i].GetComponent<ObjectCanPlaced>() != null)
            {
                flag = true; break;
            }
        }
        return flag;
    }

    public override bool CallAfterPlaced(Collider2D[] result)
    {
        for (int i = 0; i < result.Length; i++)
        {
            if (result[i].GetComponent<ObjectCanPlaced>() != null)
            {
                NetworkServer.Destroy(result[i].gameObject);
                NetworkServer.Destroy(gameObject);
                break;
            }
        }
        return true;
    }
    public override void ShowPosAfterPlaced(Collider2D collider, ContactFilter2D contactFilter2D, Collider2D[] result)
    {
        if (IsCanPlaced(collider, contactFilter2D, result))
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.black;
        }
    }
    public override void Reset()
    {
        throw new System.NotImplementedException();
    }
}
