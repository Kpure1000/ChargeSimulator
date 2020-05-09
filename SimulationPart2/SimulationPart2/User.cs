using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationPart2
{
    public class User
    {
        public string ID;

        public int curTime;
        /// <summary>
        /// 寻找时间
        /// </summary>
        public int waitTime;
        /// <summary>
        /// 寻找次数
        /// </summary>
        public int waitTimes;
        
        public V2 position;

        public void Update()
        {
            StateMachine();
            //根据电量信息
            //更新状态
        }

        public void SearchBar()
        {

        }

        public enum State
        {
            Run,
            charged,
            charging
        }

        void StateMachine()
        {
            switch (userState)
            {
                case State.Run:
                    //单位时间耗电XX
                    break;
                case State.charged:
                    //单位时间充电
                    break;
                case State.charging:
                    //单位时间充电
                    break;
                default:
                    break;
            }
        }

        State userState;

    }
}
