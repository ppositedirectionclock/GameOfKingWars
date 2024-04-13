using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：建筑
//***************************************** 
public class Building : Unit
{
    public bool isKing;
    private float attackCD = 1.4f;
    private float attackTimer;
    public Transform characterTrans;
    public GameObject[] turretGos;
    public AudioClip destoryClip;


    protected override void Start()
    {
        unitInfo = GameController.Instance.unitInfos[12];
        base.Start();
        if (isKing)
        {
            SetColliders(false);
            unitInfo.attackArea += 0.5f;
            unitInfo.attackValue += 1;
            unitInfo.hp *= 2;
        }
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }
        Attack();
    }
    /// <summary>
    /// 建筑攻击方法
    /// </summary>
    private void Attack()
    {
        if (Time.time-attackTimer>=attackCD)
        {
            attackTimer = Time.time;
            if (hasTarget&&targetUnit!=null)
            {
                animator.SetBool("IsAttacking", true);
                characterTrans.LookAt(new Vector3(targetUnit.transform.position.x, 
                    characterTrans.position.y, targetUnit.transform.position.z));
                targetUnit.TakeDamage(unitInfo.attackValue,this);
            }
            else
            {
                animator.SetBool("IsAttacking",false);
            }
        }
    }

    protected override void Die(Unit attacker)
    {
        base.Die(attacker);
        turretGos[0].SetActive(false);
        turretGos[1].SetActive(true);
        GameManager.Instance.PlaySound(destoryClip);
        if (isKing)
        {
            //游戏结束
            UIManager.Instance.GameOver(!isOrange);
        }
        else
        {
            GameController.Instance.EnableKing(isOrange);
        }
    }

    public override void AttackAnimationEvent()
    {
        
    }
}
