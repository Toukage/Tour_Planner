using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class DataAccessLayerException : Exception
    {
        public DataAccessLayerException(string message, Exception? inner = null) : base(message, inner) { }
    }

    public class TourRepoException : DataAccessLayerException
    {
        public TourRepoException(string message, Exception? inner = null) : base(message, inner) { }
    }

    public class LogRepoException : DataAccessLayerException
    {
        public LogRepoException(string message, Exception? inner = null) : base(message, inner) { }
    }
}
