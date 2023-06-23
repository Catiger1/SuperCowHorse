

namespace Assets.Scripts.StateMachine.State
{
    internal class SelectTrapState : State<Trigger>
    {
        protected override void Init()
        {
            base.Init();
            StateID = GameStateID.SelectTrap;
        }
        public override void ActionState(IStateMachine sm)
        {

        }

        public override void EnterState(IStateMachine sm)
        {
            UnityEngine.Debug.Log("Enter Select Trap State");
            WindowsManager.Instance.OpenWindow(WindowsType.PlacementWindow);
        }

        public override void ExitState(IStateMachine sm)
        {
            WindowsManager.Instance.GetWindow<PlacementWindow>(WindowsType.PlacementWindow).AutoCloseImmediately();
        }
    }
}
