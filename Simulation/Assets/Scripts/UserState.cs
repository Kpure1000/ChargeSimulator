using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Pathfinding;
using UnityEditor;

public abstract class UserState
{
    public abstract UserController.UserStateType StateType { get; }

    public abstract void Enter(params object[] param);

    public abstract void Update();

    public abstract void Exit();

    public bool isEnter = false;
}

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

        int nearistTargetIndex = 0;
        float minDistance = Vector3.Distance(userController.transform.position, userController.runningTargets[nearistTargetIndex].position);
        float curDistance;
        for (int i = 0; i < userController.runningTargets.GetLength(0); i++)
        {
            curDistance = Vector3.Distance(userController.transform.position, userController.runningTargets[i].position);
            if (minDistance > curDistance)
            {
                minDistance = curDistance;

                nearistTargetIndex = i;
            }
        }
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
        if (userController.aipath.endReachedDistance >
            Vector3.Distance(userController.transform.position, userController.curTarget.position))
        {
            currentIndex++;
            currentIndex %= userController.runningTargets.GetLength(0);
            userController.curTarget = userController.runningTargets[currentIndex];
        }
    }

    /**********************************************************/

    private UserController userController;

    private int currentIndex;
}

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

        if(userController.isCharge)
        {
            //发送消息给userInfo
            userController.userInfo.chargeRecord.EndCharge(DayTimerController.curDayTime,
                    userController.userInfo.userInfo.curBattery, userController.userInfo.simulationParam);
            //改变积分
            userController.userInfo.userInfo.Currency += userController.userInfo.chargeRecord.rewards;
            //加入消息列表
            userController.userInfo.AddToMsgList();
        }
    }

    public override void Update()
    {
        if (isEnter == false) return;

        //如果已经进入充电区域
        if(userController.isCharge)
        {
            userController.userInfo.chargeRecord.StartCharge(DayTimerController.curDayTime, 
                userController.userInfo.userInfo.curBattery);
        }
    }

    /**********************************************************/

    private UserController userController;

}

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

        if (userController.isCharge)
        {
            //是否付费
            //停止时间
            userController.dayTimerController.button_Pause();
            //弹窗
            userController.userInfo.PayPanel.SetActive(true);

            //发送消息给userInfo
            userController.userInfo.chargeRecord.EndCharge(DayTimerController.curDayTime,
                    userController.userInfo.userInfo.curBattery, userController.userInfo.simulationParam);
            //改变余额
            userController.userInfo.userInfo.Balance += userController.userInfo.chargeRecord.balance;
            //加入消息列表
            userController.userInfo.AddToMsgList();
        }
    }

    public override void Update()
    {
        if (isEnter == false) return;

        //如果已经进入充电区域
        if (userController.isCharge)
        {
            userController.userInfo.chargeRecord.StartCharge(DayTimerController.curDayTime,
                userController.userInfo.userInfo.curBattery);
        }
    }

    /**********************************************************/

    UserController userController;


}
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

    UserController userController;

}