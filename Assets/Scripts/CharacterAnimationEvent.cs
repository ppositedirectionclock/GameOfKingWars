using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ��ɫ�����¼���
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
    /// �����¼�
    /// </summary>
    private void AttackAnimationEvent()
    {
        //Debug.Log("��ǰ��ɫ���������ˣ�");
        unit.AttackAnimationEvent();
    }
}
