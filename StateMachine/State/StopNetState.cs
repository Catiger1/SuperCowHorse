

using UnityEngine.SceneManagement;

namespace Assets.Scripts.StateMachine
{
    internal class StopNetState : State<Trigger>
    {
        protected override void Init()
        {
            base.Init();
            StateID = GameStateID.StopNet;
        }
        public override void ActionState(IStateMachine sm)
        {

        }

        public override void EnterState(IStateMachine sm)
        {

            //执行完后直接转到游戏入口
            sm.SetFlag(GameTriggerID.GameLoadingFinish);
        }

        public override void ExitState(IStateMachine sm)
        {

        }
    }
}
