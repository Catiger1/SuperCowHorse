using UnityEngine;

namespace Mirror.Examples.NetworkRoom
{
    [AddComponentMenu("")]
    public class NetworkRoomPlayerExt : NetworkRoomPlayer
    {
        //private Transform roomCharacterSlot;
        public RoomPlayerSelector RoomCharacterSelector { get; set; }
        [SyncVar(hook = nameof(CmdChangeSeletCharacter))]
        public int CurSelectCharacterIndex = -1;
        public Animator anim;
        //[ClientRpc(includeOwner =true)]
        //public void ChangeSeletCharacter(int newIndex)
        //{
        //    if (CurSelectCharacterIndex != -1)
        //    {
        //        GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
        //        RuntimeAnimatorController animatorController = ResourcesManager.Load<RuntimeAnimatorController>("Animation/AnimCharacter/" + "Player" + newIndex.ToString());
        //        anim.runtimeAnimatorController = animatorController;
        //    }else
        //    {
        //        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        //    }
        //}
        [Command]
        public void CmdChangeSeletCharacter(int _, int newIndex)
        {
            if (CurSelectCharacterIndex != -1)
            {
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                RuntimeAnimatorController animatorController = ResourcesManager.Load<RuntimeAnimatorController>("Animation/AnimCharacter/" + "Player" + newIndex.ToString());
                anim.runtimeAnimatorController = animatorController;
            }
            else
            {
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            }
            //ChangeSeletCharacter(newIndex);
        }

        public override void OnStartClient()
        {
            Debug.Log($"OnStartClient {gameObject}");
        }
        public override void OnClientEnterRoom()
        {
            Debug.Log($"OnClientEnterRoom");
            Transform roomTf = GameObject.FindGameObjectWithTag("RoomSelectSlot").transform;
            transform.position = roomTf.GetChild(index).position;

            GameObject.FindGameObjectWithTag("SelectButton");
        }

        public override void OnClientExitRoom()
        {
            //Debug.Log($"OnClientExitRoom {SceneManager.GetActiveScene().path}");
        }

        public override void IndexChanged(int oldIndex, int newIndex)
        {
            //Debug.Log($"IndexChanged {newIndex}");
        }

        public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
        {
            //Debug.Log($"ReadyStateChanged {newReadyState}");
        }

        public override void OnGUI()
        {
            base.OnGUI();
        }
    }
}
