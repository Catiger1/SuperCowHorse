using Assets.Scripts.Common;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class NetCursor : MonoBehaviour
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


    private void Start()
    {
        offset = GetComponent<SpriteRenderer>().size/2;
        offset.x = -offset.x;
        result = new Collider2D[2];
        contactFilter2D = new ContactFilter2D();
    }

    void Update()
    {
        newCursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = newCursorPos - offset;

        if(Input.GetMouseButtonDown(0))
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
                if (objectCanPlaced.IsCanPlaced(boxCollider,contactFilter2D,result))
                {
                    gameObject.SetActive(false);
                }
            }
        }

        if (curSelectTF != null)
        {
            objectCanPlaced.ShowPosAfterPlaced(boxCollider, contactFilter2D, result);
            curSelectTF.position = newCursorPos;
        }
    }

    private void OnDisable()
    {
        curSelectTF = null;
        canPlace = false;
        objectCanPlaced = null;
    }

}