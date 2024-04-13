using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingAngel : Character
{
    private float healNum=2;
    public Bullet ball;

    public override void AttackAnimationEvent()
    {
        base.AttackAnimationEvent();
        Bullet bullet = Instantiate(ball, transform.position, Quaternion.identity);
        if (hasTarget)
        {
            if (targetUnit)
            {
                bullet.targetPos = targetUnit.transform.position;
            }          
        }
        else
        {
            bullet.targetPos = defaultTarget.transform.position;
        }     
        healNum++;
        if (healNum>=2)
        {
            GameController.Instance.CreateUnit(12, transform.position, !isOrange);
            healNum = 0;
        }     
    }
}
