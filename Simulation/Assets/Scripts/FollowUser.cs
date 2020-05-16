using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUser : MonoBehaviour
{
    /// <summary>
    /// 用户Transform
    /// </summary>
    public Transform user;
    /// <summary>
    /// 充电桩A
    /// </summary>
    public Transform barA;
    /// <summary>
    /// 充电桩B
    /// </summary>
    public Transform barB;
    /// <summary>
    /// 停车位
    /// </summary>
    public Transform park;
    /// <summary>
    /// Map中心
    /// </summary>
    public Transform center;


    public Transform focusPoint;

    Dictionary<int, Transform> targetDictionary = new Dictionary<int, Transform>();

    Transform currentTarget;

    /// <summary>
    /// 接近速度
    /// </summary>
    [Header("接近速度")]
    public float closeSpeed = 5f;

    /// <summary>
    /// 接近判定距离
    /// </summary>
    [Header("接近判定距离")]
    public float closeDistance = .2f;

    private void Start()
    {
        targetDictionary[0] = user;
        targetDictionary[1] = barA;
        targetDictionary[2] = barB;
        targetDictionary[3] = park;
        targetDictionary[4] = center;

        FocusOn(0);

        transform.position = new Vector3(center.position.x, transform.position.y, center.position.z);

    }

    private void Update()
    {
        Vector3 accelerate = currentTarget.position - transform.position;
        accelerate.y = 0f;
        accelerate *= closeSpeed * Time.unscaledDeltaTime;

        float distance = Vector2.Distance(new Vector2(currentTarget.position.x, currentTarget.position.z),
            new Vector2(transform.position.x, transform.position.z));

        if (distance >= closeDistance)
        {
            transform.position += accelerate;
        }

        focusPoint.position = new Vector3(currentTarget.position.x, focusPoint.position.y, currentTarget.position.z);

    }

    public void FocusOn(int TargetNum)
    {
        if (targetDictionary.ContainsKey(TargetNum) == true)
        {
            currentTarget = targetDictionary[TargetNum];
        }
    }

}
