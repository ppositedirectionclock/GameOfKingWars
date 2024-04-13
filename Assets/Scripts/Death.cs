using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : Character
{
    public AudioClip callSkeletonClip;

    protected override void Start()
    {
        base.Start();
        InvokeRepeating("CreateSkeletons", 1, 4);
    }

    private void CreateSkeletons()
    {
        GameManager.Instance.PlaySound(callSkeletonClip);
        GameController.Instance.CreateUnit(11, transform.position + new Vector3(0, 0, -1), isOrange);
        GameController.Instance.CreateUnit(11, transform.position + new Vector3(-1, 0, -1), isOrange);
        GameController.Instance.CreateUnit(11, transform.position + new Vector3(1, 0, -1), isOrange);
    }
}
