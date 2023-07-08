using Assets.Scripts.StateMachine;
using Mirror;
using Mirror.Examples.NetworkRoom;
using Mirror.Examples.NetworkRoomExt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : ObjectCanPlaced
{
    public override void Initialize()
    {
        throw new System.NotImplementedException();
    }

    public override bool IsCanPlaced(Collider2D collider, ContactFilter2D contactFilter2D, Collider2D[] result)
    {
        return collider.OverlapCollider(contactFilter2D, result) == 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
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
    public override void Reset()
    {
        throw new System.NotImplementedException();
    }
}
