using Assets.Scripts.Common;
using Mirror;
using Mirror.Examples.NetworkRoomExt;
using UnityEngine;

public class PlayerCursor : NetworkBehaviour
{
    // Start is called before the first frame update
    public GameObject CursorGo;
    void Start()
    {
        if(isLocalPlayer)
            CmdSpawnCursor();
        //ChangeCursorColor(GetComponent<PlayerScore>().index);
    }

    //public void ChangeCursorColor(int index)
    //{
    //    if(index>0)
    //        CursorGo.GetComponent<SpriteRenderer>().color = Color.red;
    //    else if(index>1)
    //        CursorGo.GetComponent<SpriteRenderer>().color = Color.yellow;
    //    else if(index>2)
    //        CursorGo.GetComponent<SpriteRenderer>().color = Color.black;
    //}

    [Command(requiresAuthority = false)]
    public void CmdSpawnCursor()
    {
        GameObject go = ResourcesManager.Load<GameObject>("Prefab/Cursor");
        CursorGo = GameObject.Instantiate(go);
        Debug.Log("CursorGo = "+CursorGo);
        NetworkServer.Spawn(CursorGo);
        RpcSetLocalPlayer(CursorGo);

    }
    [ClientRpc]
    public void RpcSetLocalPlayer(GameObject go)
    {
        go.GetComponent<NetCursor>().SetLocalPlayer(gameObject);
    }

    //public void OnDestroy()
    //{
    //    if (CursorGo != null)
    //       Destroy(CursorGo);
    //}
}
