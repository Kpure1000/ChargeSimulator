using System;
using UnityEngine;
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

    private float realTime;

    /**********************************************************/

    private void Start()
    {
        quitPanel.SetActive(false);

        curTime = 60 * initCurDayTime.x + initCurDayTime.y;

        button_Normal();

        //quitConfirmPanel.SetActive(false);

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

    }
    /// <summary>
    /// 1.7分钟秒结束
    /// </summary>
    public float endTime = 102f;

    public float realWorldTime = 0f;

    /// <summary>
    /// 监听
    /// </summary>
    private void FixedUpdate()
    {
        if (realTime >= repeatRate)
        {
            realTime = 0f;
            dayTimeUpdate();
        }
        realTime += Time.fixedDeltaTime;


    }

    private void Update()
    {
        realWorldTime += Time.unscaledDeltaTime;

        if (realWorldTime > endTime)
        {
            SimulationEnd();
        }
    }

    /**********************************************************/

    public void button_Pause()
    {
        Time.timeScale = 0;
    }
    public void button_Normal()
    {
        Time.timeScale = 1;
    }
    public void button_Fast()
    {
        Time.timeScale = 10;
    }

    public GameObject quitPanel;

    public void SimulationEnd()
    {
        button_Pause();
        quitPanel.SetActive(true);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); 
#endif
    }
}
