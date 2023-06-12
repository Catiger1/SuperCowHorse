using System;

namespace Assets.Scripts.StateMachine
{
    internal class GameEntryButtonTrigger : Trigger
    {
        protected override void Init()
        {
            base.Init();
            TriggerID = GameTriggerID.GameEntryButton;
        }
        public override bool HandleTrigger(IStateMachine sm)
        {
            return (sm.GetAndClearFlag() & (int)GameTriggerID.GameEntryButton) != 0;
        }
    }
}
