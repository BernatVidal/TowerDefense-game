using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Freeze : Tower
{
    protected override void Attack()
    {
        base.Attack();
        anim.PlayAnimation(Animations.AnimationType.attack);
    }
}
