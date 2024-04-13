using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 敌人进程
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CreateUnits",5,20);//隔5秒去调用CreateUnits，再每隔20秒重复调用CreateUnits
    }
    /// <summary>
    /// 生成敌方单位
    /// </summary>
    private void CreateUnits()
    {
        GameController.Instance.CreateUnit(Random.Range(1,8),transform.position,false);//在紫色方生成基础8个卡牌人物
    }
}
