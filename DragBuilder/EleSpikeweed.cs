using Assets.Scripts.StateMachine;
using Mirror;
using Mirror.Examples.NetworkRoom;
using Mirror.Examples.NetworkRoomExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EleSpikeweed : ObjectCanPlaced
{
    public override void Initialize()
    {
        
    }

    [ServerCallback]
    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision Detected");
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerScore>().Death();
            NetworkRoomManagerExt.singleton.AliveCount--;
            if (NetworkRoomManagerExt.singleton.AliveCount <= 1)
                SendChangeStateMessage();
        }
    }

    [ServerCallback]
    public void SendChangeStateMessage()
    {
        NetworkServer.SendToAll(new StateMessage
        {
            newStateID = (int)GameStateID.Result
        });
    }

    public override bool IsCanPlaced(Collider2D collider, ContactFilter2D contactFilter2D, Collider2D[] result)
    {
        bool flag = collider.OverlapCollider(contactFilter2D, result) == 0;
        Vector2 rayFirePos = new Vector2(collider.bounds.center.x, collider.bounds.center.y- collider.bounds.size.y/2);
        RaycastHit2D hit = Physics2D.Raycast(rayFirePos, Vector2.down, RayLength, CanPlacedLayer);
        flag &= hit;
        return flag;
    }

    public override void Reset()
    {
       
    }
}
