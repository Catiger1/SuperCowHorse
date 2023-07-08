using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : ObjectCanPlaced
{
    public override void Initialize()
    {
        
    }

    public override void Reset()
    {

    }
    public void TrampolineEvent()
    {
        GetComponent<Animator>().SetBool("Jump", false);
    }
    public override bool CallAfterPlaced(Collider2D[] result)
    {
        //transform.GetComponent<BoxCollider2D>().enabled = false;
        transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = true;
        transform.GetChild(0).GetChild(0).GetComponent<BoxCollider2D>().enabled = true;
        return false;
    }

    public override bool IsCanPlaced(Collider2D collider,ContactFilter2D contactFilter2D, Collider2D[] result)
    {
        return collider.OverlapCollider(contactFilter2D, result) == 0;
    }
}
