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
            UnityEngine.Debug.Log("GameEntry State");
            //SceneManager.LoadScene("GameStartScene");
            AudioManager.Play(SoundName.BGM1);
        }

        public override void ExitState(IStateMachine sm)
        {

        }
    }
}
