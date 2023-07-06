using Assets.Scripts.Common;
using Assets.Scripts.GameManager;
using Mirror;
using Mirror.Examples.NetworkRoom;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInputManager = Mirror.Examples.NetworkRoom.PlayerInputManager;

namespace Assets.Scripts.StateMachine.State
{
    internal class PlayState : State<Trigger>
    {
        NetCountdownTimer netCountdownTimer;
        protected override void Init()
        {
            base.Init();
            StateID = GameStateID.Play;
        }
        public override void ActionState(IStateMachine sm)
        {

        }
        public override void EnterState(IStateMachine sm)
        {
            UnityEngine.Debug.Log("Entry Play State");
            NetworkRoomManagerExt.singleton.AliveCount = NetworkRoomManagerExt.singleton.numPlayers;

            NetworkClient.localPlayer.GetComponent<PlayerInputManager>().CanControl = true;
            
            NetCursor netCursor = WindowsManager.Instance.GetWindow<PlacementWindow>(WindowsType.PlacementWindow).NetCursor;
            Camera.main.GetComponent<CameraController>().ChangeLookAtTarget(NetworkClient.localPlayer.transform);
           
            GameObject[] timers = GameObject.FindGameObjectsWithTag("Timer");
            for (int i = 0; i < timers.Length; i++)
            {
                if (timers[i].name.Equals("CountdownTimer"))
                {
                    netCountdownTimer = timers[i].GetComponent<NetCountdownTimer>();
                    netCountdownTimer.InitTimer(null, () => {
                        SendChangeStateMessage();
                    });
                    netCountdownTimer.SetActive(true);
                    netCountdownTimer.StartTimer();
                }
            }
        }

        [ServerCallback]
        public void SendChangeStateMessage()
        {
            NetworkServer.SendToAll(new StateMessage
            {
                newStateID = (int)GameStateID.Result
            });
        }

        public override void ExitState(IStateMachine sm)
        {
            GameObject spawnPos = GameObject.FindWithTag("Respawn");
            if(spawnPos != null) { 
            //Transform spawnPos = GameObject.FindWithTag("Respawn").transform;
            NetworkClient.localPlayer.GetComponent<PlayerInputManager>().CanControl = false;
            netCountdownTimer.StopTimerAndCallEndFunc();
            
                for (int i = 0; i < spawnPos.transform.childCount; i++)
                {
                    spawnPos.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
                }
            }
        }
    }
}
