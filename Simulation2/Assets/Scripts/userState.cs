using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
public enum stateType
{
    Running,
    Resting,
    Charged,
    Charging
}

public abstract class userState
{
    public abstract stateType statettype
    {
        get;
    }

    public abstract void Enter(params object[] param);

    public abstract void Update();

    public abstract void Exit();

    protected UserController userController;
}

public class RunningState : userState
{
    public override stateType statettype
    {
        get
        {
            return stateType.Running;
        }
    }
    public override void Enter(params object[] param)
    {
        userController = param[0] as UserController;
        float Distance = Vector3.Distance(userController.runningTargets[0].position, userController.transform.position);
        float curDistance = 0;
        for (int i = 0; i < userController.runningTargets.Count; i++)
        {
            curDistance = Vector3.Distance(userController.runningTargets[i].position, userController.transform.position);
            if (Distance > curDistance)
            {
                Distance = curDistance;
                curIndex = i;
            }
        }
        userController.barTarget = userController.runningTargets[curIndex];
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        if (userController.isClosed)
        {
            curIndex++;
            curIndex %= userController.runningTargets.Count;
            userController.barTarget = userController.runningTargets[curIndex];
        }
    }

    int curIndex = 0;

}

public class RestingState : userState
{
    public override stateType statettype
    {
        get
        {
            return stateType.Resting;
        }
    }

    public override void Enter(params object[] param)
    {
        userController = param[0] as UserController;
        //原地休息
        userController.barTarget = userController.restTarget;
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        
    }
}

public class ChargedState : userState
{
    public override stateType statettype
    {
        get
        {
            return stateType.Charged;
        }
    }

    public override void Enter(params object[] param)
    {
        userController = param[0] as UserController;

        targetBar = param[1] as BarController;

        userController.barTarget = targetBar.transform;
    }

    public override void Exit()
    {

    }

    public override void Update()
    {

    }
    /// <summary>
    /// 目标充电桩
    /// </summary>
    BarController targetBar;
}

public class ChargingState : userState
{
    public override stateType statettype
    {
        get
        {
            return stateType.Charging;
        }
    }
    public override void Enter(params object[] param)
    {
        userController = param[0] as UserController;

        targetBar = param[1] as BarController;

        userController.barTarget = targetBar.transform;
    }

    public override void Exit()
    {

    }

    public override void Update()
    {

    }
    /// <summary>
    /// 目标充电桩
    /// </summary>
    BarController targetBar;
}