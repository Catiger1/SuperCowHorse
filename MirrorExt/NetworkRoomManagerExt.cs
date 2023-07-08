using Assets.Scripts.Common;
using Assets.Scripts.StateMachine;
using Mirror.Examples.NetworkRoomExt;
using System;
using UnityEngine;
using UnityEngine.UI;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/components/network-manager
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

namespace Mirror.Examples.NetworkRoom
{
    [AddComponentMenu("")]
    public class NetworkRoomManagerExt : NetworkRoomManager
    {
        [Header("Spawner Setup")]
        [Tooltip("Reward Prefab for the Spawner")]
        public GameObject rewardPrefab;

        public int SelectCount;
        public int AliveCount;
        public int TotalScore;
        public int MaxTotalScore = 5;
        public static new NetworkRoomManagerExt singleton { get; private set; }

        public static bool IsGameFinish()
        {
            bool flag = singleton.numPlayers <= 1||(singleton.TotalScore >= singleton.MaxTotalScore) && NetworkServer.active && Utils.IsSceneActive(singleton.GameplayScene);
            if(flag)singleton.ServerChangeScene(singleton.RoomScene);
            return flag;
        }

        
        //public NetworkRoomPlayerExt LocalPlayer { get; set; }
        /// <summary>
        /// Runs on both Server and Client
        /// Networking is NOT initialized when this fires
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            singleton = this;
        }

        /// <summary>
        /// This is called on the server when a networked scene finishes loading.
        /// </summary>
        /// <param name="sceneName">Name of the new scene.</param>
        public override void OnRoomServerSceneChanged(string sceneName)
        {
            // spawn the initial batch of Rewards
            if (sceneName == GameplayScene)
            {
                Spawner.InitialSpawn();
                //Spawner.SpawnCursor(this);
            }
        }

        public override GameObject OnRoomServerCreateGamePlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
        {
            return base.OnRoomServerCreateGamePlayer(conn, roomPlayer);
        }
        /// <summary>
        /// Called just after GamePlayer object is instantiated and just before it replaces RoomPlayer object.
        /// This is the ideal point to pass any data like player name, credentials, tokens, colors, etc.
        /// into the GamePlayer object as it is about to enter the Online scene.
        /// </summary>
        /// <param name="roomPlayer"></param>
        /// <param name="gamePlayer"></param>
        /// <returns>true unless some code in here decides it needs to abort the replacement</returns>
        public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer, GameObject gamePlayer)
        {
            PlayerSelector playerSelector = gamePlayer.GetComponent<PlayerSelector>();
            gamePlayer.GetComponent<PlayerScore>().index = roomPlayer.GetComponent<NetworkRoomPlayerExt>().index;
            playerSelector.CharacterIndex = roomPlayer.GetComponent<NetworkRoomPlayerExt>().CurSelectCharacterIndex;//roomPlayer.GetComponent<NetworkRoomManagerExt>().room;
            return true;
        }

        public override void OnRoomStopClient()
        {
            base.OnRoomStopClient();
        }

        public override void OnRoomStopServer()
        {
            base.OnRoomStopServer();
        }

        /*
            This code below is to demonstrate how to do a Start button that only appears for the Host player
            showStartButton is a local bool that's needed because OnRoomServerPlayersReady is only fired when
            all players are ready, but if a player cancels their ready state there's no callback to set it back to false
            Therefore, allPlayersReady is used in combination with showStartButton to show/hide the Start button correctly.
            Setting showStartButton false when the button is pressed hides it in the game scene since NetworkRoomManager
            is set as DontDestroyOnLoad = true.
        */

        bool showStartButton;

        public override void OnRoomServerPlayersReady()
        {
            // calling the base method calls ServerChangeScene as soon as all players are in Ready state.
#if UNITY_SERVER
            base.OnRoomServerPlayersReady();
#else
            showStartButton = true;
#endif
        }
        public override void OnStartClient()
        {
            base.OnStartClient();
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            Debug.Log("DisConnect Client");
            base.OnServerDisconnect(conn);
        }
        public override void OnRoomClientDisconnect()
        {
            base.OnRoomClientDisconnect();
        }

        public override void OnClientStateInternal(StateMessage msg)
        {
            GameStateManager.Instance.ChangeState(msg.newStateID);
        }

        bool isShowingStartBtn = false;

        public override void OnGUI()
        {
            base.OnGUI();

            if (allPlayersReady && showStartButton)// && GUI.Button(new Rect(150, 300, 120, 20), "START GAME"))
            {
                // set to false to hide it in the game scene
                showStartButton = false;

                Transform roomCanvas = GameObject.FindGameObjectWithTag("RoomCanvas").transform;
                Button btnRoomStart = roomCanvas.FindChildByName<Button>("BtnRoomStart");
                btnRoomStart.gameObject.SetActive(true);
                btnRoomStart.onClick.RemoveAllListeners();
                isShowingStartBtn = true;
                btnRoomStart.onClick.AddListener(() =>
                {
                    isShowingStartBtn = false;
                    btnRoomStart.gameObject.SetActive(false);
                    ServerChangeScene(GameplayScene);
                    NetworkServer.SendToAll(new StateMessage
                    {
                        newStateID =(int)GameStateID.EnterGamePlayRoom
                    });
                });  
            }else if(isShowingStartBtn&&!allPlayersReady)
            {
                Transform roomCanvas = GameObject.FindGameObjectWithTag("RoomCanvas").transform;
                GameObject btnRoomStartGo = roomCanvas.FindChildByName("BtnRoomStart").gameObject;
                btnRoomStartGo.SetActive(false);
            }
        }

    }
}
