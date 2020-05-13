using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

/// <summary>
/// 模拟参数
/// </summary>
[Serializable]
[CreateAssetMenu(fileName = "New Simulation Param Asset", menuName = "NewSimulationParamAsset", order = 0)]
public class SimulationParam : ScriptableObject
{
    /// <summary>
    /// 单位时间长度
    /// </summary>
    [Header("设置单位时间长度:")]
    [Range(1, 100)]
    public int unitDayTime = 1;

    /// <summary>
    /// 获取太阳能速度
    /// </summary>
    [Space(30, order = 0)]
    [Header("充电桩设置：", order = 1)]
    [Header("获取太阳能速度", order = 2)]
    [Range(5, 20)]
    public int getSunPowerSpeed = 10;

    /// <summary>
    /// 充电桩被充电速度
    /// </summary>
    [Header("充电桩被充电速度")]
    [Range(1, 10)]
    public int getCarPowerSpeed = 3;

    /// <summary>
    /// 若低于此电量，不被用户寻找
    /// </summary>
    [Header("下限电量，低于此电量，仅被能够贡献电量的用户发现")]
    [Range(0f, 1f)]
    public float lowBatteryAlarm = .1f;

    /**********************************************************/

    /// <summary>
    /// 电动车被充电速度
    /// </summary>
    [Space(30, order = 0)]
    [Header("电动车设置：", order = 1)]
    [Header("电动车被充电速度", order = 2)]
    [Range(5, 20)]
    public int getBarPowerSpeed = 10;

    /// <summary>
    /// 电动车耗电速度
    /// </summary>
    [Header("电动车耗电速度")]
    [Range(1, 3)]
    public int usePowerSpeed = 1;

    ///// <summary>
    ///// 充电积分速度（电量/每积分）
    ///// </summary>
    //[Header("充电积分速度（电量/每积分）")]
    //[Range(10, 200)]
    //public int chargeRewardSpeed = 10;

    ///// <summary>
    ///// 充电扣费速度（电量/每元）
    ///// </summary>
    //[Header("充电费用（电量/每元）")]
    //[Range(1, 100)]
    //public int chargeCostSpeed = 10;

    ///// <summary>
    ///// 失信处罚比例，用于削减积分
    ///// </summary>
    //[Header("失信处罚比例，用于削减积分")]
    //[Range(0f, 1f)]
    //public float creditPublishRate = .6f;

    /// <summary>
    /// 开始被充电的电量，低于此电量开始寻找充电桩充电，找不到则休息，直到充电桩有电
    /// </summary>
    [Header("开始被充电的电量，用于警告充电")]
    [Range(0f, 1f)]
    public float startChargdePower = .1f;

    /// <summary>
    /// 停止充电的电量，达到此电量才可离开（除非充电桩没电）
    /// </summary>
    [Header("停止充电的电量，达到此电量才可离开（除非充电桩没电）")]
    [Range(0f, 1f)]
    public float stopChargedPower = .7f;

    /// <summary>
    /// 最低贡献电量，高于此值可给充电桩贡献电量
    /// </summary>
    [Header("最低贡献电量，高于此值可给充电桩贡献电量")]
    [Range(0f, 1f)]
    public float startChargingPower = .6f;

}
