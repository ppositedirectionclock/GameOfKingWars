using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicFog : Unit
{
    public float timeVal;
    public float destoryTime;
    public AudioClip damageClip;
    public AudioClip healingClip;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if (unitInfo.attackValue >= 0)
        {
            GameManager.Instance.PlaySound(damageClip);
        }
        else
        {
            GameManager.Instance.PlaySound(healingClip);
        }
        Destroy(gameObject, destoryTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time-timeVal>=1)
        {
            for (int i = 0; i < targetsList.Count; i++)
            {
                targetsList[i].TakeDamage(unitInfo.attackValue, this);
            }
            timeVal = Time.time;
        }
    }
}
