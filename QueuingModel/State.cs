using System;

namespace QueuingModel
{
    /// <summary>
    /// Class represents a system state
    /// </summary>
    public class State
    {
        /// <summary>
        /// A number of requests in ChannelOne
        /// </summary>
        public int ChannelOne
        {
            get { return _channelOne; }
            private set
            {
                if (value > 1 || value < 0)
                    throw new ArgumentException($"Value {value} should be either 0 or 1");

                _channelOne = value;
            }
        }

        /// <summary>
        /// A number of requests int Queue
        /// </summary>
        public int Queue
        {
            get { return _queue; }
            private set
            {
                if (value > 2 || value < 0)
                    throw new ArgumentException($"Value {value} should be either 0, 1 or 2");

                _queue = value;
            }
        }

        /// <summary>
        /// A number of requests int ChannelTwo
        /// </summary>
        public int ChannelTwo
        {
            get { return _channelTwo; }
            private set
            {
                if (value > 1 || value < 0)
                    throw new ArgumentException($"Value {value} should be either 0 or 1");

                _channelTwo = value;
            }
        }

        /// <summary>
        /// An index of the state
        /// </summary>
        public int Index => StrStateToIndex(this.ToString());

        private int _channelOne;
        private int _channelTwo;
        private int _queue;

        public State(int channelOne, int queue, int channelTwo)
        {
            ChannelOne = channelOne;
            ChannelTwo = channelTwo;
            Queue = queue;
        }

        // DA ETO JOSTKA
        /// <summary>
        /// Determines the next state from the values of `ro`, `pi1`, `pi2`
        /// </summary>
        /// <param name="ro">False if Source has generated new request</param>
        /// <param name="pi1">False if ChannelOne has processed the request</param>
        /// <param name="pi2">False if ChannelTwo has processed the request</param>
        /// <returns>Next state</returns>
        public State NextState(bool ro, bool pi1, bool pi2)
        {
            var strState = this.ToString();
            State nextState = null;

            switch (strState)
            {
                case "000":
                    nextState = ro ? new State(0, 0, 0) : new State(1, 0 ,0);
                    break;
   
                case "100":
                    if (pi1)
                        nextState = new State(1, 0, 0);
                    else if (!ro && !pi1) // (1 - p)(1 - pi1)
                        nextState = new State(1, 0, 1);
                    else if (ro && !pi1) // p(1 - pi1)
                        nextState = new State(0, 0, 1); 
                    break;

                case "001":
                    if (ro && pi2) // p*pi2
                        nextState = new State(0, 0, 1);
                    else if (!ro && !pi2) // (1 - p)(1 - pi2)
                        nextState = new State(1, 0, 0);
                    else if (!ro && pi2)
                        nextState = new State(1, 0, 1);
                    else if (ro && !pi2) // p(1 - pi2)
                        nextState = new State(0, 0, 0);
                    break;

                case "101":
                    if (pi1 && pi2 || 
                        !ro && !pi1 && !pi2) // pi1*pi2 + (1 - p)(1 - pi1)(1 - pi2)
                        nextState = new State(1, 0, 1);
                    else if (pi1 && !pi2) // pi1(1 - pi2)
                        nextState = new State(1, 0, 0);
                    else if (ro && !pi1 && !pi2) // p(1 - pi1)(1 - pi2)
                        nextState = new State(0, 0, 1);
                    else if (ro && !pi1 && pi2) // p(1 - pi1)pi2
                        nextState = new State(0, 1, 1);
                    else if (!ro && !pi1 && pi2) // (1 - p)(1 - pi1)pi2
                        nextState = new State(1, 1, 1);
                    break;

                case "011":
                    if (ro && pi2) // p*pi2
                        nextState = new State(0, 1, 1);
                    else if (!ro && !pi2) // (1 - p)(1 - pi2)
                        nextState = new State(1, 0, 1);
                    else if (!ro && pi2) // (1 - p)pi2
                        nextState = new State(1, 1, 1);
                    else if (ro && !pi2) // p(1 - pi2)
                        nextState = new State(0, 0, 1);
                    break;

                case "111":
                    if (pi1 && pi2 ||
                        !ro && !pi1 && !pi2) // pi1*pi2 + (1 - p)(1 - pi1)(1 - pi2)
                        nextState = new State(1, 1, 1);
                    else if (pi1 && !pi2) // pi1(1 - pi2)
                        nextState = new State(1, 0, 1);
                    else if (ro && !pi1 && !pi2) // p(1 - pi1)(1 - pi2)
                        nextState = new State(0, 1, 1);
                    else if (ro && !pi1 && pi2) // p(1 - pi1)pi2
                        nextState = new State(0, 2, 1);
                    else if (!ro && !pi1 && pi2) // (1 - p)(1 - pi1)pi2
                        nextState = new State(1, 2, 1);
                    break;

                case "021":
                    if (ro && pi2) // p*pi2
                        nextState = new State(0, 2, 1);
                    else if (!ro && !pi2) // (1 - p)(1 - pi2)
                        nextState = new State(1, 1, 1);
                    else if (!ro && pi2) // (1 - p)pi2
                        nextState = new State(1, 2, 1);
                    else if (ro && !pi2) // p(1 - pi2)
                        nextState = new State(0, 1, 1);
                    break;

                case "121":
                    if (pi1 && pi2 ||
                        !ro && !pi1) // pi1*pi2 + (1 - p)(1 - pi1)
                        nextState = new State(1, 2, 1);
                    else if (pi1 && !pi2) // pi1(1 - pi2)
                        nextState = new State(1, 1, 1);
                    else if (ro && !pi1)
                        nextState = new State(0, 2, 1);
                    break;

                default:
                    nextState = null;
                    break;
            }

            return nextState;
        }

        /// <summary>
        /// Convert index of the state to state string value
        /// </summary>
        /// <param name="index">index of the state</param>
        /// <returns>State string value or String.Empty if index is not valid</returns>
        public static string IndexToStateStr(int index)
        {
            var str = string.Empty;

            switch (index)
            {
                case 0:
                    str = "000";
                    break;

                case 1:
                    str = "100";
                    break;

                case 2:
                    str = "001";
                    break;

                case 3:
                    str = "101";
                    break;

                case 4:
                    str = "011";
                    break;

                case 5:
                    str = "111";
                    break;

                case 6:
                    str = "021";
                    break;

                case 7:
                    str = "121";
                    break;

                default:
                    str = string.Empty;
                    break;
            }

            return str;
        }

        /// <summary>
        /// Convert state string value to it's index
        /// </summary>
        /// <param name="strState">State string value</param>
        /// <returns></returns>
        public static int StrStateToIndex(string strState)
        {
            int index = -1;

            switch (strState)
            {
                case "000":
                    index = 0;
                    break;

                case "100":
                    index = 1;
                    break;

                case "001":
                    index = 2;
                    break;

                case "101":
                    index = 3;
                    break;

                case "011":
                    index = 4;
                    break;

                case "111":
                    index = 5;
                    break;

                case "021":
                    index = 6;
                    break;

                case "121":
                    index = 7;
                    break;

                default:
                    index = -1;
                    break;
            }

            return index;
        }

        public override string ToString()
        {
            return string.Concat(ChannelOne, Queue, ChannelTwo);
        }
    }
}
