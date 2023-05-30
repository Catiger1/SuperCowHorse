using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetCursor : MonoBehaviour 
{
    // Update is called once per frame
    Vector3 newCursorPos;
    Vector2 offset;
    private void Start()
    {
        offset = GetComponent<SpriteRenderer>().size/2;
        offset.x = -offset.x;
    }
    void Update()
    {
        newCursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position =(Vector2)newCursorPos - offset;
    }
}
