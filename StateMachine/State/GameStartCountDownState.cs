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
            Transform spawnPos = GameObject.FindWithTag("Respawn").transform;
            for(int i=0;i<spawnPos.childCount;i++)
            {
                spawnPos.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
            }
            
            NetworkClient.localPlayer.transform.position = spawnPos.GetChild(NetworkClient.localPlayer.GetComponent<PlayerSelector>().CharacterIndex).position;
            GameObject[] timers = GameObject.FindGameObjectsWithTag("Timer");
            for (int i = 0; i < timers.Length; i++)
            {
                if (timers[i].name.Equals("CountdownTimerCanvas"))
                {
                    NetCountdownTimer netCountdownTimer = timers[i].GetComponent<NetCountdownTimer>();
                    netCountdownTimer.InitTimer(null, () =>
                    {
                        SendChangeStateMessage();
                    });
                    netCountdownTimer.SetActive(true);
                    netCountdownTimer.StartTimer();
                    break;
                }
            }
        }
        [ServerCallback]
        public void SendChangeStateMessage()
        {
            NetworkServer.SendToAll(new StateMessage
            {
                newStateID = (int)GameStateID.Play
            });
        }

        public override void ExitState(IStateMachine sm)
        {
        
        }
    }
}
