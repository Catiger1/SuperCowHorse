using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void StopAnim(string anmationName)
    {
        if(animator!=null)
            animator.SetBool(anmationName,false);
    }
}
