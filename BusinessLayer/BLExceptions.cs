using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class BusinessLayerException : Exception
    {
        public BusinessLayerException(string message, Exception? inner = null) : base(message, inner) { }
    }

    public class TourLogicException : BusinessLayerException
    {
        public TourLogicException(string message, Exception? inner = null) : base(message, inner) { }
    }

    public class LogLogicException : BusinessLayerException
    {
        public LogLogicException(string message, Exception? inner = null) : base(message, inner) { }
    }

    public class ReportException : BusinessLayerException
    {
        public ReportException(string message, Exception? inner = null) : base(message, inner) { }
    }

    public class RoutingException : BusinessLayerException
    {
        public RoutingException(string message, Exception? inner = null) : base(message, inner) { }
    }

    public class DirectionsException : BusinessLayerException
    {
        public DirectionsException(string message, Exception? inner = null) : base(message, inner) { }
    }

    public class GeocodeException : BusinessLayerException
    {
        public GeocodeException(string message, Exception? inner = null) : base(message, inner) { }
    }
}
