using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    /// <summary>
    /// GameObject of Lamp Light
    /// </summary>
    public GameObject lampObject;

    [Range(0, 23)]
    public int offTime;
    [Range(0, 23)]
    public int onTime;

    private void Awake()
    {
        if(offTime > onTime)
        {
            offTime = onTime;
        }
    }

    private void Update()
    {
        if (DayTimerController.curDayTime.x >= onTime || DayTimerController.curDayTime.x < offTime)
        {
            lampObject.SetActive(true);
        }
        else
        {
            lampObject.SetActive(false);
        }
    }

}
