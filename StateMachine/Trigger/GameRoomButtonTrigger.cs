

using System;

namespace Assets.Scripts.StateMachine
{
    internal class GameRoomButtonTrigger : Trigger
    {
        protected override void Init()
        {
            base.Init();
            TriggerID = GameTriggerID.GameRoomButton;
        }
        public override bool HandleTrigger(IStateMachine sm)
        {
            return (sm.GetAndClearFlag() & (int)GameTriggerID.GameRoomButton) != 0;
        }
    }
}
