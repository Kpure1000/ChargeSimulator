using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Pathfinding;
using UnityEditor;

[RequireComponent(typeof(UserInfo))]
public class UserController : MonoBehaviour
{
    /**********************************************************/
    [NonSerialized]
    public UserInfo userInfo;

    /**********************************************************/

    /// <summary>
    /// 用户状态定义
    /// </summary>
    public enum UserStateType
    {
        Running,
        Charging,
        getCharged,
        Resting
    }
    /// <summary>
    /// 用户状态
    /// </summary>
    [Header("初始状态")]
    public UserStateType initState;

    /// <summary>
    /// 当前状态
    /// </summary>
    public UserState _curState { get; set; }

    /// <summary>
    /// 状态实例放在字典中
    /// </summary>
    public Dictionary<UserStateType, UserState> _stateDic = new Dictionary<UserStateType, UserState>();

    /**********************************************************/
    //寻路
    [Header("寻路目标设置")]

    [Tooltip("Run的目标列表")]
    public Transform[] runningTargets;

    [Tooltip("充电桩A")]
    public Transform chargeBarATarget;

    [Tooltip("充电桩B")]
    public Transform chargeBarBTarget;

    [Tooltip("停车位")]
    public Transform parkingTarget;
    /// <summary>
    /// 当前目标
    /// </summary>
    public Transform curTarget;
    ///// <summary>
    ///// 下一目标
    ///// </summary>
    //public Transform nextTarget;

    /**********************************************************/

    [Header("寻路参数设置")]
    /// <summary>
    /// 移动速度
    /// </summary>
    [Tooltip("移动速度")]
    public float moveSpeed = 5f;

    [Tooltip("转动速度")]
    public float rotateSpeed = 360f;

    [Tooltip("到达下一路径点的判断距离")]
    public float nextWayPointDistance = .75f;

    [NonSerialized]
    public AIPath aipath;

    /**********************************************************/

    private void Awake()
    {
        _stateDic[UserStateType.Running] = new UserRunState();
        _stateDic[UserStateType.getCharged] = new UserChargedState();
        _stateDic[UserStateType.Charging] = new UserChargingState();
        _stateDic[UserStateType.Resting] = new UserRestState();

        _curState = _stateDic[initState];
    }

    private void Start()
    {
        userInfo = GetComponent<UserInfo>();
        aipath = GetComponent<AIPath>();
        _curState.Enter(this);
    }

    private void Update()
    {
        _curState.Update();
    }

    /**********************************************************/

    /*寻路参数
	private Path path;
	int currentWayPoint = 0;
	bool reachedEndOfPath = false;

	Seeker seeker;
	Rigidbody rb;
	Vector3 rbForce;
	*/

    /*原本自己写的操控，废弃
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        seeker = GetComponent<Seeker>();
		InvokeRepeating("StartSearchPath", 0f, .5f);
    }

	void StartSearchPath()
	{
		if (seeker.IsDone())
		{
			seeker.StartPath(rb.position, curTarget.position, OnPathComplete);
		}
	}

	private void FixedUpdate()
	{
		PathUpdate();
	}

	void PathUpdate()
	{
		if(path == null)
		{
			return;
		}
		if(currentWayPoint >= path.vectorPath.Count)
		{
			reachedEndOfPath = true;
			return;
		}
		else
		{
			reachedEndOfPath = false;
		}

		Vector3 curDirection = path.vectorPath[currentWayPoint] - rb.position;
		rbForce = curDirection * moveSpeed;

		rb.AddForce(rbForce);

		//transform.Rotate(force - transform.rotation.eulerAngles);

		//transform.rotation = newRotation;

		float distance = Vector3.Distance(rb.position, path.vectorPath[currentWayPoint]);

		if(distance < nextWayPointDistance)
		{
			currentWayPoint++;
		}

	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(rb.position, rb.position + rbForce.normalized);
	}

	private void OnPathComplete(Path p)
	{
		if(!p.error)
		{
			path = p;
			currentWayPoint = 0;
		}
	}*/

}

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
            Vector3.Distance(userController.transform.position,userController.curTarget.position))
        {
            Debug.Log("Running:到达目标");

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
    }

    public override void Exit()
    {
        isEnter = false;
    }

    public override void Update()
    {
        if (isEnter == false) return;
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

        if(BarName.Equals(userController.chargeBarATarget.ToString()))
        {
            userController.curTarget = userController.chargeBarATarget;
        }
        else if (BarName.Equals(userController.chargeBarBTarget.ToString()))
        {
            userController.curTarget = userController.chargeBarBTarget;
        }
    }

    public override void Exit()
    {
        isEnter = false;
    }

    public override void Update()
    {
        if (isEnter == false) return;
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