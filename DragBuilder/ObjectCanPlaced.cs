using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class ObjectCanPlaced:MonoBehaviour
{
    public bool IsSeleted { get; set; }
    public float RayLength = 0.7f;
    public LayerMask CanPlacedLayer;
    public abstract void Reset();
    public abstract void Initialize();
    public abstract bool IsCanPlaced(Collider2D collider, ContactFilter2D contactFilter2D, Collider2D[] result);
    /// <summary>
    /// Just Place Position On Gound
    /// </summary>
    /// <param name="pos"></param>
    public void PlacedPosAdjust(Vector2 hitPos,Collider2D col) {
        transform.position = new Vector2(transform.position.x, hitPos.y+ col.bounds.size.y/2);
    }

    public void ShowPosAfterPlaced(Collider2D collider, ContactFilter2D contactFilter2D, Collider2D[] result)
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
