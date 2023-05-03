using Assets.Scripts.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameStartScene : MonoBehaviour
{
    public Button ButtonPlay;
    public Button ButtonSetting;
    public Button ButtonExit;
    void Start()
    {
        ButtonPlay.onClick.AddListener(() => { Debug.Log("Play");
            AudioManager.Play(SoundName.Button);
        });
        ButtonSetting.onClick.AddListener(() => {
            WindowsManager.Instance.OpenWindow(WindowsType.SettingWindow);
            AudioManager.Play(SoundName.Button);
        });
        ButtonExit.onClick.AddListener(ExitGame);
    }

    
    private void ExitGame()
    {
        #if UNITY_EDITOR        //Unity�༭���е���ʹ��
                EditorApplication.isPlaying = false;
        #else                   //������Ϸ����ʹ��
                Application.Quit();
        #endif
    }
}
