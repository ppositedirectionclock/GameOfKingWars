using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���˽���
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CreateUnits",5,20);//��5��ȥ����CreateUnits����ÿ��20���ظ�����CreateUnits
    }
    /// <summary>
    /// ���ɵз���λ
    /// </summary>
    private void CreateUnits()
    {
        GameController.Instance.CreateUnit(Random.Range(1,8),transform.position,false);//����ɫ�����ɻ���8����������
    }
}
