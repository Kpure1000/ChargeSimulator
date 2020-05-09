using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayTimerController : MonoBehaviour
{
    /// <summary>
    /// 当前实际时间
    /// </summary>
    public int curTime { get; set; }
    /// <summary>
    /// 最大实际时间
    /// </summary>
    private const int maxCurTime = 1440;
    /// <summary>
    /// 当前格式时间（只读）
    /// </summary>
    public Vector2Int curDayTime
    {
        get { return new Vector2Int(curTime / 60, curTime % 60); }
    }
    /// <summary>
    /// 初始化时间
    /// </summary>
    public Vector2Int initCurDayTime;
    /// <summary>
    /// 引用时间显示UI
    /// </summary>
    public GameObject timeText;

    private void Start()
    {
        curTime = 60 * initCurDayTime.x + initCurDayTime.y;

        InvokeRepeating("dayTimeUpdate", 0f, 1f);
    }
    /// <summary>
    /// 增量重复监听方法
    /// </summary>
    void dayTimeUpdate()
    {
        curTime++;
        curTime %= maxCurTime;
    }

    private void Update()
    {
        timeTextUpdate();
    }
    /// <summary>
    /// 更新时间显示UI的时间
    /// </summary>
    private void timeTextUpdate()
    {
        if (timeText == null) return;
        timeText.GetComponent<Text>().text = string.Format("{0:00}:{1:00}", curDayTime.x, curDayTime.y);
    }
}
