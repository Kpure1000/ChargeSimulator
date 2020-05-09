using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour
{
    int curTime;

    /**********************************************************/

    private void Start()
    {
        Time.timeScale = 10f;
        curTime = DayTimerController.curTime;
    }

    private void FixedUpdate()
    {
        lightAngleUpdate();
    }

    private void lightAngleUpdate()
    {
        if (curTime != DayTimerController.curTime)
        {
            curTime = DayTimerController.curTime;
            transform.Rotate(360f / DayTimerController.maxCurTime, 0f, 0f);
        }
    }
}
