

namespace Assets.Scripts.StateMachine
{
    internal class GameSetTrapTrigger : Trigger
    {
        protected override void Init()
        {
            base.Init();
            TriggerID = GameTriggerID.GameSetTrap;
        }
        public override bool HandleTrigger(IStateMachine sm)
        {
            return sm.GetAndClearFlag(GameTriggerID.GameSetTrap);
        }
    }
}
