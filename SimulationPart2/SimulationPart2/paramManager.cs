using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SimulationPart2
{
    public class paramManager
    {
        /// <summary>
        /// 充电速度
        /// </summary>
        public int chargeSpeed;
        /// <summary>
        /// 耗电速度
        /// </summary>
        public int useSpeed;
        /// <summary>
        /// 当前实际时间
        /// </summary>
        public int curTime;

        public int usersNum;

        public int barsNum;
        /// <summary>
        /// 活动范围
        /// </summary>
        public Rect rangeRect;

        public int avgWaitTime;

        public int avgWaitTimes;

        //统计充电桩 某些信息
        //list

    }
}
