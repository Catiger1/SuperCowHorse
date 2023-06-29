using Assets.Scripts.Common;
using Mirror;
using Mirror.Examples.NetworkRoom;
using Mirror.Examples.NetworkRoomExt;
using UnityEngine;

namespace Assets.Scripts.StateMachine.State
{
    internal class ResultState : State<Trigger>
    {
        protected override void Init()
        {
            base.Init();
            StateID = GameStateID.Result;
        }
        public override void ActionState(IStateMachine sm)
        {

        }

        public override void EnterState(IStateMachine sm)
        {
            GameObject HidePos = GameObject.FindWithTag("HidePos");
            Transform player = NetworkClient.localPlayer.transform;
            player.position = HidePos.transform.position;

            AddScore();
            //WindowsManager.Instance.OpenWindow(WindowsType.ResultWindow);
            HidePos.GetComponent<MonoBehaviour>().DelayCallBack
            (1f, () => { WindowsManager.Instance.OpenWindow(WindowsType.ResultWindow); });
        }

        [ServerCallback]
        public void AddScore()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            bool dogfall = true;
            foreach(GameObject player in players)
            {
                if (player.GetComponent<PlayerScore>().isDeath)
                {
                    dogfall = false;
                    break;
                }
            }
            Debug.Log(dogfall);

            if(!dogfall)
                foreach (GameObject player in players)
                {
                    PlayerScore score = player.GetComponent<PlayerScore>();
                    if (!score.isDeath)
                    {
                        score.AddScore();
                        NetworkRoomManagerExt.singleton.TotalScore++;
                        Debug.Log("Add Score");
                    }
                }
        }

        public override void ExitState(IStateMachine sm)
        {
            
        }
    }
}
