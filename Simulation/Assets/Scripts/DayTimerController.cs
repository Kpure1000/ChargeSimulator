using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DayTimerController : MonoBehaviour
{
    /// <summary>
    /// 当前实际时间
    /// </summary>
    public static int curTime { get; set; }
    /// <summary>
    /// 最大实际时间
    /// </summary>
    public static int maxCurTime = 1440;
    /// <summary>
    /// 重复间隔时间
    /// </summary>
    [Range(.1f, 1f)]
    public float repeatRate = 0.1f;
    /// <summary>
    /// 当前格式时间（只读）
    /// </summary>
    public static Vector2Int curDayTime
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
    Text timeText_textComponent;

    public GameObject timeScaleText;
    Text timeScaleText_textComponent;

    private float realTime;

    /**********************************************************/

    private void Start()
    {
        curTime = 60 * initCurDayTime.x + initCurDayTime.y;

        timeText_textComponent = timeText.GetComponent<Text>();
        timeScaleText_textComponent = timeScaleText.GetComponent<Text>();
        button_Normal();

        realTime = 0;

        //InvokeRepeating("dayTimeUpdate", 0f, repeatRate);
    }
    /// <summary>
    /// 增量重复监听方法
    /// </summary>
    void dayTimeUpdate()
    {
        curTime++;
        curTime %= maxCurTime;

        if (timeText == null) return;
        timeText_textComponent.text = string.Format("{0:00}:{1:00}", curDayTime.x, curDayTime.y);
    }
    /// <summary>
    /// 监听
    /// </summary>
    private void FixedUpdate()
    {
        if(realTime >= repeatRate)
        {
            realTime = 0f;
            dayTimeUpdate();
        }
        realTime += Time.fixedDeltaTime;
    }

    /**********************************************************/



    public void button_Pause()
    {
        Time.timeScale = 0;
        timeScaleText_textComponent.text = "时间缩放：x0";
    }
    public void button_Normal()
    {
        Time.timeScale = 1;
        timeScaleText_textComponent.text = "时间缩放：x1";
    }
    public void button_Fast()
    {
        Time.timeScale = 10;
        timeScaleText_textComponent.text = "时间缩放：x10";
    }
}
