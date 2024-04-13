using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：人物角色
//***************************************** 
public class Character : Unit
{
    protected override void Start()
    {
        base.Start();
        meshAgent.speed = unitInfo.speed;
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }
        UnitMove();
    } 
}
