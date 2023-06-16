
using System;

namespace Assets.Scripts.StateMachine
{
    internal class GameCountDownTrigger : Trigger
    {
        protected override void Init()
        {
            base.Init();
            TriggerID = GameTriggerID.GameStartCountDown;
        }
        public override bool HandleTrigger(IStateMachine sm)
        {
            return sm.GetAndClearFlag(GameTriggerID.GameStartCountDown);
        }
    }
}
