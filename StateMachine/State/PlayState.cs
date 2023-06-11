using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.StateMachine.State
{
    internal class PlayState : State<Trigger>
    {
        protected override void Init()
        {
            base.Init();
            StateID = GameStateID.Play;
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
