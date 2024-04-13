using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Effect : MonoBehaviour
{
    public float destoryTime=2;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destoryTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
