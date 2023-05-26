using UnityEngine;

namespace Mirror.Examples.NetworkRoom
{
    [AddComponentMenu("")]
    public class NetworkRoomPlayerExt : NetworkRoomPlayer
    {
        public RoomPlayerSelector RoomCharacterSelector { get; set; }
        [SyncVar(hook = nameof(CmdSetSeletCharacter))]
        public int CurSelectCharacterIndex = -1;
        public Animator anim;
        
        [ClientRpc(includeOwner = true)]
        public void ChangeSeletCharacter(int newIndex)
        {
            GameObject selectBtnGo = GameObject.FindGameObjectWithTag("SelectButton");
            RoomPlayerSelector selectBtn = selectBtnGo.GetComponent<RoomPlayerSelector>();
            selectBtn?.EnableBtn(CurSelectCharacterIndex);
            selectBtn?.DisableBtn(newIndex);
            CurSelectCharacterIndex = newIndex;
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
        }

        [Command(requiresAuthority = false)]
        public void CmdChangeSeletCharacter(int newIndex)
        {
            ChangeSeletCharacter(newIndex);
        }

        [Command(requiresAuthority = false)]
        public void CmdSetSeletCharacter(int oldIndex, int newIndex)
        {
            ChangeSeletCharacter(newIndex);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
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
        }
        private void OnDestroy()
        {
            if (CurSelectCharacterIndex != -1)
            {
                GameObject selectBtn = GameObject.FindGameObjectWithTag("SelectButton");
                selectBtn?.GetComponent<RoomPlayerSelector>().Clear(CurSelectCharacterIndex);
            }
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
