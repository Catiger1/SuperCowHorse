
using System;

namespace Assets.Scripts.StateMachine
{
    internal class GameSelectTrapTrigger : Trigger
    {
        protected override void Init()
        {
            base.Init();
            TriggerID = GameTriggerID.GameSelectTrap;
        }
        public override bool HandleTrigger(IStateMachine sm)
        {
            throw new NotImplementedException();
        }
    }
}
