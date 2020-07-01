using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Pathfinding;
using UnityEditor;

public abstract class UserState
{
    /// <summary>
    /// 当前状态类型
    /// </summary>
    public abstract UserController.UserStateType StateType { get; }
    /// <summary>
    /// 进入状态
    /// </summary>
    /// <param name="param"></param>
    public abstract void Enter(params object[] param);
    /// <summary>
    /// 更新状态
    /// </summary>
    public abstract void Update();
    /// <summary>
    /// 退出状态
    /// </summary>
    public abstract void Exit();

    /**********************************************************/
    /// <summary>
    /// 进入状态的标志
    /// </summary>
    public bool isEnter { get; set; } = false;

    /// <summary>
    /// 用户控制器引用
    /// </summary>
    protected UserController userController { get; set; }
}

/// <summary>
/// 行驶状态
/// </summary>
public class UserRunState : UserState
{
    public override UserController.UserStateType StateType
    {
        get { return UserController.UserStateType.Running; }
    }

    public override void Enter(params object[] param)
    {
        isEnter = true;
        userController = param[0] as UserController;
        
        int nearistTargetIndex = 0; //  最近目标点的索引

        //最小距离
        float minDistance = Vector3.Distance(userController.transform.position, userController.runningTargets[nearistTargetIndex].position);
        //当前距离
        float curDistance;
        //寻找最小距离:
        for (int i = 0; i < userController.runningTargets.GetLength(0); i++)
        {
            curDistance = Vector3.Distance(userController.transform.position, userController.runningTargets[i].position);
            if (minDistance > curDistance)
            {
                minDistance = curDistance;
                nearistTargetIndex = i;
            }
        }
        //初始化当前目标索引
        currentIndex = nearistTargetIndex;

        userController.curTarget = userController.runningTargets[nearistTargetIndex];
    }

    public override void Exit()
    {
        isEnter = false;
    }

    public override void Update()
    {
        if (isEnter == false) return;
        //到达
        if (userController.aipath.endReachedDistance > Vector3.Distance(userController.transform.position, userController.curTarget.position))
        {
            //更新当前目标索引:
            currentIndex++;
            currentIndex %= userController.runningTargets.GetLength(0);
            //更新当前目标:
            userController.curTarget = userController.runningTargets[currentIndex];
        }
    }

    /**********************************************************/

    /// <summary>
    /// 当前目标的索引
    /// </summary>
    private int currentIndex;
}

/// <summary>
/// 贡献电量状态
/// </summary>
public class UserChargingState : UserState
{
    public override UserController.UserStateType StateType
    {
        get { return UserController.UserStateType.Charging; }
    }

    public override void Enter(params object[] param)
    {
        isEnter = true;

        userController = param[0] as UserController;

        string BarName = param[1] as string;

        if (BarName.Equals(userController.chargeBarATarget.gameObject.ToString()))
        {
            userController.curTarget = userController.chargeBarATarget;
        }
        else if (BarName.Equals(userController.chargeBarBTarget.gameObject.ToString()))
        {
            userController.curTarget = userController.chargeBarBTarget;
        }
    }

    public override void Exit()
    {
        isEnter = false;

        //确保退出状态时，用户已经与充电桩实际接触了
        if (userController.isCharge)
        {
            //发送充电结束的消息给userInfo
            userController.userInfo.chargeRecord.EndCharge(
                DayTimerController
                .curDayTime,
                userController.userInfo.userInfo.curBattery,
                userController.userInfo.simulationParam,
                0f
                );

            //改变积分
            userController.userInfo.userInfo.Currency += userController.userInfo.chargeRecord.rewards;
            //加入消息列表
            userController.userInfo.AddToMsgList();

            userController.curChargeBar = null;
        }
    }

    public override void Update()
    {
        if (isEnter == false) return;

        //如果已经进入充电区域
        if (userController.isCharge && userController.curChargeBar)
        {
            barController = userController.curChargeBar.GetComponent<ChargeBarController>();

            userController.userInfo.chargeRecord.StartCharge(DayTimerController.curDayTime,
                userController.userInfo.userInfo.curBattery,
                barController.barInfo.ID);
        }
    }
    ChargeBarController barController;
}

/// <summary>
/// 被充电状态
/// </summary>
public class UserChargedState : UserState
{
    public override UserController.UserStateType StateType
    {
        get { return UserController.UserStateType.getCharged; }
    }
    /// <summary>
    /// 需要两参数，1.userController, 2.chargeBarTargetName
    /// </summary>
    /// <param name="param"></param>
    public override void Enter(params object[] param)
    {
        isEnter = true;

        userController = param[0] as UserController;

        string BarName = param[1] as string;

        if (BarName.Equals(userController.chargeBarATarget.gameObject.ToString()))
        {
            userController.curTarget = userController.chargeBarATarget;
        }
        else if (BarName.Equals(userController.chargeBarBTarget.gameObject.ToString()))
        {
            userController.curTarget = userController.chargeBarBTarget;
        }
    }

    public override void Exit()
    {
        isEnter = false;

        float deltaPower = userController.userInfo.userInfo.curBattery - userController.userInfo.chargeRecord.startPower;

        //确保退出状态时，用户已经与充电桩实际接触了
        if (userController.isCharge)
        {
            if (deltaPower > 0) //  需要付费
            {
                //停止时间
                userController.dayTimerController.button_Pause();
                //弹窗
                //Debug.Log("弹窗");
                userController.userInfo.PayPanel.SetActive(true);

                userController.curChargeBar = null;
            }
        }
    }

    public override void Update()
    {
        if (isEnter == false) return;

        //如果已经进入充电区域
        if (userController.isCharge && userController.curChargeBar)
        {
            barController = userController.curChargeBar.GetComponent<ChargeBarController>();

            userController.userInfo.chargeRecord.StartCharge(DayTimerController.curDayTime,
                userController.userInfo.userInfo.curBattery,
                barController.barInfo.ID);
        }
    }
    ChargeBarController barController;

}

/// <summary>
/// 休息状态
/// </summary>
public class UserRestState : UserState
{
    public override UserController.UserStateType StateType
    {
        get { return UserController.UserStateType.Resting; }
    }

    public override void Enter(params object[] param)
    {
        isEnter = true;
        userController = param[0] as UserController;

        userController.curTarget = userController.parkingTarget;
    }

    public override void Exit()
    {
        isEnter = false;
    }

    public override void Update()
    {
        if (isEnter == false) return;
    }

}