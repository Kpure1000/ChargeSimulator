using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    /// <summary>
    /// GameObject of Lamp Light
    /// </summary>
    public GameObject lampObject;
    private void Update()
    {
        if (DayTimerController.curDayTime.x >= 18 || DayTimerController.curDayTime.x < 5)
        {
            lampObject.SetActive(true);
        }
        else
        {
            lampObject.SetActive(false);
        }
    }

}
