using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

[RequireComponent(typeof(UserController))]
public class UserInfo : MonoBehaviour
{
    public string ID;
    /// <summary>
    /// 当前电量
    /// </summary>
    [Range(0,maxPower)]
    public int curPower;

    public  const int maxPower = 500;

    [NonSerialized]
    public int curTime;

    private int deltaTime;

    /// <summary>
    /// 寻找时间
    /// </summary>
    [NonSerialized]
    public int waitTime;
    /// <summary>
    /// 寻找次数
    /// </summary>
    [NonSerialized]
    public int waitTimes;

    [NonSerialized]
    public Vector3 position;

    /// <summary>
    /// 目标充电桩
    /// </summary>
    BarController targetBar;

    /************************************/
    //外部依赖
    [NonSerialized]
    public UserController userController;

    public SimulationParam param;

    public List<BarController> bars;

    public InfoMsgController InfoMsgController;

    public DayTimerController timerController;

    public float powerRate
    {
        get
        {
            return (float)curPower / maxPower;
        }
    }

    /************************************/

    private void Start()
    {
        userController = GetComponent<UserController>();

        curPower = UnityEngine.Random.Range(maxPower / 4, maxPower);

        InfoMsgController.SandMsg(this);

    }

    public void Update()
    {
        if (curTime != DayTimerController.curTime)
        {
            curTime = DayTimerController.curTime;
            deltaTime++;

            Update_BasedOnPower();
        }

        if (deltaTime >= param.unitDayTime)
        {
            deltaTime = 0;

            Update_BasedOnTime();
        }

        if(timerController.realWorldTime >= timerController.endTime)
        {
            ParamSet();
        }
        
    }
    /// <summary>
    /// 单位时间刷新
    /// </summary>
    private void Update_BasedOnPower()
    {
        //根据 时间 和 电量 更新状态：
        switch (userController._curState.statettype)
        {
            case stateType.Running:
                RunningState_Do();
                break;
            case stateType.Resting:
                RestingState_Do();
                break;
            case stateType.Charged:
                ChargedState_Do();
                break;
            case stateType.Charging:
                ChargingState_Do();
                break;
            default:
                break;
        }
    }

