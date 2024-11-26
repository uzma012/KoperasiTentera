using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KT.Exceptions.DB
{
    public class DuplicateDatabaseKeyException: Exception
    {
        public DuplicateDatabaseKeyException() : base()
        {
        }

        public DuplicateDatabaseKeyException(string message) : base(message)
        {
        }

        public DuplicateDatabaseKeyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
