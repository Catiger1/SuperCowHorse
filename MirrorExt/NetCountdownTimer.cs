using DG.Tweening;
using Mirror;
using System;
using UnityEngine;
using UnityEngine.UI;

public class NetCountdownTimer : NetworkBehaviour
{
    public Text timerText;
    [SyncVar(hook = nameof(TimerValueChange))]
    public int SynTime = 30;
    private float countdownDuration = 30.0f;
    public int maxCountdownTime = 30;
    private Tweener countdownTween;
    private Action endFunc;
    private Action startFunc;
    private void OnEnable()
    {
        Reset();
    }

    public NetCountdownTimer InitTimer(Action startfunc,Action endfunc)
    {
        startFunc = startfunc;
        endFunc = endfunc;
        return this;
    }
    //[Command(requiresAuthority = false)]
    public void TimerValueChange(int _,int newValue)
    {
        timerText.text = FormatTime(newValue);
    }

    public void Reset()
    {
        countdownDuration = (int)maxCountdownTime;
        SynTime = (int)maxCountdownTime;
    }

    public void StartTimer()
    {
        startFunc?.Invoke();
        if(countdownTween==null)
        countdownTween = DOTween.To(() => countdownDuration, x => countdownDuration = x, 0.0f, countdownDuration)
            .OnUpdate(() => {
                if((int)countdownDuration< SynTime)
                {
                    SynTime = (int)countdownDuration;
                }
            }).SetEase(Ease.Linear)
            .OnComplete(() => {
                endFunc?.Invoke();
                countdownTween = null;
                Debug.Log("µ¹¼ÆÊ±½áÊø£¡");
            });
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
