using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.ViewModel
{
    class VMExceptions
    {
        public class ViewModelException : Exception
        {
            public ViewModelException(string message, Exception? inner = null) : base(message, inner) { }
        }

        public class CreateLogVMException : ViewModelException
        {
            public CreateLogVMException(string message, Exception inner) : base(message, inner) { }
        }

        public class CreateTourVMException : ViewModelException
        {
            public CreateTourVMException(string message, Exception inner) : base(message, inner) { }
        }

        public class MainVMException : ViewModelException
        {
            public MainVMException(string message, Exception inner) : base(message, inner) { }
        }

        public class MapVMException : ViewModelException
        {
            public MapVMException(string message, Exception inner) : base(message, inner) { }
        }

        public class ModifyLogVMException : ViewModelException
        {
            public ModifyLogVMException(string message, Exception inner) : base(message, inner) { }
        }

        public class ModifyTourVMException : ViewModelException
        {
            public ModifyTourVMException(string message, Exception inner) : base(message, inner) { }
        }

        public class TourDetailsVMException : ViewModelException
        {
            public TourDetailsVMException(string message, Exception inner) : base(message, inner) { }
        }
        public class LogListVMException : ViewModelException
        {
            public LogListVMException(string message, Exception inner) : base(message, inner) { }
        }
        public class TourListVMException : ViewModelException
        {
            public TourListVMException(string message, Exception inner) : base(message, inner) { }
        }

    }
}
