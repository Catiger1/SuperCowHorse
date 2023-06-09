using Assets.Scripts.Common;
using Assets.Scripts.StateMachine;
using Mirror;
using Mirror.Examples.NetworkRoomExt;
using Mirror.Examples.NetworkRoom;
using UnityEngine;
using System;
using UnityEngine.SocialPlatforms;

public class NetCursor : NetworkBehaviour
{
    // Update is called once per frame
    Vector2 newCursorPos;
    Vector2 offset;
    //����Ƿ����ѡ��
    RaycastHit2D hit;
    public LayerMask layer;
    public Transform curSelectTF;

    //����Ƿ���Է���
    Collider2D boxCollider;
    Collider2D[] result;
    ContactFilter2D contactFilter2D;
    ObjectCanPlaced objectCanPlaced;
    //����
    float delayTime = 0.2f;
    public bool CanPlace = false;

    private Transform GenerateGos;
    public GameObject localPlayer;

    public SpriteRenderer spriteRenderer;

    public bool islocal = false;
    //[SyncVar(hook = nameof(CmdSetPos))]
    public bool Active = false;

    //Camera Bound
    PolygonCollider2D CameraBound;
    private void Start()
    {
        offset = GetComponent<SpriteRenderer>().size / 2;
        offset.x = -offset.x;
        result = new Collider2D[2];
        contactFilter2D = new ContactFilter2D();
        CameraBound = GameObject.FindWithTag("CameraBound").GetComponent<PolygonCollider2D>();
        CmdUpdateCursorPosition(new Vector2(1000, 1000));
    }

    [ClientRpc]
    void RpcSetSpriteRendererColor(int index)
    {
        if (index > 0)
            spriteRenderer.color = Color.red;
        else if (index > 1)
            spriteRenderer.color = Color.yellow;
        else if (index > 2)
            spriteRenderer.color = Color.black;
    }
    [Command(requiresAuthority = false)]
    public void UpdateSpriteColor(int index)
    {
        RpcSetSpriteRendererColor(index);
    }

    [Command(requiresAuthority = false)]
    private void CmdUpdateCursorPosition(Vector2 position)
    {
        transform.position = position;
    }

