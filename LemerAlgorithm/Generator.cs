using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LemerAlgorithm.Exceptions;

namespace LemerAlgorithm
{
    public class Generator
    {
        private long _a;

        public long A
        {
            get { return _a; }
            private set
            {
                if (!IsPositive(value))
                    throw new NegativeOrZeroNumberException($"Value a = {value} is not positive", "a");

                _a = value;
            }
        }

        private long _r0;

        public long R0
        {
            get { return _r0; }
            private set
            {
                if (!IsPositive(value))
                    throw new NegativeOrZeroNumberException($"Value r0 = {value} is not positive", "r0");

                _r0 = value;
                RNew = value;
            }
        }

        private long _m;

        public long M
        {
            get { return _m; }
            private set
            {
                if (!IsPositive(value))
                    throw new NegativeOrZeroNumberException($"Value m = {value} is not positive", "m");

                if (value <= A)
                    throw new ArgumentException($"Value m = {value} must be greater than a = {A}", "m");

                _m = value;
            }
        }

        public long RPrev { get; private set; }
        public long RNew { get; private set; }

        public Generator(long a = 50021, long r0 = 50023, long m = 50033)
        {
            A = a;
            R0 = r0;
            M = m;
        }

        public double GetNext()
        {
            RPrev = RNew;
            RNew = (A * RPrev) % M;

            return RNew / (float)M;
        }

        private static bool IsPositive(long number) => number > 0;
    }
}
