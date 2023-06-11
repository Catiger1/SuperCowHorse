

using System;

namespace Assets.Scripts.StateMachine
{
    internal class GameResultTrigger : Trigger
    {
        protected override void Init()
        {
            base.Init();
            TriggerID = GameTriggerID.GameResult;
        }
        public override bool HandleTrigger(IStateMachine sm)
        {
            throw new NotImplementedException();
        }
    }
}
