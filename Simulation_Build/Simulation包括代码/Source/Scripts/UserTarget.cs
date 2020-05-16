using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Pathfinding;
using UnityEditor;
[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(UserController))]
[UniqueComponent(tag = "ai.destination")]
public class UserTarget : VersionedMonoBehaviour
{
    /// <summary>
    /// 目标
    /// </summary>
    Transform target;

    IAstarAI ai;
    /// <summary>
    /// User控制组件
    /// </summary>
    UserController userController;

    private void Start()
    {
        userController = GetComponent<UserController>();
    }

    void OnEnable()
    {
        ai = GetComponent<IAstarAI>();

        if (ai != null) ai.onSearchPath += Update;
    }

    void OnDisable()
    {
        if (ai != null) ai.onSearchPath -= Update;
    }

    void Update()
    {
        target = userController.curTarget;
        if (target != null && ai != null)
        {
            ai.destination = target.position;
        }
    }

    private void OnDrawGizmos()
    {
        if (target)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(target.position, .3f);
        }
    }

}

