using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using UnityEditorInternal;

public class UserInfo : MonoBehaviour
{
    /**********************************************************/
    //用户信息

    /// <summary>
    /// 用户信息定义
    /// </summary>
    [Serializable]
    public struct Info
    {
        /// <summary>
        /// 电量信息
        /// </summary>
        [Range(0, maxBattery)]
        public int curBattery;

        public const int maxBattery = 500;

        public const int alarmBattery = 100;
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
    }
    /// <summary>
    /// 用户信息
    /// </summary>
    [Header("用户信息")]
    public Info userInfo;

    /**********************************************************/
    //充电信息

    /// <summary>
    /// 充电记录
    /// </summary>
    public struct chargeDeltaRecord
    {
        public int time;
        //电量变化
        public int pwer;
        public float balance;
        public int rewards;
    }
    /// <summary>
    /// 充电记录表
    /// </summary>
    [NonSerialized]
    public List<chargeDeltaRecord> chargedeltaRecords;

    /**********************************************************/
    //外部依赖项

    UserController userController;

    public SimulationParam simulationParam;

    /**********************************************************/

    private int curTime;

    private int deltaTime;

    private void Start()
    {
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
        }

        Update_BasedOnBattery();

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
                userInfo.curBattery = Mathf.Clamp(userInfo.curBattery - simulationParam.usePowerSpeed, 0, Info.maxBattery);

                break;
            case UserController.UserStateType.Charging:
                //给充电桩充电
                userInfo.curBattery = Mathf.Clamp(userInfo.curBattery - simulationParam.getCarPowerSpeed, 0, Info.maxBattery);

                break;
            case UserController.UserStateType.getCharged:
                //被充电
                userInfo.curBattery = Mathf.Clamp(userInfo.curBattery + simulationParam.getBarPowerSpeed, 0, Info.maxBattery);

                break;
            case UserController.UserStateType.Resting:
                //啥也不干
                break;
            default:
                break;
        }
    }


}
