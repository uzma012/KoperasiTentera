using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KT.Exceptions
{
    public class OTPException: Exception
    {
        public OTPException() : base()
        {
        }

        public OTPException(string message) : base(message)
        {
        }

        public OTPException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
