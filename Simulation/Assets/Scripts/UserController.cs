using System;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

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

    public bool isCharge = false;

    public bool isPark;

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

    void StateInit(UserStateType userInitState)
    {
        if (_stateDic.ContainsKey(userInitState))
        {
            _curState = _stateDic[userInitState];

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
#if UNITY_EDITOR
            //Debug.Log("初始化状态: " + _curState.StateType.ToString());
#endif

        }
        else
        {
            _curState = _stateDic[UserStateType.Resting];
            _curState.Enter(this);
#if UNITY_EDITOR
            //Debug.Log("初始化状态: " + _curState.StateType.ToString());
#endif

        }
    }

    private string chooseBar(int a)
    {
        if (a == 1)
        {
            //找电量最少的充电桩
            return barA.barInfo.curBattery < barB.barInfo.curBattery ?
                barA.gameObject.ToString() : barB.gameObject.ToString();
        }
        if (a == 2)
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
    [NonSerialized]
    public GameObject curChargeBar;

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

    private void Start()
    {
        isCharge = false;

        userInfo = GetComponent<UserInfo>();

        aipath = GetComponent<AIPath>();

        _stateDic[UserStateType.Running] = new UserRunState();
        _stateDic[UserStateType.getCharged] = new UserChargedState();
        _stateDic[UserStateType.Charging] = new UserChargingState();
        _stateDic[UserStateType.Resting] = new UserRestState();

        StateInit(initState);

    }

    private void Update()
    {
        _curState.Update();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("ChargeCapsule_A") || other.gameObject.name.Equals("ChargeCapsule_B"))
        {
            curChargeBar = other.gameObject;
            isCharge = true;
        }
        if (other.gameObject.name.Equals("parkTarget") && _curState.StateType == UserStateType.Resting)
        {
            isPark = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Equals("ChargeCapsule_A") || other.gameObject.name.Equals("ChargeCapsule_B"))
        {
            curChargeBar = other.gameObject;
            isCharge = false;
        }
        if (other.gameObject.name.Equals("parkTarget"))
        {
            isPark = false;
        }
    }

}
