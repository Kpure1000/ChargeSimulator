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
    /// <summary>
    /// 初始化
    /// </summary>
    public userState _curState;

    public stateType initState;

    public Dictionary<stateType, userState> stateDictionary = new Dictionary<stateType, userState>();

    public List<Transform> runningTargets;

    [NonSerialized]
    public bool isClosed;

    public float closeDistance;

    /*****************************************************/
    public Transform barTarget;

    [Range(0, 100)]
    public float closeForce;

    new Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        stateDictionary[stateType.Running] = new RunningState();

        //初始化状态
        _curState = stateDictionary[initState];
        _curState.Enter(this);
    }


    private void FixedUpdate()
    {
        Vector3 force = barTarget.position - rigidbody.position;
        force = force.normalized * closeForce;
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
