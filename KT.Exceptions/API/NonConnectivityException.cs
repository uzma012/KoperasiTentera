using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KT.Exceptions.API
{
    public class NonConnectivityException : Exception
    {
        public NonConnectivityException() : base()
        {
        }

        public NonConnectivityException(string message) : base(message)
        {
        }

        public NonConnectivityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
