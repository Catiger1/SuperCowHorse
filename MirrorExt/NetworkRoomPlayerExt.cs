using Assets.Scripts.Common;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.NetworkRoom
{
    [AddComponentMenu("")]
    public class NetworkRoomPlayerExt : NetworkRoomPlayer
    {
        public RoomPlayerSelector RoomCharacterSelector { get; set; }
        [SyncVar(hook = nameof(CmdSetSeletCharacter))]
        public int CurSelectCharacterIndex = -1;
        public Animator anim;
        Transform btnRoomReadyTF;
        [ClientRpc(includeOwner = true)]
        public void ChangeSeletCharacter(int newIndex)
        {
            if (readyToBegin)
                return;
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
            Transform roomCanvas = GameObject.FindGameObjectWithTag("RoomCanvas").transform;
            Transform roomTf = roomCanvas.FindChildByName("SelectSlot");
            transform.position = roomTf.GetChild(index).position;
            //ChangeReadyStateUI(readyToBegin);
            //设置准备按钮等
            if(NetworkClient.active&&isLocalPlayer)
            {
                btnRoomReadyTF = roomCanvas.FindChildByName("BtnRoomReady");
                Button btnRoomReady = btnRoomReadyTF.GetComponent<Button>();
                btnRoomReady.onClick.RemoveAllListeners();
                btnRoomReady.onClick.AddListener(() => {
                    if (CurSelectCharacterIndex < 0) return;
                    CmdChangeReadyState(!readyToBegin);
                });
            }
            //Reset Select Button
            this.DelayCallBack(1, () => {
                if (CurSelectCharacterIndex != -1)
                {
                    GameObject selectBtn = GameObject.FindGameObjectWithTag("SelectButton");
                    RoomPlayerSelector selector = selectBtn?.GetComponent<RoomPlayerSelector>();
                    selector?.DisableBtn(CurSelectCharacterIndex);
                }
                //Exit Button
                Button btnRoomExit = roomCanvas.FindChildByName("BtnRoomExit").GetComponent<Button>();
                btnRoomExit?.onClick.RemoveAllListeners();
                btnRoomExit?.onClick.AddListener(() => {
                    if ((NetworkServer.active && NetworkClient.isConnected))
                        NetworkRoomManagerExt.singleton.StopHost();
                    else if (NetworkClient.isConnected)
                        NetworkRoomManagerExt.singleton.StopClient();
                    else { }
                });
            });
        }

        public override void OnGUI()
        {
            if (isLocalPlayer&&readyToBegin && btnRoomReadyTF)
                btnRoomReadyTF.GetComponentInChildren<Text>().text = "Cancel";
            else if(btnRoomReadyTF)
                btnRoomReadyTF.GetComponentInChildren<Text>().text = "Ready";
            base.OnGUI();
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
            if (newReadyState)
                transform.Find("Tick").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            else
                transform.Find("Tick").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }
    }
}
