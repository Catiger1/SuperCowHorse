using Assets.Scripts.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.StateMachine
{
    interface ITrigger
    {
        bool HandleTrigger(IStateMachine sm);
    }
}
