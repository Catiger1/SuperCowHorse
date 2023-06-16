using System;

namespace Assets.Scripts.StateMachine
{
    internal class GameLoadingFinishTrigger : Trigger
    {
        protected override void Init()
        {
            base.Init();
            TriggerID = GameTriggerID.GameLoadingFinish;
        }
        public override bool HandleTrigger(IStateMachine sm)
        {
            return sm.GetAndClearFlag(GameTriggerID.GameLoadingFinish);
        }
    }
}
