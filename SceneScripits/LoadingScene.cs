using DG.Tweening;
using UnityEngine.UI;
using Assets.Scripts.StateMachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoSingleton<LoadingScene>
{
    public Slider slider;
    public Transform EndPos;
    public Transform UIBarCharacter;
    public float deltaTime = 5;
    private float time;
    void Start()
    {
        UIBarCharacter.DOMoveX(EndPos.position.x, deltaTime).SetEase(Ease.Linear);
        time = Time.time;
    }
    
    void Update()
    {
        if ((Time.time - time) > (deltaTime * (slider.value + (slider.maxValue / 6))))
        {
            slider.value += slider.maxValue / 6;
            if (slider.maxValue - slider.value < 0.1f)
            {
                SceneManager.LoadScene("GameStartScene");
                GameStateManager.Instance.SetGameStateMachineFlag(GameTriggerID.GameLoadingFinish);
            }
        };
    }
}
