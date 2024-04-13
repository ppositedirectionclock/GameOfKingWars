using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour
{
    public Vector3 targetPos;
    private Vector3 moveSpeed;
    private int speed=10;
    public GameObject hitEffectGo;
    // Start is called before the first frame update
    private void Start()
    {
        if (targetPos==Vector3.zero)
        {
            targetPos = transform.position;
        }
        moveSpeed = (targetPos - transform.position).normalized * speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, targetPos) <= 0.3f)
        {
            if (hitEffectGo)
            {
                Instantiate(hitEffectGo, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
        else
        {
            // 使用Lerp方法进行平滑移动
            transform.position += moveSpeed * Time.deltaTime;
        }
    }
}
