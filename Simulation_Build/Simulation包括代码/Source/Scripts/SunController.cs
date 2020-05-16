using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour
{
    //int curTime;
    float realTime;
    /// <summary>
    /// Game Object of DayTimeController 
    /// </summary>
    public GameObject TimeController;

    DayTimerController dayTimerController;

    private int lerpTimes = 2;

    /**********************************************************/

    private void Start()
    {
        //curTime = DayTimerController.curTime;

        dayTimerController = TimeController.GetComponent<DayTimerController>();

    }

    private void FixedUpdate()
    {
        //if (curTime != DayTimerController.curTime)
        //{
        //    curTime = DayTimerController.curTime;

        //    transform.Rotate(360f / DayTimerController.maxCurTime, 0f, 0f);

        //}

        if (realTime >= dayTimerController.repeatRate / lerpTimes)
        {
            realTime = 0f;
            transform.Rotate(360f / DayTimerController.maxCurTime / lerpTimes, 0f, 0f);
        }
        realTime += Time.fixedDeltaTime;
    }

}
