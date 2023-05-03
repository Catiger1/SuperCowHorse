using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Assets.Scripts.StateMachine
{
    internal class GameLoadingState : State<Trigger>
    {
        protected override void Init()
        {
            base.Init();
            StateID = GameStateID.LoadingGame;
        }
        public override void ActionState(IStateMachine sm)
        {
            
        }

        public override void EnterState(IStateMachine sm)
        {
            UnityEngine.Debug.Log("Enter GameLoading State");
            AudioManager.Play(SoundName.BGM0);
        }

        public override void ExitState(IStateMachine sm)
        {
            UnityEngine.Debug.Log("Exit GameLoading State");
        }
    }
}
