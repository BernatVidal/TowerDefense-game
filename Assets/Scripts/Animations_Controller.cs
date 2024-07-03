using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used in Scriptable Objects so Animations can be defined on the inspector
/// </summary>
[Serializable]
public struct Animations
{
    public enum AnimationType
    {
        walk,
        attack,
        hit,
        die,
        idle
    }

    public AnimationType animationType;
    public AnimationClip animationClip;
}


/// <summary>
/// Basic controls for animations
/// </summary>
public class Animations_Controller : MonoBehaviour
{
    Dictionary<Animations.AnimationType, AnimationClip> anims;
    Animator animator;

    public void SetAnimationController(Animations[] anims)
    {
        this.anims = new();
        foreach (Animations anim in anims)
            this.anims.Add(anim.animationType, anim.animationClip);
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation(Animations.AnimationType animType)
    {
        if (anims.ContainsKey(animType))
        {
            animator.Play(anims[animType].name);
            SetAnimSpeed(1);
        }
    }

    public void PauseAnimation(Animations.AnimationType animType)
    {
        if (anims.ContainsKey(animType))
        {
            SetAnimSpeed(0);
        }
    }

    public void SetAnimSpeed(float newSpeed)
    {
        animator.speed = newSpeed;
    }

    public float GetAnimDuration()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length;
    }
}
