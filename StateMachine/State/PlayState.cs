using Mirror;
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
            NetworkClient.localPlayer.GetComponent<PlayerInputManager>().CanControl = true;
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
            NetworkClient.localPlayer.GetComponent<PlayerInputManager>().CanControl = false;
            netCountdownTimer.StopTimerAndCallEndFunc();

            Transform spawnPos = GameObject.FindWithTag("Respawn").transform;
            for (int i = 0; i < spawnPos.childCount; i++)
            {
                spawnPos.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
            }
        }
    }
}
