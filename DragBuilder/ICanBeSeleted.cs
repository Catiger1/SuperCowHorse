using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.RuleTile.TilingRuleOutput;

public interface ICanBeSeleted
{
    public bool IsSeleted { get; set; }
}
