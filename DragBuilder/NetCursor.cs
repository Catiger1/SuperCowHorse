using Assets.Scripts.Common;
using Mirror;
using UnityEngine;

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
    bool canPlace = false;

    private Transform GenerateGos;
    public GameObject localPlayer;
    public bool islocal = false;
    //[SyncVar(hook = nameof(CmdSetPos))]
    public bool Active = false;
    private void Start()
    {
        offset = GetComponent<SpriteRenderer>().size/2;
        offset.x = -offset.x;
        result = new Collider2D[2];
        contactFilter2D = new ContactFilter2D();
        CmdUpdateCursorPosition(new Vector2(1000, 1000));
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
        if(islocal)
            WindowsManager.Instance.GetWindow<PlacementWindow>(WindowsType.PlacementWindow).SetNetCursor(this);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetPos(bool _,bool flag)
    {
        if (flag == false)
            CmdUpdateCursorPosition(new Vector2(1000, 1000));
    }
    [ServerCallback]
    public void TrapStateEnd(Transform parent)
    {
        if(parent.childCount<=5-NetworkManager.singleton.numPlayers)
        {
            Debug.Log("Break;");
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSyncSelectTFPos(Transform tf,Vector2 pos)
    {
        tf.position = pos;
    }
    [Command(requiresAuthority = false)]
    public void CmdSyncSelectTFPlaced(Transform tf, Transform parent)
    {
        Transform parentTF = tf.parent;
        tf.SetParent(parent);
        TrapStateEnd(parentTF);
    }

    void Update()
    {
        if (islocal&& Active)
        {
            newCursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CmdUpdateCursorPosition(newCursorPos - offset);
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
                    //д������
                    if (objectCanPlaced.IsCanPlaced(boxCollider, contactFilter2D, result))
                    {
                        CmdSyncSelectTFPlaced(curSelectTF, GenerateGos);
                        Active = false;
                        CmdUpdateCursorPosition(new Vector2(1000, 1000));
                        //TrapStateEnd(parentTF);
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

    private void OnDisable()
    {
        curSelectTF = null;
        canPlace = false;
        objectCanPlaced = null;
    }

}
