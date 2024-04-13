using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BtnCard : MonoBehaviour
{
    public bool isBattleCard;
    public int posIndex;
    public ChioceUIManager chioceUIManager;
    private Button button;

    private void Start()
    {
        button=GetComponent<Button>();
        button.onClick.AddListener(MoveToTargetPos);
    }

    public void MoveToTargetPos()
    {
        button.interactable = false;     
        if (chioceUIManager.freeStorePosIndexList.Count > 0)
        {
            if (isBattleCard)
            {
                if (chioceUIManager.freeStorePosIndexList.Count <= 0)
                {
                    button.interactable = true;
                    return;
                }
                transform.DOLocalMove(chioceUIManager.GetFreeStorePos(posIndex), 0.3f).
                    OnComplete(() => { button.interactable = true; isBattleCard = false; }
                    );

            }
            else
            {
                if (chioceUIManager.freeBattlePosIndexList.Count <= 0)
                {
                    button.interactable = true;
                    return;
                }
                transform.DOLocalMove(chioceUIManager.GetFreeBattlePos(posIndex), 0.3f).
                    OnComplete(() => { button.interactable = true; isBattleCard = true; }
                    );
            }
        }      
    }
}
