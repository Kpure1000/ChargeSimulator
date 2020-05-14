using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using UnityEditorInternal;
using UnityEngine.UI;
using JetBrains.Annotations;

public class UserInfo : MonoBehaviour
{
    /**********************************************************/
    //用户信息

    /// <summary>
    /// 用户信息定义
    /// </summary>
    [Serializable]
    public class Info
    {
        /// <summary>
        /// 初始电量信息
        /// </summary>
        [Range(0, maxBattery)]
        public int curBattery;

        public const int maxBattery = 500;

        /// <summary>
        /// 余额
        /// </summary>
        [Tooltip("余额")]
        public float Balance;
        /// <summary>
        /// 充电币
        /// </summary>
        [Tooltip("充电币")]
        public int Currency;

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
    /// <summary>
    /// 用户信息
    /// </summary>
    [Header("用户信息")]
    public Info userInfo;

    /**********************************************************/
    //充电记录

    /// <summary>
    /// 充电记录
    /// </summary>
    public class chargeDeltaRecord
    {
        public Vector2 startTime;
        public Vector2 endTime;
        public int startPower;
        public int endPower;
        public float balance;
        public int rewards;

        public bool isStart;
        public bool isEnd;

        public chargeDeltaRecord()
        {
            startTime = Vector2.zero;
            endTime = Vector2.zero;
            startPower = 0;
            endPower = 0;
            balance = 0f;
            rewards = 0;
            isStart = false;
            isEnd = false;
        }

        public chargeDeltaRecord(chargeDeltaRecord record)
        {
            startTime = new Vector2(record.startTime.x, record.startTime.y);
            endTime = new Vector2(record.endTime.x, record.endTime.y);
            startPower = record.startPower;
            endPower = record.endPower;
            balance = record.balance;
            rewards = record.rewards;
            isStart = false;
            isEnd = false;
        }

        public void StartCharge(Vector2 starttime,int startpower)
        {
            if (isStart) return;
            startTime = starttime;
            startPower = startpower;
            isStart = true;
        }

        public void EndCharge(Vector2 endtime,int endpower, SimulationParam simulation)
        {
            if (isEnd) return;
            endTime = endtime;
            endPower = endpower;
            isEnd = true;

            int deltaPower = endPower - startPower;

            //获取积分
            if (deltaPower < 0)
            {
                rewards = -deltaPower / simulation.chargeRewardSpeed;
            }
            //扣费
            else if (deltaPower > 0)
            {
                balance = -deltaPower / simulation.chargeCostSpeed;
            }
        }

    }
    /// <summary>
    /// 充电记录表
    /// </summary>
    [NonSerialized]
    public List<chargeDeltaRecord> chargedeltaRecords = new List<chargeDeltaRecord>();

    [NonSerialized]
    public chargeDeltaRecord chargeRecord = new chargeDeltaRecord();

    public void AddToMsgList()
    {
        if(chargeRecord.isEnd)
        {
            chargedeltaRecords.Add(new chargeDeltaRecord(chargeRecord));
            chargeRecord.isEnd = false;chargeRecord.isStart = false;
        }
    }

    /**********************************************************/
    //外部依赖项
    /// <summary>
    /// 本对象的controller组件
    /// </summary>
    UserController userController;
    /// <summary>
    /// 公有参数Asset
    /// </summary>
    [Header("参数资源")]
    [Tooltip("创建方式：AssetMenu->NewSimulationParamAsset")]
    public SimulationParam simulationParam;

    /**********************************************************/
    //UI 依赖项
    public Image stateImage;

    public Text balanceText;

    public Text powerText;

    public Text currencyText;

    public Text creditText;

    public Text msgText;

    public GameObject PayPanel;

    /**********************************************************/
    /// <summary>
    /// 目前时间，用于同步时间戳
    /// </summary>
    private int curTime;
    /// <summary>
    /// 变化时间，用于获取单位时间变化
    /// </summary>
    private int deltaTime;

    /// <summary>
    /// 之前一次更新的电量
    /// </summary>
    private int preBattery;

    /// <summary>
    /// 开始记录的电量
    /// </summary>
    private int startBattery;

    private void Start()
    {
        payState = PayState.Unknow;
        userController = GetComponent<UserController>();
    }

    private void FixedUpdate()
    {
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
            Debug.Log("电量：" + userInfo.curBattery.ToString());
#endif

        }

        if(preBattery != userInfo.curBattery)
        {
            startBattery += Mathf.Abs(userInfo.curBattery - preBattery);
        }

        Update_BasedOnBattery();

        UpdateUserUI();

    }
    /// <summary>
    /// 基于电量戳更新
    /// </summary>
    void Update_BasedOnBattery()
    {
        switch (userController._curState.StateType)
        {
            case UserController.UserStateType.Running:
                break;
            case UserController.UserStateType.Charging:
                break;
            case UserController.UserStateType.getCharged:
                if(userController.isCharge)
                {
                    chargeRecord.StartCharge(DayTimerController.curDayTime, userInfo.curBattery);
                }
                break;
            case UserController.UserStateType.Resting:
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 基于时间更新
    /// </summary>
    void Update_BasedOnTime()
    {
        switch (userController._curState.StateType)
        {
            case UserController.UserStateType.Running:
                //耗电
                if (userController.isCharge == false)
                    userInfo.curBattery = Mathf.Clamp(userInfo.curBattery - simulationParam.usePowerSpeed, 0, Info.maxBattery);

                break;
            case UserController.UserStateType.Charging:
                //给充电桩充电
                if (userController.isCharge == true)
                    userInfo.curBattery = Mathf.Clamp(userInfo.curBattery - simulationParam.getCarPowerSpeed, 0, Info.maxBattery);
                else if(userController.isCharge == false)
                    userInfo.curBattery = Mathf.Clamp(userInfo.curBattery - simulationParam.usePowerSpeed, 0, Info.maxBattery);

                break;
            case UserController.UserStateType.getCharged:
                //被充电
                if (userController.isCharge == true)
                    userInfo.curBattery = Mathf.Clamp(userInfo.curBattery + simulationParam.getBarPowerSpeed, 0, Info.maxBattery);
                else if (userController.isCharge == false)
                    userInfo.curBattery = Mathf.Clamp(userInfo.curBattery - simulationParam.usePowerSpeed, 0, Info.maxBattery);

                break;
            case UserController.UserStateType.Resting:
                //啥也不干
                break;
            default:
                break;
        }
    }

    public enum PayState
    {
        Unknow,
        Pay,
        UnPay
    }

    public PayState payState;

    public void PayStateController(int key)
    {
        payState = (PayState)key;
        userController.dayTimerController.button_Normal();
    }

    public void UpdateUserUI()
    {
        stateImage.color = userInfo.curColor;

        balanceText.text = string.Format("余额: {0}", userInfo.Balance);

        powerText.text = string.Format("电量: {0}/{1} ({2:P})",
            userInfo.curBattery,
            Info.maxBattery,
            (float)userInfo.curBattery / Info.maxBattery
            );

        currencyText.text = string.Format("充电币: {0}", userInfo.Currency);

        creditText.text = string.Format("失信记录: 0");

        //string msgString = "";

        //for(int i=0;i<2;i++)
        //{
        //    msgString += chargedeltaRecords[chargedeltaRecords.Count - 1 - i].ToString();
        //}

    }


}
