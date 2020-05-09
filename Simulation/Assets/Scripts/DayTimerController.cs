using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayTimerController : MonoBehaviour
{
    public int curTime { get; set; }

    public GameObject timeText;

    private void Start()
    {
        curTime = 0;
        this.InvokeRepeating("dayTimeUpdate", 0f, 1f);
    }

    void dayTimeUpdate()
    {
        curTime++;
    }

    private void Update()
    {

        timeTextUpdate();
    }

    private void timeTextUpdate()
    {
        if (timeText == null) return;
        timeText.GetComponent<Text>().text =
            string.Format("{0}:{1}", curTime, curTime);
    }
}
