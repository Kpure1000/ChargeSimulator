using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SimulationPart2
{
    public class Bar
    {
        public string ID;

        public int power;

        public int curTime;
        /// <summary>
        /// 仅指 提供充电的次数
        /// </summary>
        public int chargeOutTimes;
        /// <summary>
        /// 被车充电次数
        /// </summary>
        [NonSerialized]
        public int chargeInTimes;

        public enum State
        {
            charged,
            charging,
            rest
        }

        public State barState;

        public void Update()
        {
            StateMachine();
        }

        private void StateMachine()
        {
            switch (barState)
            {
                case State.charged:
                    break;
                case State.charging:
                    break;
                case State.rest:
                    //啥也不干
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 交互次数
        /// </summary>
        public int chargeStress
        {
            get { return chargeInTimes + chargeOutTimes; }
        }

        public Vector2 position;



    }
}
