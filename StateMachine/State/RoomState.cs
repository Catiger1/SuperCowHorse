
using Assets.Scripts.GameManager;
using Mirror.Examples.NetworkRoom;

namespace Assets.Scripts.StateMachine.State
{
    internal class RoomState : State<Trigger>
    {
        protected override void Init()
        {
            base.Init();
            StateID = GameStateID.Room;
        }
        public override void ActionState(IStateMachine sm)
        {

        }

        public override void EnterState(IStateMachine sm)
        {
            UnityEngine.Debug.Log("RoomState");
            WindowsManager.Instance.GetWindow<PlacementWindow>(WindowsType.PlacementWindow).Clear();
            NetworkRoomManagerExt.singleton.TotalScore = 0;

        }

        public override void ExitState(IStateMachine sm)
        {

        }
    }
}
