using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public abstract class userState
{
    public abstract void Enter(params object[] param);

    public abstract void Update();

    public abstract void Exit();
}

public class RunningState : userState
{
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
        if(userController.isClosed)
        {
            curIndex++;
            curIndex %= userController.runningTargets.Count;
            userController.barTarget = userController.runningTargets[curIndex];
        }
    }

    int curIndex = 0;
    UserController userController;
}

public enum stateType
{
    Running,
    Resting,
    Charged,
    Charging
}
