using Mirror;
using Mirror.Examples.NetworkRoom;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayerSelector : NetworkBehaviour
{
    public int CurSelectIndex
    {
        get { return curSelectIndex; }
        set { }
    }
    private int curSelectIndex = -1;
    private List<Button> btnList=new List<Button>();

    public NetworkRoomPlayerExt localRoomPlayer;
    private int local_SlotIndex = -1;
    public int Local_SlotIndex { get {
            return local_SlotIndex;
        } set { } }

    #region Start & Stop Callbacks
    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    public override void OnStartServer(){}

    [ClientRpc(includeOwner = true)]
    public void CmdChooseCharacter(int changeIndex)
    {
        if (changeIndex != -1)
            btnList[changeIndex].interactable = true;
    }
    [Command(requiresAuthority = false)]
    void SetBtnFunc(int changeIndex)
    {
        CmdChooseCharacter(changeIndex);
    }

    public void DisableBtn(int index)
    {
        if (index != -1)
            btnList[index].interactable = false;
    }

    public void EnableBtn(int index)
    {
        if (index != -1)
            btnList[index].interactable = true;
    }
    //[ClientRpc]
    //void Clear(int selectIndex)
    //{
    //    btnList[selectIndex].interactable = true;
    //}
    //[Command(requiresAuthority = false)]
    //public void ClearCmd(int selectIndex)
    //{
    //    Clear(selectIndex);
    //}

    //[ClientRpc]
    //public void SetSlotAnim(int index)
    //{

    //}
    //[Command(requiresAuthority = false)]
    //void SetPlayerSlotAnimCmd(int index)
    //{
    //    SetSlotAnim(index);
    //}

    public int GetCanUseCharacterIndex()
    {
        for(int i=0;i< btnList.Count;i++)
        {
            if (btnList[i].interactable) { return i; }
        }
        return -1;
    }

    /// <summary>
    /// Invoked on the server when the object is unspawned
    /// <para>Useful for saving object data in persistent storage</para>
    /// </summary>
    public override void OnStopServer() { }

    /// <summary>
    /// Called on every NetworkBehaviour when it is activated on a client.
    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
    /// </summary>
    public override void OnStartClient() { }

    /// <summary>
    /// This is invoked on clients when the server has caused this object to be destroyed.
    /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
    /// </summary>
    public override void OnStopClient() {

    }

    /// <summary>
    /// Called when the local player object has been set up.
    /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStartLocalPlayer()
    {

    }
    /// <summary>
    /// Called when the local player object is being stopped.
    /// <para>This happens before OnStopClient(), as it may be triggered by an ownership message from the server, or because the player object is being destroyed. This is an appropriate place to deactivate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStopLocalPlayer() {

            //ClearCmd(curSelectIndex, local_SlotIndex);
    }

    /// <summary>
    /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
    /// <para>This is called after <see cref="OnStartServer">OnStartServer</see> and before <see cref="OnStartClient">OnStartClient.</see></para>
    /// <para>When <see cref="NetworkIdentity.AssignClientAuthority">AssignClientAuthority</see> is called on the server, this will be called on the client that owns the object. When an object is spawned with <see cref="NetworkServer.Spawn">NetworkServer.Spawn</see> with a NetworkConnectionToClient parameter included, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStartAuthority() {

    }

    //public void InitBtn(NetworkRoomPlayerExt localPlayer)
    //{
    //    for (int i = 0; i < transform.childCount; i++)
    //    {
    //        Button btn = transform.GetChild(i).GetComponent<Button>();
    //        btn.onClick.AddListener(() => {
    //            int index = GetSelectorNameIndex(btn.name);
    //            int changeIndex = curSelectIndex;
    //            curSelectIndex = index;
    //            localPlayer.CmdChangeSeletCharacter(index);
    //            SetBtnFunc(index, changeIndex);
    //        });
    //        btnList.Add(btn);
    //    }
    //}
    public void Clear(int index)
    {
        btnList[index].interactable = true;
    }

    private void Start()
    {
        if (localRoomPlayer == null)
        {
            GameObject[] roomPlayers = GameObject.FindGameObjectsWithTag("RoomPlayer");
            for (local_SlotIndex = 0; local_SlotIndex < roomPlayers.Length; local_SlotIndex++)
            {
                if (roomPlayers[local_SlotIndex].GetComponent<NetworkRoomPlayerExt>().isLocalPlayer)
                {
                    localRoomPlayer = roomPlayers[local_SlotIndex].GetComponent<NetworkRoomPlayerExt>();
                    localRoomPlayer.RoomCharacterSelector = this;
                    break;
                }
            }
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            Button btn = transform.GetChild(i).GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                int index = GetSelectorNameIndex(btn.name);
                int changeIndex = curSelectIndex;
                curSelectIndex = index;
                localRoomPlayer.CmdChangeSeletCharacter(index);//localRoomPlayer.CurSelectCharacterIndex = index;
                //SetBtnFunc(changeIndex);
            });
            btnList.Add(btn);
        }
    }

    private int GetSelectorNameIndex(string str)
    {
        int index = 0;
        for(int i=str.Length-1;i>=0;i--)
        {
            if (str[i]>='0'&& str[i] <= '9')
            {
                index = index*10 + str[i] - '0';
            }
        }
        return index;
    }
    /// <summary>
    /// This is invoked on behaviours when authority is removed.
    /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStopAuthority() { }

    #endregion
}
