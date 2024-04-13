using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： 曾嘉骏
//功能说明：游戏世界中血条显示
//***************************************** 
public class HPSlider : MonoBehaviour
{
    private Camera mainCamera;
     public Slider slider;

    void Awake()
    {
        // 获取主摄像机的引用
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 获取摄像机的正方向并应用到 UI 画布上
        transform.LookAt(transform.position + 
            mainCamera.transform.rotation * Vector3.forward,
                         mainCamera.transform.rotation * Vector3.up);
    }

    public void SetHPColorSlider(bool isOrange)
    {
        if (isOrange)
        {
            slider = transform.Find("Slider_Red").GetComponent<Slider>();
        }
        else
        {
            slider = transform.Find("Slider_Blue").GetComponent<Slider>();
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void SetHPValue(float value)
    {
        if (!slider.gameObject.activeSelf)
        {
            slider.gameObject.SetActive(true);
        }
        slider.value = value;
    }
}
