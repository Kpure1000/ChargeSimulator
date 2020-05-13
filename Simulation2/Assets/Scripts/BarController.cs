using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarController : MonoBehaviour
{
    public string ID;

    [Header("初始化电量")]
    [Range(0, 1000)]
    public int Power;


}