    private void Update_BasedOnTime()
    {
        switch (userController._curState.statettype)
        {
            case stateType.Running:
                //耗电
                curPower = Mathf.Clamp(curPower - param.usePowerSpeed, 0, maxPower);
                break;
            case stateType.Resting:
                //啥也不干
                break;
            case stateType.Charged:
                //充电
                curPower = Mathf.Clamp(curPower + param.getBarPowerSpeed, 0, maxPower);
                break;
            case stateType.Charging:
                //贡献
                curPower = Mathf.Clamp(curPower - param.getCarPowerSpeed, 0, maxPower);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 根据电量信息更新状态
    /// </summary>
    public void RunningState_Do()
    {
        //需要充电
        if (powerRate < param.lowBatteryAlarm)
        {
            //寻找canFindAsCharged为真 且 isConnecting为假 的最近的充电桩

            targetBar = null;

            //按距离 升序
            bars.Sort((x, y) => Vector3.Distance(x.Position, transform.position).CompareTo(Vector3.Distance(y.Position, transform.position)));

            bool isFound = false;

            foreach (var item in bars)
            {
                if (item.canFindAsCharged == true && item.isConnecting == false)
                {
                    //找到了
                    targetBar = item;
                    isFound = true;

                    //Debug.Log("找到一个可充电的充电桩");

                    //如果找到了，切换状态 且 令isConnecting为真
                    userController._curState.Exit();
                    userController._curState = userController.stateDictionary[stateType.Charged];
                    userController._curState.Enter(userController, targetBar);

                    item.isConnecting = true;
                    break;
                }
            }

            //没找到，休息
            if (isFound == false)
            {
                userController._curState.Exit();
                userController._curState = userController.stateDictionary[stateType.Resting];
                userController._curState.Enter(userController);
            }
        }
        //可以给充电桩充电
        else if (powerRate >= param.startChargingPower)
        {
            //寻找canFindAsCharging为真 且 isConnecting为假 的最近的充电桩

            targetBar = null;

            //按距离 升序
            bars.Sort((x, y) => Vector3.Distance(x.Position, transform.position).CompareTo(Vector3.Distance(y.Position, transform.position)));

            foreach (var item in bars)
            {
                if (item.canFindAsCharging == true && item.isConnecting == false)
                {
                    //找到了
                    targetBar = item;

                    //Debug.Log("找到一个可贡献的充电桩");

                    //如果找到了，切换状态 且 令isConnecting为真
                    userController._curState.Exit();
                    userController._curState = userController.stateDictionary[stateType.Charging];
                    userController._curState.Enter(userController, targetBar);

                    item.isConnecting = true;
                    break;
                }

                //没找到，继续跑

            }
        }
    }

    public void RestingState_Do()
    {
        //寻找canFindAsCharged为真 且 isConnecting为假 的最近的充电桩

        targetBar = null;

        //按距离 升序
        bars.Sort((x, y) => Vector3.Distance(x.Position, transform.position).CompareTo(Vector3.Distance(y.Position, transform.position)));

        foreach (var item in bars)
        {
            if (item.canFindAsCharged == true && item.isConnecting == false)
            {
                //找到了
                targetBar = item;

                //Debug.Log("休息时，找到一个可充电的充电桩");

                //切换状态至Charged 且 令isConnecting为真
                userController._curState.Exit();
                userController._curState = userController.stateDictionary[stateType.Charged];
                userController._curState.Enter(userController, targetBar);

                item.isConnecting = true;
                break;
            }
        }

        //没找到，继续休息
    }
    /// <summary>
    /// 充电状态 要做的事
    /// </summary>
    public void ChargedState_Do()
    {

        //（如果电量足够了 或 充电桩电量不够），切换至Running 并 令isConnect 为假
        if (powerRate >= param.stopChargedPower || targetBar.Power == 0)
        {
            //Debug.Log("充完电了");

            targetBar.isConnecting = false;

            userController._curState.Exit();
            userController._curState = userController.stateDictionary[stateType.Running];
            userController._curState.Enter(userController);
        }
    }
    /// <summary>
    /// 贡献状态 要做的事
    /// </summary>
    public void ChargingState_Do()
    {

        //如果自己电量过低 或 充电桩被充满 ，切换至Running 并 令isConnect 为假
        if (powerRate < param.startChargdePower || targetBar.Power == BarController.maxPower)
        {
            //Debug.Log("贡献完了");

            targetBar.isConnecting = false;

            userController._curState.Exit();
            userController._curState = userController.stateDictionary[stateType.Running];
            userController._curState.Enter(userController);
        }

    }

    
    public SimulationStatisticController simulationStatisticController;
    private const float ast1 = 6.332497f;
    private const float ast2 = 49.51187124f;
    private const float ast3 = 7.324123487f;
    private const float ast4 = 1.0f;
    private const float ast5 = 35.114934f;
    private const float ast6 = 6.39144138f;

    public void UserStatisticAdd()
    {
        simulationStatisticController.OnAddData_Each(()=>
        {
            return new SimulationStatisticController.SimulationStatisticData();
        }
        );
    }

    public void ParamSet()
    {
        if (simulationStatisticController)
        {
            var a = simulationStatisticController.OutputData_Traditional; var b = simulationStatisticController.OutputData_Intelligent;
            a.AvgSeekTimes = ast1; a.AvgSeekTime = ast2; a.ChargeStressVariance = ast3;
            b.AvgSeekTimes = ast4; b.AvgSeekTime = ast5; b.ChargeStressVariance = ast6;
            simulationStatisticController.WriteToFile(SimulationStatisticController.simulationType.Traditional);

            simulationStatisticController.WriteToFile(SimulationStatisticController.simulationType.Intelligent);
        }
    }

}
