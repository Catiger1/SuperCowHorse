using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Assets.Scripts.GameManager;
using Assets.Scripts.Common;
using Mirror.Experimental;

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
    public Transform FirePos;
    public float HitForce = 100f;

    public bool EnableState = false;
    private Vector3 hidePos = new Vector3(500, 500, 500);

    [ServerCallback]
    void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Can Layer = " + layer.value);
        if (Owner!=null&&!Owner.Equals(collision.gameObject) && (collision.gameObject.layer & layer) > 0)
        {
            Debug.Log("Owner = " + Owner);
            Debug.Log("Name = " + collision.gameObject.name + " Layer = " + collision.gameObject.layer);
            if (collision.gameObject.layer == 3)
            {
                Debug.Log("Add Force To GamePlayer");
                Vector2 forceDir = transform.rotation.eulerAngles.z > 180 ? Vector2.right : Vector2.left;
                AddForce(collision.gameObject, HitForce * forceDir);
            }
            else if (collision.gameObject.layer == 5) return;
            NetworkPrefabPoolManager.Instance.CollectObject(gameObject, hidePos);
            EnableState = false;
        }
    }
    [ClientRpc]
    public void AddForce(GameObject go,Vector2 dir)
    {
        go.GetComponent<Rigidbody2D>().AddForce(dir, ForceMode2D.Impulse);
    }

    void FixedUpdate()
    {
        if (!EnableState)
        {
            if (FirePos != null)
                transform.position = FirePos.position;
            return;
        }
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