    public void SetLocalPlayer(GameObject go)
    {
        localPlayer = go;
        islocal = localPlayer.GetComponent<NetworkIdentity>().isLocalPlayer;
        if (islocal)
        {
            PlacementWindow placementWindow = WindowsManager.Instance.GetWindow<PlacementWindow>(WindowsType.PlacementWindow);
            Debug.Log("Placement="+ placementWindow);
            placementWindow.SetNetCursor(this);
            UpdateSpriteColor(localPlayer.GetComponent<PlayerScore>().index);
        }
        //go.GetComponent<PlayerCursor>().ChangeCursorColor(go.GetComponent<PlayerScore>().index);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetPos(bool _, bool flag)
    {
        if (flag == false)
            CmdUpdateCursorPosition(new Vector2(1000, 1000));
    }

    [Command(requiresAuthority = false)]
    public void CmdSelectTF(Transform tf)
    {
        tf.SetParent(GenerateGos);
    }

    [ServerCallback]
    public void NetSendChangeSetTrapStateMessage()
    {
        NetworkServer.SendToAll(new StateMessage
        {
            newStateID = (int)GameStateID.SetTrap
        });
    }
    [ServerCallback]
    public void NetSendChangeCountDownStateMessage()
    {
        NetworkServer.SendToAll(new StateMessage
        {
            newStateID = (int)GameStateID.StartCountDown
        });
    }
    [Command(requiresAuthority = false)]
    public void CmdSyncSelectTFPos(Transform tf, Vector2 pos)
    {
        tf.position = pos;
    }
    [Command(requiresAuthority = false)]
    public void CmdSyncSelectTFPlaced()
    {
        Debug.Log("Select");
        NetworkRoomManagerExt.singleton.SelectCount++;
        if (NetworkRoomManagerExt.singleton.SelectCount >= NetworkManager.singleton.numPlayers)
            NetSendChangeSetTrapStateMessage();
    }
    [Command(requiresAuthority = false)]
    public void CmdSyncSetTFPlaced()
    {
        //Debug.Log("Placed");
        NetworkRoomManagerExt.singleton.SelectCount--;
        if (NetworkRoomManagerExt.singleton.SelectCount <= 0)
            NetSendChangeCountDownStateMessage();
    }

    void Update()
    {
        if (islocal&& Active)
        {
            newCursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (CameraBound)
            {
                newCursorPos.x = Mathf.Abs(newCursorPos.x) > CameraBound.bounds.max.x ? (CameraBound.bounds.max.x * newCursorPos.x / Mathf.Abs(newCursorPos.x)) : newCursorPos.x;
                newCursorPos.y = Mathf.Abs(newCursorPos.y) > CameraBound.bounds.max.y ? (CameraBound.bounds.max.y * newCursorPos.y / Mathf.Abs(newCursorPos.y)) : newCursorPos.y;
            }
            CmdUpdateCursorPosition(newCursorPos - offset);
            if (Input.GetMouseButtonDown(0))
            {
                if (curSelectTF == null)
                {
                    hit = Physics2D.Raycast(newCursorPos, Vector2.zero, 100f, layer);
                    if (hit.collider != null)
                    {
                        boxCollider = hit.collider;
                        objectCanPlaced = boxCollider.GetComponent<ObjectCanPlaced>();

                        if (!objectCanPlaced.IsSeleted)
                        {
                            curSelectTF = hit.collider.transform;
                            CmdSelectTF(curSelectTF);
                            this.DelayCallBack(delayTime, () =>
                            {
                                CmdObjectSelectState(objectCanPlaced.transform);// objectCanPlaced.IsSeleted = true;
                            });
                            CmdSyncSelectTFPlaced();
                        }
                    }
                }
                else if (CanPlace)
                {
                    //д������
                    if (objectCanPlaced.IsCanPlaced(boxCollider, contactFilter2D, result))
                    {
                        if (!objectCanPlaced.CallAfterPlaced(result))
                        {
                            PlacedPosAdjust(curSelectTF, boxCollider.bounds.center, boxCollider.bounds.size.y);
                            Debug.Log("Layer Setting");
                            CmdLayerSetting(curSelectTF);
                            curSelectTF.GetComponent<SpriteRenderer>().color = Color.white;
                        }
                        SetActive(false);
                        CmdSyncSetTFPlaced();
                    }
                }
            }

            if (curSelectTF != null)
            {
                objectCanPlaced.ShowPosAfterPlaced(boxCollider, contactFilter2D, result);
                CmdSyncSelectTFPos(curSelectTF,newCursorPos);
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdObjectSelectState(Transform tf)
    {
        RpcObjectSelectState(tf);
    }
    [ClientRpc]
    public void RpcObjectSelectState(Transform tf)
    {
        tf.GetComponent<ObjectCanPlaced>().IsSeleted = true;
    }

    [Command(requiresAuthority = false)]
    public void CmdLayerSetting(Transform tf)
    {
        RpcLayerSetting(tf);
    }
    [ClientRpc]
    public void RpcLayerSetting(Transform tf)
    {
        tf.GetComponent<ObjectCanPlaced>().IsSeleted = true;
        tf.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("DragBuilder");
    }

    [Command(requiresAuthority = false)]
    public void PlacedPosAdjust(Transform tf,Vector2 pos,float colHeight)
    {
        tf.GetComponent<ObjectCanPlaced>().PlacedPosAdjust(pos,colHeight);
    }
    public void SetActive(bool flag)
    {
        if (islocal)
        {
            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            cameraController.ChangeLookAtTarget(null);
            cameraController.SetInitial();
        }
        if (flag)
        {
            Active = true;
            CmdUpdateCursorPosition(new Vector2(0,0));
        }else
        {
            Debug.Log("SetActive false");
            Active = false;
            CmdUpdateCursorPosition(new Vector2(1000, 1000));
            curSelectTF = null;
            CanPlace = false;
            objectCanPlaced = null;
        }
    }

    //private void OnDisable()
    //{
    //    curSelectTF = null;
    //    canPlace = false;
    //    objectCanPlaced = null;
    //}

}
