using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 角色动画事件类
/// </summary>
public class CharacterAnimationEvent : MonoBehaviour
{
    private Unit unit;

    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponentInParent<Unit>();
    }
    /// <summary>
    /// 动画事件
    /// </summary>
    private void AttackAnimationEvent()
    {
        //Debug.Log("当前角色攻击到人了！");
        unit.AttackAnimationEvent();
    }
}
