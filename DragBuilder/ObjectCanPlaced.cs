using Mirror;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class ObjectCanPlaced:MonoBehaviour
{
    public bool IsSeleted { get; set; }
    public float RayLength = 0.7f;
    public LayerMask CanPlacedLayer;
    public bool IsNeedPosAdjust = false;
    public abstract void Reset();
    public abstract void Initialize();
    public abstract bool IsCanPlaced(Collider2D collider, ContactFilter2D contactFilter2D, Collider2D[] result);
    /// <summary>
    /// Just Place Position On Gound
    /// </summary>
    /// <param name="pos"></param>

    //public void PlacedPosAdjust(Vector2 hitPos,Collider2D col) {
    //    transform.position = new Vector2(transform.position.x, hitPos.y+ col.bounds.size.y/2);
    //}
    public void PlacedPosAdjust(Vector2 colPos, float colHeight)
    {
        if (IsNeedPosAdjust)
        {
            Vector2 rayFirePos = new Vector2(colPos.x, colPos.y - colHeight / 2);
            RaycastHit2D hit = Physics2D.Raycast(rayFirePos, Vector2.down, RayLength, CanPlacedLayer);
            transform.position = new Vector2(transform.position.x, hit.point.y + colHeight / 2);
        }
    }

    public virtual bool CallAfterPlaced(Collider2D[] result) { return false; }

    public virtual void ShowPosAfterPlaced(Collider2D collider, ContactFilter2D contactFilter2D, Collider2D[] result)
    {
        if(IsCanPlaced(collider,contactFilter2D,result))
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }else
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

}
