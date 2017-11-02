using System;
using System.Collections.Generic;
using LemerAlgorithm;

namespace QueuingModel
{
    /// <summary>
    /// Class represents Queuing model
    /// </summary>
    public class Model
    {
        /// <summary>
        /// Counter of how many time units we need to substract because of refusals 
        /// </summary>
        public int Channel1Counter { get; private set; }
        
        /// <summary>
        /// Amound of requests which are in the system now
        /// </summary>
        public int RequestCount => CurrentState.ChannelOne + CurrentState.ChannelTwo + CurrentState.Queue;

        /// <summary>
        /// The system parameter which is determined by dividing outputs by inputs
        /// </summary>
        public float RelativeThroughput => (float)outputs / inputs;

        /// <summary>
        /// Current tick in system
        /// </summary>
        public int TicksCount { get; private set; }

        /// <summary>
        /// One of the system parameter
        /// </summary>
        public float QueueAverageLength
        {
            get
            {
                long sum = 0;

                foreach (var state in States)
                {
                    sum += state.Queue;
                }

                return (float)sum / TicksCount;
            }
        }

        /// <summary>
        /// The current state of the system
        /// </summary>
        public State CurrentState { get; private set; }

        /// <summary>
        /// All states the system was stated before
        /// </summary>
        public List<State> States { get; private set; }

        /// <summary>
        /// Array that is storing a count of each state in system 
        /// </summary>
        public int[] StatesCount { get; private set; }

        /// <summary>
        /// Probability that source won't generate a request   
        /// </summary>
        public float Ro
        {
            get { return _ro; }
            set
            {
                if (!IsValidProbability(value))
                    throw new ArgumentException($"Value {value} is not between 0 and 1");

                _ro = value;
            }
        }

        /// <summary>
        /// Probability that ChannelOne won't process the request
        /// </summary>
        public float Pi1
        {
            get { return _pi1; }
            set
            {
                if (!IsValidProbability(value))
                    throw new ArgumentException($"Value {value} is not between 0 and 1");

                _pi1 = value;
            }
        }


        /// <summary>
        /// Probability that ChannelTwo won't process the request
        /// </summary>
        public float Pi2
        {
            get { return _pi2; }
            set
            {
                if (!IsValidProbability(value))
                    throw new ArgumentException($"Value {value} is not between 0 and 1");

                _pi2 = value;
            }
        }

        /// <summary>
        /// Determines an action of Source
        /// </summary>
        /// <remarks>If it equals true, the source didn't generate a request</remarks>
        public bool isRo { get; private set; }

        /// <summary>
        /// Determines an action of ChannelOne
        /// </summary>
        /// <remarks>If it equals true, the channelOne didn't process the request</remarks>
        public bool isPi1 { get; private set; }

        /// <summary>
        /// Determines an action of ChannelTwo
        /// </summary>
        /// <remarks>If it equals true, the channelOne didn't process the request</remarks>
        public bool isPi2 { get; private set; }

        private float _ro;
        private float _pi1;
        private float _pi2;

        /// <summary>
        /// A float values generator from 0 to 1
        /// </summary>
        private Generator generator;

        /// <summary>
        /// Counter for requests which have entered the system
        /// </summary>
        private int inputs;

        /// <summary>
        /// Counter for request which have processed by the system
        /// </summary>
        private int outputs;

        public Model(float ro = 0.75f, float pi1 = 0.7f, float pi2 = 0.65f)
        {
            Ro = ro;
            Pi1 = pi1;
            Pi2 = pi2;

            generator = new Generator();
            Reset();
        }

        /// <summary>
        /// Reset the model to start state 
        /// </summary>
        public void Reset()
        {
            CurrentState = new State(0, 0, 0);
            States = new List<State> { CurrentState };
            StatesCount = new int[8];
            StatesCount[CurrentState.Index] = 1;

            inputs = 0;
            outputs = 0;

            TicksCount = 0;
            Channel1Counter = 0;
        }

        /// <summary>
        /// Immitates the system changes after one unit of time
        /// </summary>
        public void NextTick()
        {
            TicksCount += 1;
            
            isRo = generator.GetNext() <= Ro; // true <==> Источник не выдал заявку
            isPi1 = generator.GetNext() <= Pi1; // true <==> Первый канал не обработал заявку
            isPi2 = generator.GetNext() <= Pi2; // true <==> Второй канал не обработал заявку

            if (!isRo)
                inputs++;
            if (CurrentState.ChannelTwo == 1 && !isPi2)
                outputs++;

            if (CurrentState.ChannelOne == 1)
                Channel1Counter++;

            if (CurrentState.ChannelOne == 1 && CurrentState.Queue == 2 && CurrentState.ChannelTwo == 1 && isPi2 &&
                !isPi1)
            {
                TicksCount -= Channel1Counter;
                Channel1Counter = 0;
            }
            else if (!isPi1)
                Channel1Counter = 0;

                
            var newState = CurrentState.NextState(isRo, isPi1, isPi2);
            States.Add(newState);
            CurrentState = newState;

            StatesCount[newState.Index]++;
        }

        /// <summary>
        /// Get probabiity of state using his string value
        /// </summary>
        /// <param name="stateStr">String representation of the state</param>
        /// <returns>Probability of the state</returns>
        public float GetStateProbability(string stateStr)
            => (float) this.StatesCount[State.StrStateToIndex(stateStr)] / States.Count;

        /// <summary>
        /// Get probabiity of state using his index value
        /// </summary>
        /// <param name="index">Index value of the state</param>
        /// <returns>Probability of the state</returns>
        public float GetStateProbability(int index) => (float) this.StatesCount[index] / States.Count;

        /// <summary>
        /// One of the system parameter
        /// </summary>
        public float AverageRequestTimeInSystem => (float)TicksCount / outputs;

        /// <summary>
        /// Determines whether the value is between 0 and 1
        /// </summary>
        /// <param name="probability">Value</param>
        /// <returns>True if it's between 0 and 1</returns>
        private static bool IsValidProbability(float probability) => probability >= 0 && probability <= 1;
    }
}
