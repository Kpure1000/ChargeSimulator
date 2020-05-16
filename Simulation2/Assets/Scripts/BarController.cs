using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
[Serializable]
public class BarController : MonoBehaviour
{
    /********************************/
    //外部依赖
    public SimulationParam param;

    public string ID;

    [Header("初始化电量")]
    [Range(0, maxPower)]
    public int Power;

    public float powerRate
    {
        get
        {
            return (float)Power / maxPower;
        }
    }

    public const int maxPower = 1000;

    //能够在此充电
    public bool canFindAsCharged
    {
        get
        {
            return powerRate >= param.lowBatteryAlarm;
        }
    }

    //能够在此贡献
    public bool canFindAsCharging
    {
        get
        {
            return powerRate < param.lowBatteryAlarm;
        }
    }

    //正在接触user
    public bool isConnecting;

    public Vector3 Position
    {
        get
        {
            return gameObject.transform.position;
        }
    }

    public enum barState
    {
        SunPower,
        OutPower,
        Inpower
    }

    /// <summary>
    /// 当前状态
    /// </summary>
    public barState _curstate = barState.SunPower;

    private int curTime;
    
    private int deltaTime;

    //总状态：三种：1、太阳能 2、正在给车子充电 3、正在被车子贡献

    /********************************/

    private void Start()
    {
        isConnecting = false;
    }

    private void Update()
    {

        if (curTime != DayTimerController.curTime)
        {
            curTime = DayTimerController.curTime;
            deltaTime++;
        }

        if (deltaTime >= param.unitDayTime)
        {
            deltaTime = 0;

            Update_BasedOnTime();
        }
    }
    /// <summary>
    /// 单位时间
    /// </summary>
    private void Update_BasedOnTime()
    {
        switch (_curstate)
        {
            case barState.SunPower:
                //不干啥
                break;
            case barState.OutPower:
                //给电动车充电
                Power = Mathf.Clamp(Power - param.getBarPowerSpeed, 0, maxPower);
                break;
            case barState.Inpower:
                //被电动车充电
                Power = Mathf.Clamp(Power + param.getCarPowerSpeed, 0, maxPower);
                break;
            default:
                break;
        }

        //接收太阳能
        Power = Mathf.Clamp(Power + param.getSunPowerSpeed, 0, maxPower);
    }

    [NonSerialized]
    public SimulationStatisticController simulationStatisticController;

    public void BarStatisticAdd()
    {
        simulationStatisticController.OnAddData_Each(() =>
        {
            return new SimulationStatisticController.SimulationStatisticData();
        }
        );
    }
}
