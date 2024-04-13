using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//*****************************************
//创建人：曾嘉骏
//功能说明：单位基类
//***************************************** 
public class Unit : MonoBehaviour
{
    //基础属性与状态
    public UnitInfo unitInfo;
    public bool isOrange;
    public bool hasTarget;//如果有目标，则攻击，否则朝国王移动
    public int currentHP;//当前血量
    public bool isDead;

    //组件引用
    public Animator animator;
    public NavMeshAgent meshAgent;
    protected HPSlider hpslider;
    //其他引用
    public Unit targetUnit;//目标攻击单位
    public Unit defaultTarget;//默认攻击目标：国王或者两边某一个弓箭手
    public List<Unit> targetsList = new List<Unit>();//当前攻击范围内可选敌人列表(主动方)
    public List<Unit> attackerList = new List<Unit>();//攻击我们的敌人列表（被动方）
    protected Collider[] colliders;

    public AudioClip attackClip;
    public AudioClip dieClip;

    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        meshAgent = GetComponent<NavMeshAgent>();
        GetComponentInChildren<SphereCollider>().radius = unitInfo.attackArea * 2;
        colliders = GetComponentsInChildren<Collider>();
        currentHP = unitInfo.hp;
        if (unitInfo.hp > 0)
        {
            hpslider = transform.Find("Canvas_HP").GetComponent<HPSlider>();
            hpslider.SetHPColorSlider(isOrange);
        }
    }
    /// <summary>
    /// 单位移动
    /// </summary>
    protected virtual void UnitMove()
    {
        //有目标
        if (hasTarget)
        {
            //目标没有销毁且没有死亡
            if (targetUnit!=null&&!targetUnit.isDead)
            {
                //朝目标移动
                meshAgent.SetDestination(targetUnit.transform.position);
                JudegeIfReachTargetPos(transform.position,targetUnit.transform.position);
            }
            else
            {
                //重置目标
                ResetTarget(targetUnit);
            }
        }
        //无目标
        else
        {
            //获取当前默认目标位置
            GameController.Instance.UnitGetTargetPos(this,isOrange);
            //默认目标不为空
            if (defaultTarget!=null)
            {
                //朝默认目标进行移动
                meshAgent.SetDestination(defaultTarget.transform.position);
                JudegeIfReachTargetPos(transform.position, defaultTarget.transform.position);
            }
        }
    }
    /// <summary>
    /// 判断当前是否达到攻击范围并执行移动
    /// </summary>
    /// <param name="currentPos"></param>
    /// <param name="target"></param>
    public void JudegeIfReachTargetPos(Vector3 currentPos,Vector3 target)
    {
        //没有达到攻击范围
        if (Vector3.Distance(currentPos,target)>=unitInfo.attackArea)
        {
            //执行相关行为
            UnitBehaviour();
        }
        //达到攻击范围
        else
        {
            meshAgent.isStopped = true;
            //动画控制器播放攻击动画
            animator.SetBool("IsAttacking",true);
            animator.SetBool("IsMoving", false);
        }
    }
    /// <summary>
    /// 重置目标
    /// </summary>
    /// <param name="unit">目标</param>
    private void ResetTarget(Unit unit)
    {
        targetsList.Remove(unit);
        ClearDeadUnitInList();
        if (targetsList.Count>0)
        {
            SetTarget();
        }
        else//当前视野没有目标，那么攻击敌方建筑
        {
            hasTarget = false;
            targetUnit = null;
            GameController.Instance.UnitGetTargetPos(this,isOrange);
        }
    }
    /// <summary>
    /// 单位执行行为
    /// </summary>
    protected virtual void UnitBehaviour()
    {
        if (meshAgent.enabled)
        {
            meshAgent.isStopped = false;
        }
        //动画控制器播放移动动画
        animator.SetBool("IsMoving",true);
        animator.SetBool("IsAttacking",false);
    }
    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damageValue">攻击值</param>
    /// <param name="attacker">攻击者（攻击我们的人）</param>
    public void TakeDamage(int damageValue,Unit attacker)
    {
        currentHP -= damageValue;
        Mathf.Clamp(currentHP,0,unitInfo.hp);
        //显示血量到血条上 todo
        hpslider.SetHPValue((float)currentHP / unitInfo.hp);
        if (currentHP<=0)
        {
            Die(attacker);
        }
    }
    /// <summary>
    /// 单位死亡
    /// </summary>
    /// <param name="attacker">攻击者（攻击我们的人）</param>
    protected virtual void Die(Unit attacker)
    {
        GameManager.Instance.PlaySound(dieClip);
        isDead = true;
        animator.SetTrigger("Die");
        SetColliders(false);
        attacker.ResetTarget(this);
        RemoveSelfFromOtherAttack();
        //血条隐藏 todo
        hpslider.gameObject.SetActive(false);
        if (meshAgent)
        {
            if (meshAgent.enabled)
            {
                meshAgent.isStopped = true;
            }
        }
        //让该人物死亡后在战场上逗留5S
        Invoke("DestoryGame",5);
    }
    private void DestoryGame()
    {
        Destroy(gameObject);
    }
    /// <summary>
    /// 使其他攻击者把我们自身从攻击目标列表里移除
    /// </summary>
    private void RemoveSelfFromOtherAttack()
    {
        for (int i = 0; i < attackerList.Count; i++)
        {
            attackerList[i].targetsList.Remove(this);
        }
    }

    /// <summary>
    /// 攻击动画事件
    /// </summary>
    public virtual void AttackAnimationEvent()
    {
        //看向敌人
        if (hasTarget)
        {
            transform.LookAt(targetUnit.transform);
        }
        else
        {
            transform.LookAt(defaultTarget.transform);
        }
        if (targetUnit)
        {
            //敌人扣血
            targetUnit.TakeDamage(unitInfo.attackValue, this);
        }
        GameManager.Instance.PlaySound(attackClip);
    }
    /// <summary>
    /// 触发检测，用于检测范围内的敌人并锁定目标
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Unit"))
        {
            Unit unit= other.GetComponentInParent<Unit>();
            if (isOrange!=unit.isOrange)
            {
                //添加进目标列表里
                targetsList.Add(unit);
                //把我们自身作为攻击者添加到我们被攻击对象的攻击者列表里
                //目的是为了排除在战斗中因为各种情况下，随时死亡的单位
                unit.AddAttackerToList(this);
                //清除在列表中已经死亡的单位
                ClearDeadUnitInList();
                SetTarget();
            }
        }
    }
    /// <summary>
    /// 触发检测，用于检测攻击范围内的敌人离开重置目标并搜索新的攻击目标
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Unit"))
        {
            Unit unit = other.GetComponentInParent<Unit>();
            if (isOrange != unit.isOrange&&unit!=null&&targetUnit==unit)
            {
                ResetTarget(unit);
            }
        }
    }

    /// <summary>
    /// 我们被其他想要攻击我们的对象检测到了，所以需要把攻击我们的对象添加进
    /// 攻击者列表里边（攻击者是主动方，我们是被动方），所以这个方法需要其他unit单位去调用
    /// </summary>
    /// <param name="unit">攻击方</param>
    public void AddAttackerToList(Unit unit)
    {
        attackerList.Add(unit);
    }
    /// <summary>
    /// 清除在可选择攻击目标列表中已经死亡的单位
    /// </summary>
    private void ClearDeadUnitInList()
    {
        //临时清除列表 存的单位都是需要清除的对应位置索引
        List<int> clearList = new List<int>();
        //遍历一下整个目标列表
        for (int i = 0; i < targetsList.Count; i++)
        {
            //某一个元素为空时，把它对应的索引加入清除列表里
            if (targetsList[i]==null)
            {
                clearList.Add(i);
            }
        }
        //遍历临时清除列表清除所有已经销毁的unit对象
        for (int i = 0; i < clearList.Count; i++)
        {
            targetsList.RemoveAt(clearList[i]);
        }
    }
    /// <summary>
    /// 具体设置当前攻击目标,优先攻击最近目标
    /// </summary>
    private void SetTarget()
    {
        float closestDistance=Mathf.Infinity;//最短距离,默认值为无限大
        Unit u = null;//攻击目标 临时变量（容器）
        //遍历攻击目标列表
        for (int i = 0; i < targetsList.Count; i++)
        {
            float distance= Vector3.Distance(transform.position, targetsList[i].transform.position);
            if (distance<closestDistance)//如果当前距离比最短距离还要短
            {
                closestDistance = distance;//需要把作为标杆的值更新为更短的值
                u = targetsList[i];//更新离我们最近的单位
            }
        }
        targetUnit = u;
        hasTarget = true;
    }
    /// <summary>
    /// 设置碰撞器的激活失活状态（目的是为了让其可以进行或停止触发检测）
    /// </summary>
    /// <param name="state"></param>
    public void SetColliders(bool state)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = state;
        }
    }
}
/// <summary>
/// 单位信息
/// </summary>
//[Serializable]
public struct UnitInfo
{
    public int id;
    public string unitName;
    public int cost;
    public int hp;
    public float attackArea;
    public int speed;
    public int attackValue;
    public bool canCreateAnywhere;
}
