using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

[RequireComponent(typeof(UserController))]
public class UserInfo : MonoBehaviour
{
    public string ID;

    [NonSerialized]
    public int curTime;
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

    UserController userController;

    private void Start()
    {
        userController = GetComponent<UserController>();
    }

    public void Update()
    {
        //根据电量信息
        //更新状态
    }

    public void SearchBar()
    {

        Transform newTarget = null;
        //找完之后
        //变更barTarget
        userController.barTarget = newTarget;
    }

}
