using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoSingleton<ScreenManager>
{
    public override void Init()
    {
        base.Init();
        Screen.fullScreen = false;
    }
}
