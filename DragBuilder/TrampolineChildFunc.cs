using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineChildFunc : MonoBehaviour
{
    public Animator anim;
    public float jumpVelocity;

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")&&!anim.GetBool("Jump"))
        {
            anim.SetBool("Jump", true);
            Rigidbody2D rigid = collision.GetComponent<Rigidbody2D>();
            rigid.velocity = new Vector2(rigid.velocity.x,0);
            transform.parent.GetComponent<NetworkFunction>().AddForce(collision.gameObject, Vector2.up * jumpVelocity);
        }
    }

}
