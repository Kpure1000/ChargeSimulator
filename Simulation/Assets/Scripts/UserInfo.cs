using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

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
        /// 电量信息
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
        public float Currency;

        /// <summary>
        /// 失信记录
        /// </summary>
        [Tooltip("失信记录")]
        public int CreditRecord = 0;

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
    [Serializable]
    public class chargeDeltaRecord
    {
        public Vector2 startTime;
        public Vector2 endTime;
        public int startPower;
        public int endPower;
        public float balance;
        public float realPay;
        public float rewards;

        /// <summary>
        /// 电量变化
        /// </summary>
        public float deltaPower
        {
            get
            {
                return endPower - startPower;
            }
        }

        public bool isStart;
        public bool isEnd;
        public string targetBar;

        public chargeDeltaRecord()
        {
            startTime = Vector2.zero;
            endTime = Vector2.zero;
            startPower = 0;
            endPower = 0;
            balance = 0f;
            realPay = 0f;
            rewards = 0f;
            targetBar = "";

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
            realPay = record.realPay;
            rewards = record.rewards;
            targetBar = record.targetBar;

            isStart = false;
            isEnd = false;
        }

        public void StartCharge(Vector2 starttime, int startpower, string targetbar)
        {
            if (isStart) return;
            startTime = starttime;
            startPower = startpower;
            targetBar = targetbar;

            isStart = true;
        }

        public void EndCharge(Vector2 endtime, int endpower, SimulationParam simulation, float realpay)
        {
            if (isEnd) return;
            endTime = endtime;
            endPower = endpower;
            realPay = realpay;
            isEnd = true;

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

    /// <summary>
    /// 充电记录缓冲区
    /// </summary>
    [NonSerialized]
    public chargeDeltaRecord chargeRecord = new chargeDeltaRecord();

    public void AddToMsgList()
    {
        if (chargeRecord.isEnd)
        {
            chargedeltaRecords.Add(new chargeDeltaRecord(chargeRecord));
            chargeRecord.isEnd = false; chargeRecord.isStart = false;

            //发送消息给企业和政府
            companyMsg.SandMsg(chargeRecord);
            governmentMsg.SandMsg(chargeRecord);
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
    [Header("外部依赖")]
    [Tooltip("创建方式：AssetMenu->NewSimulationParamAsset")]
    public SimulationParam simulationParam;

    public ScrollMsgController companyMsg;

    public ScrollMsgController governmentMsg;

    /**********************************************************/
    //UI 依赖项
    public Image stateImage;

    public Text balanceText;

    public Text powerText;

    public Text currencyText;

    public Text creditText;

    public Text curStateText;

    public Text msgText;

    [Tooltip("支付界面")]
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

    private void Start()
    {
        PayPanel.SetActive(false);

        msgText.text = "\r\n\r\n...";

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
            //Debug.Log("电量：" + userInfo.curBattery.ToString());
#endif

        }

        //更新电量信息
        //Update_BasedOnBattery();

        //更新UI
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
                else if (userController.isCharge == false) //  还没到达充电桩
                    userInfo.curBattery = Mathf.Clamp(userInfo.curBattery - simulationParam.usePowerSpeed, 0, Info.maxBattery);

                break;
            case UserController.UserStateType.getCharged:
                //被充电
                if (userController.isCharge == true)
                    userInfo.curBattery = Mathf.Clamp(userInfo.curBattery + simulationParam.getBarPowerSpeed, 0, Info.maxBattery);
                else if (userController.isCharge == false) //  还没到达充电桩
                    userInfo.curBattery = Mathf.Clamp(userInfo.curBattery - simulationParam.usePowerSpeed, 0, Info.maxBattery);

                break;
            case UserController.UserStateType.Resting:
                //耗电
                if (userController.isPark == false)
                    userInfo.curBattery = Mathf.Clamp(userInfo.curBattery - simulationParam.usePowerSpeed, 0, Info.maxBattery);

                break;
            default:
                break;
        }
    }

    public enum PayState
    {
        Pay,
        UnPay
    }

    public PayState payState { get; set; } = PayState.UnPay;

    public void PayStateController(int key)
    {
        payState = (PayState)key;

        if (payState == PayState.Pay)
        {
            userInfo.Balance += userController.userInfo.chargeRecord.balance; //  改变余额
            //发送消息给userInfo
            chargeRecord.EndCharge(DayTimerController.curDayTime,
                    userInfo.curBattery, simulationParam, userController.userInfo.chargeRecord.balance);
        }
        else if (payState == PayState.UnPay) 
        {
            //失信记录增加
            userInfo.CreditRecord++;
            //惩罚
            userInfo.Currency *= simulationParam.creditPublishRate;

            //发送消息给userInfo
            chargeRecord.EndCharge(DayTimerController.curDayTime,
                    userInfo.curBattery, simulationParam, 0f);
        }
        //加入消息列表
        userController.userInfo.AddToMsgList();

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


        string stateString = "";
        switch (userController._curState.StateType)
        {
            case UserController.UserStateType.Running:
                stateString = "行驶";
                break;
            case UserController.UserStateType.Charging:
                if (userController.curChargeBar)
                    stateString = "给 " + userController.curChargeBar.GetComponent<ChargeBarController>().barInfo.ID + " 充电";
                break;
            case UserController.UserStateType.getCharged:
                if (userController.curChargeBar)
                    stateString = "在 " + userController.curChargeBar.GetComponent<ChargeBarController>().barInfo.ID + " 处，充电";
                break;
            case UserController.UserStateType.Resting:
                stateString = "停泊休息";
                break;
            default:
                break;
        }
        curStateText.text = "当前行为: " + stateString;

        creditText.text = string.Format("失信记录: {0}", userInfo.CreditRecord);

        //消息列表显示

        string msgString = "";

        int curIndex;

        //获取消息列表顶端的两个消息
        for (int i = 0; i < 2; i++)
        {
            curIndex = chargedeltaRecords.Count - 1 - i;
            
            if (curIndex >= 0) //  没有越界
            {
                //给充电桩充电的记录
                if (chargedeltaRecords[curIndex].deltaPower < 0)
                {
                    msgString +=
                    string.Format("{0}:{1}: U0001->{2}-pow:{3}-rew:{4}\r\n",
                    chargedeltaRecords[curIndex].endTime.x,
                    chargedeltaRecords[curIndex].endTime.y,
                    chargedeltaRecords[curIndex].targetBar,
                    chargedeltaRecords[curIndex].deltaPower,
                    chargedeltaRecords[curIndex].rewards);
                }
                //被充电的记录
                else if (chargedeltaRecords[curIndex].deltaPower > 0)
                {
                    msgString +=
                    string.Format("{0}:{1}: {2}->U0001-pow:{3}-pay:{4}\r\n",
                    chargedeltaRecords[curIndex].endTime.x,
                    chargedeltaRecords[curIndex].endTime.y,
                    chargedeltaRecords[curIndex].targetBar,
                    chargedeltaRecords[curIndex].deltaPower,
                    chargedeltaRecords[curIndex].balance);
                }
            }
            else //  越界
            {
                msgString += " \r\n";
            }
        }

        msgString += "...";

        msgText.text = msgString;

    }


}
