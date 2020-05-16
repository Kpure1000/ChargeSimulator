using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 仅控制User 移动
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class UserController : MonoBehaviour
{
    /*****************************************************/
    //状态
    /// <summary>
    /// 初始化
    /// </summary>
    public userState _curState;

    public stateType initState;

    public Dictionary<stateType, userState> stateDictionary = new Dictionary<stateType, userState>();


    /*****************************************************/
    //位置
    /// <summary>
    /// 行驶状态的目标
    /// </summary>
    public List<Transform> runningTargets;
    /// <summary>
    /// 靠近判断
    /// </summary>
    [NonSerialized]
    public bool isClosed;
    /// <summary>
    /// 靠近距离
    /// </summary>
    public float closeDistance;
    /// <summary>
    /// 充电桩目标
    /// </summary>
    public Transform barTarget;

    public Transform restTarget;

    [Range(100,1500)]
    public float closeForce;

    new Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        stateDictionary[stateType.Running] = new RunningState();
        stateDictionary[stateType.Charged] = new ChargedState();
        stateDictionary[stateType.Charging] = new ChargingState();
        stateDictionary[stateType.Resting] = new RestingState();

        //初始化状态
        _curState = stateDictionary[initState];
        _curState.Enter(this);
    }


    private void Update()
    {
        Vector3 force = barTarget.position - rigidbody.position;
        force = force.normalized * closeForce * Time.deltaTime;

        rigidbody.AddForce(force);

        if (Vector3.Distance(rigidbody.position, barTarget.position) < closeDistance)
        {
            isClosed = true;
        }
        else
        {
            isClosed = false;
        }

        _curState.Update();
    }

}
