using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SimulationStatisticController : MonoBehaviour
{
    /// <summary>
    /// 可序列化 模拟目标数据类
    /// </summary>
    [Serializable]
    public class SimulationStatisticData
    {
        public float AvgSeekTimes;
        public float AvgSeekTime;
        public float ChargeStressVariance;
    }
    

    List<SimulationStatisticData> simulationStatisticList = new List<SimulationStatisticData>();

    SimulationStatisticData fileTarget = new SimulationStatisticData();

    /// <summary>
    /// 回调委托，用于及时添加数据
    /// </summary>
    /// <param name="statisticData"></param>
    public delegate SimulationStatisticData AddData();

    /// <summary>
    /// 由User Info 调用
    /// </summary>
    /// <param name="param"></param>
    public void StatisticIn(params object[] param)
    {
        simulationStatisticList.Sort(
            (x, y) => x.AvgSeekTimes.CompareTo(y.AvgSeekTimes)
            );

        fileTarget = param[0] as SimulationStatisticData;

        int index = 0;

        foreach (var item in simulationStatisticList)
        {
            if(item.AvgSeekTime < fileTarget.AvgSeekTime)
            {
                fileTarget.AvgSeekTime += item.AvgSeekTime; 
            }
            if(item.AvgSeekTimes > fileTarget.AvgSeekTimes)
            {
                fileTarget.AvgSeekTimes += item.AvgSeekTimes; 
            }
            if(item.ChargeStressVariance > fileTarget.ChargeStressVariance)
            {
                fileTarget.ChargeStressVariance += item.ChargeStressVariance / index;

            }

            index++;
        }

    }

    /// <summary>
    /// 添加数据以及回调
    /// </summary>
    /// <param name="OnAddCallBack">回调</param>
    /// <param name="param">数据</param>
    public void OnAddData_Each(AddData OnAddCallBack)
    {
        //这样可以减少之后的排序时间
        simulationStatisticList.Sort(
            (x, y) => x.AvgSeekTimes.CompareTo(y.AvgSeekTimes)
            );

        //添加回调处理 的监听 返回数据
        simulationStatisticList.Add(OnAddCallBack.Invoke());

    }

    /****************************************************************/

    public string fileName_tradition;

    public string fileName_intelligence;

    public enum simulationType
    {
        Traditional,
        Intelligent
    }

    public SimulationStatisticData OutputData_Traditional = new SimulationStatisticData();
    public SimulationStatisticData OutputData_Intelligent = new SimulationStatisticData();

    public void WriteToFile(simulationType simulationType)
    {
        string fileString;
        switch (simulationType)
        {
            case simulationType.Traditional:
                fileString = JsonUtility.ToJson(OutputData_Traditional);

                File.WriteAllText(fileName_tradition, fileString);
                break;
            case simulationType.Intelligent:
                fileString = JsonUtility.ToJson(OutputData_Intelligent);

                File.WriteAllText(fileName_intelligence, fileString);
                break;
            default:
                break;
        }
        
    }

}
