using Assets.Scripts.Common;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class NetCursor : NetworkBehaviour
{
    // Update is called once per frame
    Vector2 newCursorPos;
    Vector2 offset;
    //检测是否可以选择
    RaycastHit2D hit;
    public LayerMask layer;
    public Transform curSelectTF;
    
    //检测是否可以放置
    Collider2D boxCollider;
    Collider2D[] result;
    ContactFilter2D contactFilter2D;
    ObjectCanPlaced objectCanPlaced;
    //防抖
    float delayTime = 0.2f;
    bool canPlace = false;

    private Transform GenerateGos;
    public GameObject localPlayer;
    public bool islocal = false;
    public bool active = false;
    private void Start()
    {
        offset = GetComponent<SpriteRenderer>().size/2;
        offset.x = -offset.x;
        result = new Collider2D[2];
        contactFilter2D = new ContactFilter2D();
    }
    [Command(requiresAuthority = false)]
    private void CmdUpdateCursorPosition(Vector2 position)
    {
        RpcUpdateCursorPosition(position);
    }

    [ClientRpc]
    private void RpcUpdateCursorPosition(Vector2 position)
    {
        transform.position = position;
    }

    public void SetLocalPlayer(GameObject go)
    {
        localPlayer = go;
        islocal = localPlayer.GetComponent<NetworkIdentity>().isLocalPlayer;
        if(islocal)
            WindowsManager.Instance.GetWindow<PlacementWindow>(WindowsType.PlacementWindow).SetNetCursor(this);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetActive(bool flag)
    {
        RpcSetActive(flag);
    }
    //[ClientRpc]
    public void RpcSetActive(bool flag)
    {
        active = flag;
        if (active == false)
            CmdUpdateCursorPosition(new Vector2(1000, 1000));
    }

    void Update()
    {
        if (islocal&&active)
        {
            newCursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButtonDown(0))
            {
                if (curSelectTF == null)
                {
                    hit = Physics2D.Raycast(newCursorPos, Vector2.zero, 100f, layer);
                    if (hit.collider != null)
                    {
                        curSelectTF = hit.collider.transform;
                        boxCollider = hit.collider;
                        objectCanPlaced = boxCollider.GetComponent<ObjectCanPlaced>();
                        this.DelayCallBack(delayTime, () => { canPlace = true; });
                    }
                }
                else if (canPlace)
                {
                    //写成网络
                    if (objectCanPlaced.IsCanPlaced(boxCollider, contactFilter2D, result))
                    {
                        curSelectTF.SetParent(GenerateGos);
                        gameObject.SetActive(false);
                    }
                }
            }

            if (curSelectTF != null)
            {
                objectCanPlaced.ShowPosAfterPlaced(boxCollider, contactFilter2D, result);
                curSelectTF.position = newCursorPos;
            }

            CmdUpdateCursorPosition(newCursorPos - offset);
        }

    }

    private void OnDisable()
    {
        curSelectTF = null;
        canPlace = false;
        objectCanPlaced = null;
    }

}
