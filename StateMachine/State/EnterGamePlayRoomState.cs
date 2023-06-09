﻿
using Assets.Scripts.GameManager;
using UnityEngine;

namespace Assets.Scripts.StateMachine
{
    internal class EnterGamePlayRoomState : State<Trigger>
    {
        protected override void Init()
        {
            base.Init();
            StateID = GameStateID.EnterGamePlayRoom;
        }
        public override void ActionState(IStateMachine sm)
        {

        }

        public override void EnterState(IStateMachine sm)
        {
            UnityEngine.Debug.Log("Enter Game Play Room");
            NetworkPrefabPoolManager.Instance.ClearAll();
            Cursor.visible = false;
        }

        public override void ExitState(IStateMachine sm)
        {

        }
    }
}
