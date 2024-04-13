using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChioceUIManager : MonoBehaviour
{
    public Text goldText;
    public Text experienceText;
    public Text diamandText;
    public Transform[] storeCardsTrans;
    public Transform[] battleCardsTrans;
    public GameObject cardBtnGo;
    public Sprite[] cardSprites;
    public Image imgCard;
    public GameObject panelChestGo;
    private int chestCardNum;
    public Button chestPanelBtn;
    public Animator animator;
    public GameObject panelCardGo;
    public List<int> freeStorePosIndexList=new List<int>();
    public List<int> freeBattlePosIndexList = new List<int>();
    public AudioClip chestSound;
    public AudioClip chestBGMusic;
    public AudioClip cardSound;
    public AudioClip normalBG;
    /// <summary>
    /// 开箱
    /// </summary>
    public void OpenChest()
    {
        GameManager.Instance.PlayMusic(chestBGMusic);
        imgCard.sprite = cardSprites[0];
        chestPanelBtn.interactable = false;
        if (chestCardNum >= 4)
        {
            ClosePanelChest();
            return;
        }
        if (animator.enabled)
        {
            animator.CrossFade("CardMoveAnimation", 0);
        }
        Invoke("SetCardSprite", 0.7f);
    }

    private void SetCardSprite()
    {
        GameManager.Instance.PlaySound(cardSound);
        chestPanelBtn.interactable = true;
        chestCardNum++;       
        int num = Random.Range(1,13);
        imgCard.sprite = cardSprites[num];

    }

    private void ClosePanelChest()
    {
        chestCardNum = 0;
        panelChestGo.SetActive(false);
        GameManager.Instance.PlayMusic(normalBG);
    }

    public void OpenCardPanel()
    {
        panelCardGo.SetActive(true);
        //出战卡组
        for (int i = 0; i < GameManager.Instance.battleCardsList.Count; i++)
        {
            GameObject go= Instantiate(cardBtnGo, panelCardGo.transform);
            go.GetComponent<Image>().sprite = cardSprites[GameManager.Instance.battleCardsList[i]];
            go.transform.localPosition = battleCardsTrans[i].localPosition;
            BtnCard btnCard = go.GetComponent<BtnCard>();
            btnCard.isBattleCard = true;
            btnCard.chioceUIManager = this;
            btnCard.posIndex = i;           
        }
        if (battleCardsTrans.Length > GameManager.Instance.battleCardsList.Count)
        {
            for (int i = GameManager.Instance.battleCardsList.Count; i < battleCardsTrans.Length; i++)
            {
                freeBattlePosIndexList.Add(i);
            }
        }
        //卡牌堆
        for (int i = 0; i < GameManager.Instance.allCardsList.Count; i++)
        {
            GameObject go = Instantiate(cardBtnGo, panelCardGo.transform);
            go.GetComponent<Image>().sprite = cardSprites[GameManager.Instance.allCardsList[i]];
            go.transform.localPosition = storeCardsTrans[i].localPosition;
            BtnCard btnCard = go.GetComponent<BtnCard>();
            btnCard.chioceUIManager = this;
            btnCard.posIndex = i;
        }
        if (storeCardsTrans.Length > GameManager.Instance.allCardsList.Count)
        {
            for (int i = GameManager.Instance.allCardsList.Count; i < storeCardsTrans.Length; i++)
            {
                freeStorePosIndexList.Add(i);
            }
        }

    }

    public Vector3 GetFreeStorePos(int index)
    {
        Vector3 pos= storeCardsTrans[freeStorePosIndexList[0]].localPosition;
        freeStorePosIndexList.RemoveAt(0);
        freeBattlePosIndexList.Add(index);
        return pos;
    }

    public Vector3 GetFreeBattlePos(int index)
    {
        Vector3 pos = battleCardsTrans[freeBattlePosIndexList[0]].localPosition;
        freeBattlePosIndexList.RemoveAt(0);
        freeStorePosIndexList.Add(index);
        return pos;
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(2);
    }

    public void PlayButtonSound()
    {
        GameManager.Instance.PlayButtonSound();
    }
}
