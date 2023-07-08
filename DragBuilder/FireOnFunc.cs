
using Assets.Scripts.StateMachine;
using Mirror.Examples.NetworkRoom;
using Mirror;
using UnityEngine;
using Mirror.Examples.NetworkRoomExt;

namespace Assets.Scripts.DragBuilder
{
    internal class FireOnFunc: MonoBehaviour
    {
        bool fireSwitch = false;
        public Animator anim;
        [ServerCallback]
        public void OnTriggerEnter2D(Collider2D collision)
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

        void FixedUpdate()
        {
            if(Time.time%5==0)
            {
                fireSwitch = !fireSwitch;
                anim.SetBool("ON", fireSwitch);
                GetComponent<BoxCollider2D>().enabled = fireSwitch;
            }
        }
    }
}
