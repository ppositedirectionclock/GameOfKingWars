using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
//*****************************************
//创建人： 曾嘉骏
//功能说明：游戏场景中UI管理
//***************************************** 
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Text energyText;
    public Slider energySlider;
    public Text leftTimeText;
    private List<int> cardIDList=new List<int>();
    public GameObject cardGo;
    public Sprite[] cardSprites;
    public Sprite[] cardDisSprites;
    private int maxContentNum=4;//卡牌面板最大容纳量
    private int currentDoardNum;//当前卡牌面板上的卡牌数
    public Transform nextCardT;
    public Transform[] boardCardsT;//卡牌面板上的四个空位置
    public Transform boardTrans;//卡牌面板的Transform引用
    public GameObject losePanelGo;
    public GameObject winPanelGo;
    public GameObject endPanelGo;
    public GameObject startPanelGo;
    public GameObject maskPanelGo;
    public AudioClip winClip;
    public AudioClip loseClip;

    private void Awake()
    {
        Instance = this;
        CreateNewCard();
        DOTween.To(() => Camera.main.orthographicSize, x => Camera.main.orthographicSize = x, 11.8f, 2)
          .OnComplete(() => { startPanelGo.SetActive(false); });
    }

    void Update()
    {

    }
    /// <summary>
    /// 设置圣水数量
    /// </summary>
    public void SetEnergySliderValue()
    {
        energyText.text = ((int)GameController.Instance.energyValue).ToString();
        energySlider.value = GameController.Instance.energyValue / 10;
    }
    /// <summary>
    /// 设置时间
    /// </summary>
    /// <param name="min">分</param>
    /// <param name="sec">秒</param>
    public void SetTimeValue(int min,int sec)
    {
        leftTimeText.text = min.ToString() + ":" + sec.ToString();
    }
    /// <summary>
    /// 从Next卡槽创建新卡牌
    /// </summary>
    private void CreateNewCard()
    {
        if (currentDoardNum>maxContentNum)
        {
            return;
        }
        GameObject go= Instantiate(cardGo,nextCardT);
        go.transform.localPosition = Vector3.zero;
        int randomNum= Random.Range(1,11);
        while (cardIDList.Contains(randomNum))//当前列表是否包含随机出来的卡牌ID
        {
            //已包含，则重新生成
            randomNum = Random.Range(1, 11);
        }
        cardIDList.Add(randomNum);
        Image image = go.transform.GetChild(0).GetComponent<Image>();
        //设置卡牌样式
        image.sprite = cardSprites[randomNum - 1];
        //设置不卡交互时的样式
        Button button = go.transform.GetChild(0).GetComponent<Button>();
        SpriteState ss= button.spriteState;
        ss.disabledSprite = cardDisSprites[randomNum-1];
        button.spriteState = ss;

        go.GetComponent<Card>().id = randomNum;
        if (currentDoardNum<maxContentNum)
        {
            MoveCardToBoard(currentDoardNum);
        }
    }
    /// <summary>
    /// 移动卡牌到卡牌面板
    /// </summary>
    /// <param name="posID">卡牌位置ID</param>
    private void MoveCardToBoard(int posID)
    {
        Transform t= nextCardT.GetChild(0);
        t.SetParent(boardTrans);
        t.DOScale(Vector3.one,0.2f);
        t.GetComponent<Card>().posID=posID;
        t.DOLocalMove(boardCardsT[posID].localPosition,0.2f).OnComplete
            (() => { CompleteMoveTween(t); });
    }

    /// <summary>
    /// 某张卡牌移动动画完成后调用的方法
    /// </summary>
    private void CompleteMoveTween(Transform t)
    {
        currentDoardNum++;
        CreateNewCard();
        t.GetComponent<Card>().SetInitPos();
    }
    /// <summary>
    /// 用掉卡牌后新卡牌对该位置的补充
    /// </summary>
    /// <param name="posID">位置ID</param>
    public void UseCard(int posID)
    {
        currentDoardNum--;
        MoveCardToBoard(posID);
    }
    /// <summary>
    /// 将当前使用的卡牌ID从列表中移除
    /// </summary>
    /// <param name="id"></param>
    public void RemoveCardIDInList(int id)
    {
        cardIDList.Remove(id);
    }
    /// <summary>
    /// 游戏结束
    /// </summary>
    /// <param name="win"></param>
    public void GameOver(bool win)
    {
        DOTween.To(() => Camera.main.orthographicSize, 
            x => Camera.main.orthographicSize = x, 12.71f, 0.5f)
    .OnComplete(() => { OpenGameOverPanel(win); });
    }
    /// <summary>
    /// 打开游戏结束的界面
    /// </summary>
    /// <param name="win">输赢的状态</param>
    private void OpenGameOverPanel(bool win)
    {
        Time.timeScale = 0;
        endPanelGo.SetActive(true);
        if (win)//赢了
        {
            GameManager.Instance.PlayMusic(winClip);
            winPanelGo.SetActive(true);
        }
        else//输了
        {
            GameManager.Instance.PlayMusic(loseClip);
            losePanelGo.SetActive(true);
        }
    }
    /// <summary>
    /// 切换界面
    /// </summary>
    public void Replay()
    {
        Time.timeScale = 1;
        StopAllCoroutines();
        SceneManager.LoadScene(2);
    }

    /// <summary>
    /// 显示隐藏不可点击区域UI
    /// </summary>
    /// <param name="show"></param>
    public void ShowOrHideMaskPanel()
    {
        maskPanelGo.SetActive(true);
        Invoke("CloseMaskPanel", 0.5f);
    }

    private void CloseMaskPanel()
    {
        maskPanelGo.SetActive(false);
    }
}
