using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//*****************************************
//创建人： Trigger 
//功能说明：卡牌UI
//***************************************** 
public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int id;
    public Button button;//卡牌按钮
    private Vector3 initPos;
    private Tween tween;
    private bool isDraging;
    private bool showCharacter;
    public GameObject characterShowGo;//人物展示模型游戏物体
    private Camera cam;
    public Text cardText;//卡牌名字
    public GameObject magicCircleGo;//法术释放范围显示游戏物体
    public Transform imgEnergyT;//圣水图片显示UI的游戏物体
    public int posID;
    public GameObject[] modelGos;//当前卡牌使用前，想要生成在哪里的模型数组
    public bool canCreateAnywhere;
    public AudioClip useCardSound;
    void Start()
    {
        cam = Camera.main;
        for (int i = 0; i < modelGos.Length; i++)
        {
            modelGos[i].SetActive(false);
        }
        if (id<=8)
        {
            modelGos[id - 1].SetActive(true);
        }
        magicCircleGo.SetActive(false);
        canCreateAnywhere = GameController.Instance.unitInfos[id - 1].canCreateAnywhere;
    }

    void Update()
    {
        button.interactable = GameController.Instance.CanUseCard(id);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable)
        {
            return;
        }
        if (!isDraging)
        {
            tween = transform.DOLocalMove(initPos + new Vector3(0, 50, 0), 0.1f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!button.interactable)
        {
            return;
        }
        if (!isDraging)
        {
            tween.Pause();
            transform.localPosition = initPos;
        }
    }

    public void SetInitPos()
    {
        initPos = transform.localPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!button.interactable)
        {
            return;
        }
        tween.Pause();
        isDraging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!button.interactable)
        {
            return;
        }
        Vector2 cardPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle
            (transform.parent.GetComponent<RectTransform>(),
            Input.mousePosition, null, out cardPos);
        transform.localPosition = cardPos;
        if (showCharacter)//显示模型
        {
            float scale = Mathf.Clamp(((transform.localPosition.y - initPos.y) - 200) / 200, 0, 1);
            characterShowGo.transform.position = ScreenPointToWorldPoint(transform.position, 14.46f);
            characterShowGo.transform.localScale = Vector3.one * scale;
            if (characterShowGo.transform.localScale.x <= 0)//即将变为显示卡牌
            {
                showCharacter = false;
                button.gameObject.SetActive(true);
                characterShowGo.SetActive(false);
                if (id > 8)
                {
                    magicCircleGo.SetActive(false);
                }
                cardText.gameObject.SetActive(false);
            }
        }
        else//显示卡牌
        {
            float scale = Mathf.Clamp((200 - (transform.localPosition.y - initPos.y)) / 200, 0, 1);
            button.transform.localScale = Vector3.one * scale;
            if (button.transform.localScale.x <= 0)//即将变为显示模型
            {
                showCharacter = true;
                button.gameObject.SetActive(false);
                characterShowGo.SetActive(true);
                if (id > 8)
                {
                    magicCircleGo.SetActive(true);
                }
                cardText.gameObject.SetActive(true);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!button.interactable)
        {
            return;
        }
        button.transform.localScale = Vector3.one;
        if (showCharacter)//模型状态
        {
            imgEnergyT.gameObject.SetActive(true);
            //射线检测
            Ray ray= cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits= Physics.RaycastAll(ray);
            //用来检测当前使用卡牌是否在卡使用范围内
            if (!canCreateAnywhere && hits.Length > 0 && JudgeIfCantClick(hits))
            {               
                ReturnToInitPos();
                UIManager.Instance.ShowOrHideMaskPanel();
                return;
            }
            GameManager.Instance.PlaySound(useCardSound);
            imgEnergyT.DOLocalMove(imgEnergyT.localPosition+
                new Vector3(0, 50, 0), 0.5f).OnComplete(
                () => { UseCurrentCard(hits); });
        }
        else//卡牌状态
        {
            ReturnToInitPos();
        }
    }
    /// <summary>
    /// 是否可以放置单位的判定
    /// </summary>
    private bool JudgeIfCantClick(RaycastHit[] hits)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.tag == "CantClick")
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 屏幕坐标转换为世界坐标
    /// </summary>
    /// <param name="screenPoint">屏幕坐标上的点</param>
    /// <param name="planeZ">距离摄像机Z平面的距离</param>
    /// <returns></returns>
    private Vector3 ScreenPointToWorldPoint(Vector2 screenPoint, float planeZ)
    {
        return cam.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, planeZ));
    }
    /// <summary>
    /// 不使用卡牌返回初始位置
    /// </summary>
    private void ReturnToInitPos()
    {
        characterShowGo.SetActive(false);
        button.gameObject.SetActive(true);
        cardText.gameObject.SetActive(false);
        imgEnergyT.gameObject.SetActive(false);
        transform.DOLocalMove(initPos, 0.2f).
        OnComplete(() => { isDraging = false; });
    }
    /// <summary>
    /// 使用当前卡牌
    /// </summary>
    private void UseCurrentCard(RaycastHit[] hits)
    {
        //消耗圣水
        GameController.Instance.DecreaseEnergyValue(id);
        //循环遍历射线检测到的所有碰撞器里有没有地面
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.collider!=null&&hit.collider.tag=="Plane")
            {
                Vector3 targetPos= hit.point;
                //如果有，则生成当前卡牌对应ID的单位
                GameController.Instance.CreateUnit(id, targetPos);
                //用掉这个卡牌后当前位置为空，则需要新卡牌补上
                UIManager.Instance.UseCard(posID);
                //使用卡牌后的后续工作，比如销毁卡牌
                UIManager.Instance.RemoveCardIDInList(id);
                Destroy(gameObject);
            }
        }
    }
}
