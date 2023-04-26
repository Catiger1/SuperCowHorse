using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.StateMachine
{
    internal class GameEntryState : State<Trigger>
    {
        protected override void Init()
        {
            base.Init();
            StateID = GameStateID.EnterGame;
        }
        public override void ActionState(IStateMachine sm)
        {

        }

        public override void EnterState(IStateMachine sm)
        {
            UnityEngine.Debug.Log("Enter GameEntry State");
            UnityEngine.Debug.Log("Change Scene to GameStartScene");
            SceneManager.LoadScene("GameStartScene");
        }

        public override void ExitState(IStateMachine sm)
        {

        }
    }
}
