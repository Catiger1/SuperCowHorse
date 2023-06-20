using Mirror;
using UnityEngine;

namespace Assets.Scripts.StateMachine.State
{
    internal class GameStartCountDownState : State<Trigger>
    {
        protected override void Init()
        {
            base.Init();
            StateID = GameStateID.StartCountDown;
        }
        public override void ActionState(IStateMachine sm)
        {

        }

        public override void EnterState(IStateMachine sm)
        {
            UnityEngine.Debug.Log("Entry Game Start Count Down State");
            WindowsManager.Instance.GetWindow<PlacementWindow>(WindowsType.PlacementWindow).AutoCloseImmediately();
            Transform spawnPos = GameObject.FindWithTag("Respawn").transform;
            NetworkClient.localPlayer.transform.position = spawnPos.GetChild(NetworkClient.localPlayer.GetComponent<PlayerSelector>().CharacterIndex).position;
        }

        public override void ExitState(IStateMachine sm)
        {
        
        }
    }
}
