using Mirror;
using Mirror.Examples.NetworkRoomExt;
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
            ResetPlayer(spawnPos);
            Transform player = NetworkClient.localPlayer.transform;
            player.position = spawnPos.GetChild(NetworkClient.localPlayer.GetComponent<PlayerSelector>().CharacterIndex).position;
            Debug.Log(spawnPos.GetChild(NetworkClient.localPlayer.GetComponent<PlayerSelector>().CharacterIndex).position);
            //NetworkClient.localPlayer.GetComponent<PlayerScore>().isDeath = false;
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
                }else
                {
                    timers[i].GetComponent<NetCountdownTimer>().Reset();
                }
            }
        }
        [ServerCallback]
        public void ResetPlayer(Transform pos)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                player.GetComponent<PlayerScore>().isDeath = false;
                //player.transform.position = pos.GetChild(player.GetComponent<PlayerSelector>().CharacterIndex).position;
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
