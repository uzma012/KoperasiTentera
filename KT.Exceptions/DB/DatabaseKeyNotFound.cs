using System;

namespace KT.Exceptions
{
    public class DatabaseKeyNotFound : Exception
    {
        public DatabaseKeyNotFound(string message) : base(message)
        {
        }

        public DatabaseKeyNotFound(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}