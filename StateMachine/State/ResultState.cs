﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.StateMachine.State
{
    internal class ResultState : State<Trigger>
    {
        protected override void Init()
        {
            base.Init();
            StateID = GameStateID.Result;
        }
        public override void ActionState(IStateMachine sm)
        {

        }

        public override void EnterState(IStateMachine sm)
        {

        }

        public override void ExitState(IStateMachine sm)
        {

        }
    }
}