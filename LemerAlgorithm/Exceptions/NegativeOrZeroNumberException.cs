using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LemerAlgorithm.Exceptions
{
    public class NegativeOrZeroNumberException: ArgumentException
    {
        public NegativeOrZeroNumberException()
        {
            
        }

        public NegativeOrZeroNumberException(string message) : base(message)
        {
            
        }

        public NegativeOrZeroNumberException(string message, Exception inner) : base(message, inner)
        {
            
        }

        public NegativeOrZeroNumberException(string message, string paramName) : base(message, paramName)
        {
            
        }
    }
}
