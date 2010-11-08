using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCleansing
{
    public class DataCleanerException : Exception
    {
        public DataCleanerException(string message)
            : base(message)
        {
        }
    }
}
