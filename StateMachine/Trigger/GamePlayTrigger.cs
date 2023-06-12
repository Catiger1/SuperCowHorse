
using System;

namespace Assets.Scripts.StateMachine
{
    internal class GamePlayTrigger : Trigger
    {
        protected override void Init()
        {
            base.Init();
            TriggerID = GameTriggerID.GamePlay;
        }
        public override bool HandleTrigger(IStateMachine sm)
        {
            return (sm.GetAndClearFlag() & (int)GameTriggerID.GamePlay) != 0;
        }
    }
}
