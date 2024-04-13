using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： 曾嘉骏
//功能说明：游戏逻辑控制
//***************************************** 
public class GameController : MonoBehaviour
{
    public static GameController Instance;//保证了它可以通过类访问，而不是通过实例访问

    public float energyValue;
    public float leftTime;//存贮以秒为单位
    public List<UnitInfo> unitInfos;//卡牌信息结构体列表
    public GameObject[] unitGos;//所有单位的预制体（游戏物体）资源
    public Building[] purpleBuildings;//紫色方建筑
    public Building[] orangeBuildings;//橘色方建筑
    public AudioClip[] gameBGMusic;//游戏资源

    private void Awake()
    {
        Instance = this;//单例模式的写法
        energyValue = 1;
        leftTime = 180;
        unitInfos = new List<UnitInfo>()
        {
            new UnitInfo(){ id=1,unitName="精灵弓箭手",cost=3,hp=10,attackArea=4,speed=1,attackValue=2},
            new UnitInfo(){ id=2,unitName="治愈天使",cost=4,hp=10,attackArea=3,speed=1,attackValue=1},
            new UnitInfo(){ id=3,unitName="三头狼",cost=6,hp=30,attackArea=2,speed=1,attackValue=5},
            new UnitInfo(){ id=4,unitName="堕天使",cost=6,hp=10,attackArea=2.5f,speed=2,attackValue=6},
            new UnitInfo(){ id=5,unitName="熔岩巨兽",cost=4,hp=30,attackArea=2,speed=1,attackValue=4},
            new UnitInfo(){ id=6,unitName="弓箭手兄弟",cost=5,hp=10,attackArea=4,speed=1,attackValue=2},
            new UnitInfo(){ id=7,unitName="装甲熊军团",cost=7,hp=10,attackValue=8,attackArea=2,speed=4},
            new UnitInfo(){ id=8,unitName="死神",cost=6,hp=10,attackValue=7,attackArea=3,speed=2},
            new UnitInfo(){ id=9,unitName="毒瘟疫",cost=4,attackArea=1.5f,speed=1,attackValue=1,canCreateAnywhere=true},
            new UnitInfo(){ id=10,unitName="大火球",cost=4,attackArea=2f,attackValue=3,speed=18,canCreateAnywhere=true},
            new UnitInfo(){ id=11,unitName="骷髅怪",cost=0,hp=2,attackArea=1.5f,speed=1,attackValue=1},
            new UnitInfo(){ id=12,unitName="治疗光环",cost=0,attackArea=2f,attackValue=-2,speed=18},
            new UnitInfo(){ id=13,unitName="防御塔",cost=0,hp=150,attackArea=4,speed=0,attackValue=3},
        };
        GameManager.Instance.PlayMusic(gameBGMusic[Random.Range(0, 3)]);
    }
   
    void Update()
    {
        if (energyValue<10)
        {
            energyValue += Time.deltaTime;
            UIManager.Instance.SetEnergySliderValue();
        }
        DecreaseTime();
    }

    private void DecreaseTime()
    {
        leftTime -= Time.deltaTime;
        int min = (int)leftTime / 60;
        int sec = (int)leftTime % 60;
        UIManager.Instance.SetTimeValue(min,sec);
    }
    /// <summary>
    /// 判断当前这张卡牌是否可以使用
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool CanUseCard(int id)
    {
        return unitInfos[id - 1].cost <= energyValue;
    }
    /// <summary>
    /// 圣水消耗
    /// </summary>
    /// <param name="value"></param>
    public void DecreaseEnergyValue(int id)
    {
        int value = unitInfos[id - 1].cost;
        energyValue -= value;
    }
    /// <summary>
    /// 生成单位
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pos">生成位置</param>
    /// <param name="isOrange">是否属于橘色方</param>
    public void CreateUnit(int id,Vector3 pos,bool isOrange=true)
    {
        GameObject go= Instantiate(unitGos[id-1]);
        go.transform.position = pos;
        switch (id)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 8:
            case 9:
            case 11:
            case 12:
                Unit unit= go.GetComponent<Unit>();
                unit.isOrange = isOrange;
                unit.unitInfo = unitInfos[id - 1];
                break;
            case 6:
            case 7:
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    Unit u = go.transform.GetChild(i).GetComponent<Unit>();
                    u.isOrange = isOrange;
                    u.unitInfo = unitInfos[id - 1];
                }
                break;
            case 10:
                MagicFire fireball = go.GetComponent<MagicFire>();
                fireball.targetPos = pos;
                fireball.isOrange = isOrange;
                fireball.unitInfo = unitInfos[id - 1];
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 某个单位获取默认攻击目标位置
    /// </summary>
    /// <param name="unit">当前单位</param>
    /// <param name="isOrange">所属方</param>
    public void UnitGetTargetPos(Unit unit,bool isOrange)
    {
        //根据颜色拿数组
        Building[] buildings = isOrange ? purpleBuildings:orangeBuildings;
        
        if (!buildings[0])
        {
            //国王已死
            return;
        }
        //国王没有挂掉 根据当前单位x坐标跟国王坐标作对比进行判断
        int index = unit.transform.position.x 
            <= buildings[0].transform.position.x ? 1 : 2;
        //当前索引对应建筑是否已阵亡
        if (buildings[index].isDead)
        {
            //已阵亡，则把国王位置设置为默认位置
            unit.defaultTarget= buildings[0];
        }
        else
        {
            //未阵亡，则把当前目标设置为默认位置
            unit.defaultTarget = buildings[index];
        }
    }
    /// <summary>
    /// 唤醒国王
    /// </summary>
    /// <param name="isOrange"></param>
    public void EnableKing(bool isOrange)
    {
        if (isOrange)
        {
            orangeBuildings[0].SetColliders(true);
        }
        else
        {
            purpleBuildings[0].SetColliders(true);
        }
    }
}
