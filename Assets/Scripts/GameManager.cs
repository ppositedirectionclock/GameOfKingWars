using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人：  曾嘉骏 
//功能说明：游戏总管理
//***************************************** 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public AudioSource audioSource;

    public List<int> battleCardsList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
    public List<int> allCardsList = new List<int>() { 9 };
    public AudioClip btnSound;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    /// <summary>
    /// 播放音乐
    /// </summary>
    /// <param name="audioClip">音乐资源</param>
    public void PlayMusic(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="audioClip">音效资源</param>
    public void PlaySound(AudioClip audioClip)
    {
        if (audioClip)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }
    public void PlayButtonSound()
    {
        audioSource.PlayOneShot(btnSound);
    }
}
