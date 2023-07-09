using Assets.Scripts.Common;
using Assets.Scripts.StateMachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameStartScene : MonoBehaviour
{
    public Button ButtonHost;
    public Button ButtonClient;
    public Button ButtonSetting;
    public Button ButtonExit;
    public Button ButtonDescription;

    void Start()
    {
        ButtonHost.onClick.AddListener(() =>
        {
            GameStateManager.Instance.SetGameStateMachineFlag(GameTriggerID.GameRoomButton);
            NetworkManager.singleton.StartHost();
            AudioManager.Play(SoundName.Button);
        });
        ButtonClient.onClick.AddListener(() =>
        {
            GameStateManager.Instance.SetGameStateMachineFlag(GameTriggerID.GameRoomButton);
            NetworkManager.singleton.StartClient();
            AudioManager.Play(SoundName.Button);
        });
        ButtonSetting.onClick.AddListener(() =>
        {
            WindowsManager.Instance.OpenWindow(WindowsType.SettingWindow);
            AudioManager.Play(SoundName.Button);
        });
        ButtonExit.onClick.AddListener(ExitGame);

        ButtonDescription.onClick.AddListener(() => {
            WindowsManager.Instance.OpenWindow(WindowsType.SettingWindow);
            AudioManager.Play(SoundName.Button);
        });
    }

    
    private void ExitGame()
    {
        #if UNITY_EDITOR        //Unity编辑器中调试使用
                EditorApplication.isPlaying = false;
        #else                   //导出游戏包后使用
                Application.Quit();
        #endif
    }
}
