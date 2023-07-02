using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Experimental;
using System.Linq;
using Assets.Scripts.GameManager;
using Assets.Scripts.Common;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

// NOTE: Do not put objects in DontDestroyOnLoad (DDOL) in Awake.  You can do that in Start instead.

public class Bullet : NetworkBehaviour,IResetable
{
    public LayerMask layer;
    public readonly float Speed = 10f;
    public float AliveTime = 10f;
    public float CurTime;
    public GameObject Owner;
    public float HitForce = 10f;

    public bool EnableState = false;
    private Vector3 hidePos = new Vector3(500, 500, 500);
    public void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void SetActive(bool flag,Vector2 pos, Quaternion quaternion)
    {
        if(flag)
            CurTime = Time.time;

        transform.position = pos;
        transform.rotation = quaternion;
        EnableState = flag;
    }

    [ServerCallback]
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(layer.value);
        // ´¦ÀíÅö×²Âß¼­
        if (!collision.gameObject != Owner && (collision.gameObject.layer & layer) > 0)
        {
            Debug.Log("Layer = "+collision.gameObject.layer);
            if (collision.gameObject.layer == 8)
                collision.GetComponent<Rigidbody2D>().AddForce(Vector2.up* HitForce, ForceMode2D.Impulse);

            NetworkPrefabPoolManager.Instance.CollectObject(gameObject, hidePos);
            EnableState = false;
            //SetActive(false);//NetworkServer.Destroy(gameObject);//CmdDestroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (!EnableState) return;
        if(Time.time-CurTime>AliveTime)
        {
            NetworkPrefabPoolManager.Instance.CollectObject(gameObject, hidePos);
            EnableState = false;
        }
        
        transform.Translate(Vector2.up * Speed * Time.fixedDeltaTime);
    }

    public void OnReset()
    {
        CurTime = Time.time;
        EnableState = true;
    }
}
