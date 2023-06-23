﻿
using Mirror;
using Mirror.Examples.NetworkRoom;
using UnityEngine;

namespace Assets.Scripts.StateMachine.State
{
    internal class SetTrapState : State<Trigger>
    {
        protected override void Init()
        {
            base.Init();
            StateID = GameStateID.SetTrap;
        }
        public override void ActionState(IStateMachine sm)
        {

        }

        public override void EnterState(IStateMachine sm)
        {
            UnityEngine.Debug.Log("Enter Set Trap State");
            WindowsManager.Instance.GetWindow<PlacementWindow>(WindowsType.PlacementWindow).NetCursor.CanPlace = true;
        }
        public override void ExitState(IStateMachine sm)
        {
            NetworkRoomManagerExt.singleton.SelectCount = 0;
        }
    }
}
