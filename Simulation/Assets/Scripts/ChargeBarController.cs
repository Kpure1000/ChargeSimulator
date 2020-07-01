using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Threading;

public class ChargeBarController : MonoBehaviour
{
    /**********************************************************/

    public enum barStateType
    {
        Resting,
        Charging,
        Charged
    }

    barStateType _curstate;

    /**********************************************************/
    //信息
    /// <summary>
    /// 充电桩信息
    /// </summary>
    [Serializable]
    public class Info
    {
        /// <summary>
        /// 充电桩ID
        /// </summary>
        [Header("充电桩ID")]
        public string ID;

        /// <summary>
        /// 用户的信息
        /// </summary>
        [Header("用户")]
        public GameObject userBelong = null;

        public bool isPublic;

        /// <summary>
        /// 初始电量信息
        /// </summary>
        [Range(0, maxBattery)]
        public int curBattery;

        public const int maxBattery = 1000;
        Color m_curColor;
        public Color curColor
        {
            get
            {
                if (curBattery < maxBattery / 2)
                {
                    m_curColor.r = 1f;
                    m_curColor.g = (float)(curBattery) / (maxBattery / 2);
                }
                else
                {
                    m_curColor.r = (float)(maxBattery - curBattery) / (maxBattery / 2);
                    m_curColor.g = 1f;
                }
                m_curColor.b = 0f;
                m_curColor.a = 1f;
                return m_curColor;
            }
        }

    }

    public Info barInfo;

    /**********************************************************/
    //充电记录

    /// <summary>
    /// 充电记录表
    /// </summary>
    [NonSerialized]
    public List<UserInfo.chargeDeltaRecord> chargedeltaRecords;
    private int curTime;
    private int deltaTime;
    /// <summary>
    /// 公有参数Asset
    /// </summary>
    [Header("参数资源")]
    [Tooltip("创建方式：AssetMenu->NewSimulationParamAsset")]
    public SimulationParam simulationParam;

    /**********************************************************/
    //外部依赖
    /// <summary>
    /// 电量信息
    /// </summary>
    [Header("电量信息UI（Text）")]
    public Text bettaryInfoText;
    /// <summary>
    /// 电量状态图片
    /// </summary>
    [Header("电量状态UI（Image）")]
    public Image bettaryStateImage;
    /// <summary>
    /// 消息
    /// </summary>
    [Header("消息Text")]
    public Text msgText;
    /// <summary>
    /// 用户充电币
    /// </summary>
    [Header("用户充电币Text")]
    public Text userCurrencyText;

    [Range(0, 23)]
    public int SunPowerStartTime;
    [Range(0, 23)]
    public int SunPowerEndTime;

    public SpriteRenderer sprite_Bar;

    public UserController userController;


    /**********************************************************/

    private void Start()
    {
        _curstate = barStateType.Charging;

        SunPowerStartTime = Mathf.Clamp(SunPowerStartTime, 0, SunPowerEndTime);

    }

    private void FixedUpdate()
    {

        stateUpdate();

        if (curTime != DayTimerController.curTime)
        {
            curTime = DayTimerController.curTime;
            deltaTime++;
        }

        if (deltaTime >= simulationParam.unitDayTime)
        {
            deltaTime = 0;
            Update_BasedOnTime();

#if UNITY_EDITOR
            //Debug.Log(string.Format("{0} 电量：{1}", gameObject.name, barInfo.curBattery.ToString()));
#endif

        }

        UIUpdate();

    }

    private void Update_BasedOnTime()
    {
        if (DayTimerController.curDayTime.x >= SunPowerStartTime && DayTimerController.curDayTime.x < SunPowerEndTime)
        {
            barInfo.curBattery = Mathf.Clamp(barInfo.curBattery + simulationParam.getSunPowerSpeed, 0, Info.maxBattery);
        }
        switch (_curstate)
        {
            case barStateType.Resting:
                //Do Nothing
                break;
            case barStateType.Charging:
                barInfo.curBattery = Mathf.Clamp(barInfo.curBattery - simulationParam.getBarPowerSpeed, 0, Info.maxBattery);
                break;
            case barStateType.Charged:
                barInfo.curBattery = Mathf.Clamp(barInfo.curBattery + simulationParam.getCarPowerSpeed, 0, Info.maxBattery);
                break;
            default:
                break;
        }
    }

    void stateUpdate()
    {
        //如果用户在充电
        if (userController.isCharge && userController._curState.StateType == UserController.UserStateType.Charging)
        {
            //在给自己充电
            if (userController.curTarget.Equals(transform))
            {
                if (userController.userInfo.userInfo.curBattery > 0)
                    _curstate = barStateType.Charged;
                else
                    _curstate = barStateType.Resting;
            }
        }
        //如果用户在被充电
        else if (userController.isCharge && userController._curState.StateType == UserController.UserStateType.getCharged)
        {
            //在被自己充电
            if (userController.curTarget.Equals(transform))
            {
                if (userController.userInfo.userInfo.curBattery < UserInfo.Info.maxBattery)
                    _curstate = barStateType.Charging;
                else
                    _curstate = barStateType.Resting;
            }
        }
        //没有与用户相连
        else
        {
            _curstate = barStateType.Resting;
        }
    }

    void UIUpdate()
    {
        //电量信息更新：
        bettaryInfoText.text = string.Format("电量: {0}/{1} ({2:P})",
            barInfo.curBattery,
            Info.maxBattery,
            (float)barInfo.curBattery / Info.maxBattery
            );

        //电量状态颜色更新：
        bettaryStateImage.color = barInfo.curColor;
        sprite_Bar.color = barInfo.curColor;

        //所属用户充电币更新
        if (barInfo.isPublic == false)
        {
            userCurrencyText.text = barInfo.userBelong.GetComponent<UserInfo>().currencyText.text;
        }
        else
        {
            userCurrencyText.text = "充电币: -";
        }

        //消息通知更新：

        if (barInfo.userBelong != null)
        {
            msgText.text = barInfo.userBelong.GetComponent<UserInfo>().msgText.text;
        }

    }

}
