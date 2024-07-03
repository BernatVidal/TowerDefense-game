using UnityEngine;

public class Tower_Bomb : Tower
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (target != null)
            RotateCannonToTarget();
    }

    void RotateCannonToTarget()
    {
        Vector3 targetPostition = new Vector3(target.transform.position.x, shootingPos.position.y, target.transform.position.z);
        shootingPos.LookAt(targetPostition);
    }

    protected override void Attack()
    {
        base.Attack();
        anim.PlayAnimation(Animations.AnimationType.attack);
    }
}