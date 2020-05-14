using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Pathfinding;
using UnityEditor;
using UnityEditor.Animations;

[RequireComponent(typeof(UserInfo))]
public class UserController : MonoBehaviour
{
    /**********************************************************/
    /// <summary>
    /// 用户信息接入
    /// </summary>
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

    //其他变量
    /// <summary>
    /// 是否真正开始充电
    /// </summary>
    [NonSerialized]
    public bool isCharge;

    /// <summary>
    /// 状态实例放在字典中
    /// </summary>
    public Dictionary<UserStateType, UserState> _stateDic = new Dictionary<UserStateType, UserState>();

    public void StateController(int StateKey)
    {
        if (_stateDic.ContainsKey((UserStateType)StateKey))
        {
            if (_curState.Equals(_stateDic[(UserStateType)StateKey])) return;

            _curState.Exit();

            _curState = _stateDic[(UserStateType)StateKey];
            switch (_curState.StateType)
            {
                case UserStateType.Running:
                    _curState.Enter(this);
                    break;
                case UserStateType.Charging:
                    _curState.Enter(this, chooseBar(1));
                    break;
                case UserStateType.getCharged:
                    _curState.Enter(this, chooseBar(2));
                    break;
                case UserStateType.Resting:
                    _curState.Enter(this);
                    break;
                default:
                    break;
            }
        }
        
    }

    private string chooseBar(int a)
    {
        if(a==1)
        {
            //找电量最少的充电桩
            return barA.barInfo.curBattery < barB.barInfo.curBattery ? 
                barA.gameObject.ToString() : barB.gameObject.ToString();
        }
        if(a==2)
        {
            //找电量多的
            return barA.barInfo.curBattery > barB.barInfo.curBattery ?
                barA.gameObject.ToString() : barB.gameObject.ToString();
        }
        return barA.ToString();
    }

    /**********************************************************/
    //寻路
    [Header("寻路目标设置")]

    [Tooltip("Run的目标列表")]
    public Transform[] runningTargets;

    [Tooltip("充电桩A")]
    public Transform chargeBarATarget;

    [Tooltip("充电桩B")]
    public Transform chargeBarBTarget;

    public ChargeBarController barA;

    public ChargeBarController barB;

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

    public DayTimerController dayTimerController;

    /**********************************************************/

    private void Awake()
    {
        _stateDic[UserStateType.Running] = new UserRunState();
        _stateDic[UserStateType.getCharged] = new UserChargedState();
        _stateDic[UserStateType.Charging] = new UserChargingState();
        _stateDic[UserStateType.Resting] = new UserRestState();

        //写入初始状态
        _curState = _stateDic[initState];
    }

    private void Start()
    {
        isCharge = false;

        userInfo = GetComponent<UserInfo>();
        aipath = GetComponent<AIPath>();
        if (_curState.StateType == UserStateType.Running)
            _curState.Enter(this);
        else if (_curState.StateType == UserStateType.getCharged)
            _curState.Enter(this, chargeBarATarget.ToString());
    }

    private void Update()
    {
        _curState.Update();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name.Equals("ChargeCapsule_A") || other.gameObject.name.Equals("ChargeCapsule_B"))
        {
            isCharge = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Equals("ChargeCapsule_A") || other.gameObject.name.Equals("ChargeCapsule_B"))
        {
            isCharge = false;
        }
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
