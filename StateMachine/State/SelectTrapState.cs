using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.StateMachine.State
{
    internal class SelectTrapState : State<Trigger>
    {
        protected override void Init()
        {
            base.Init();
            StateID = GameStateID.SelectTrap;
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
