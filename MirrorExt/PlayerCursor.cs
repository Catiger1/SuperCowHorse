using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCursor : NetworkBehaviour
{
    // Start is called before the first frame update
    public GameObject CursorGo;
    void Start()
    {
        if(isLocalPlayer)
            CmdSpawnCursor();
    }

    [Command(requiresAuthority = false)]
    public void CmdSpawnCursor()
    {
        GameObject go = ResourcesManager.Load<GameObject>("Prefab/Cursor");
        CursorGo = GameObject.Instantiate(go);
        NetworkServer.Spawn(CursorGo);
        RpcSetLocalPlayer(CursorGo);
    }
    [ClientRpc]
    public void RpcSetLocalPlayer(GameObject go)
    {
        go.GetComponent<NetCursor>().SetLocalPlayer(gameObject);
    }

    public void OnDestroy()
    {
        if (CursorGo != null)
           Destroy(CursorGo);
    }

    //private void FixedUpdate()
    //{
    //    if(CursorGo!=null&& !setFlag)
    //    {
            
    //        setFlag = true;
    //        Debug.Log("Set True");
    //    }
    //}
}
