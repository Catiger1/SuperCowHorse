using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.RuleTile.TilingRuleOutput;

public abstract class ObjectCanPlaced:MonoBehaviour
{
    public bool IsSeleted { get; set; }

    public abstract void Reset();

    public abstract void Initialize();
}
