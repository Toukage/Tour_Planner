using BusinessLayer;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TourPlanner.Model;

namespace TourPlanner.ViewModel
{
    class ModifyLogVM : INotifyPropertyChanged
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ModifyLogVM));
        private readonly LogLogic _logic;
        private TourLog? _original;
        private TourLog _log = new TourLog();

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? RequestClose;
        public event Action<TourLog>? LogSaved;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ModifyLogVM(LogLogic logic)
        {
            _logic = logic;
            SaveCommand = new Relay(async _ => await SaveAsync());
            CancelCommand = new Relay(_ => RequestClose?.Invoke());
        }

        public TourLog Log
        {
            get => _log;
            set
            {
                if (_log != value && value != null)
                {
                    _log = value;
                    OnPropertyChanged(nameof(Log));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public void LoadFrom(TourLog log) //populated die input felder mit den tour daten
        {
            _original = log;
            Log = new TourLog
            {
                
                LogID = log.LogID,
                TourID = log.TourID,
                LogDate = log.LogDate,
                LogComment = log.LogComment,
                LogDifficulty = log.LogDifficulty,
                LogDistance = log.LogDistance,
                LogTime = log.LogTime,
                Rating = log.Rating
            }; 
        }

        private async Task SaveAsync()
        {
            CommandManager.InvalidateRequerySuggested();

            //schaut ob sich iwas geaendert hat
            bool changed =
               !DateTime.Equals(Log.LogDate, _original.LogDate) ||
               !string.Equals(Log.LogComment, _original.LogComment) ||
               !int.Equals(Log.LogDifficulty, _original.LogDifficulty) ||
               !float.Equals(Log.LogDistance, _original.LogDistance) ||
               !float.Equals(Log.LogTime, _original.LogTime) ||
               !int.Equals(Log.Rating, _original.Rating);

            if (!changed) { RequestClose?.Invoke(); return; }
            Log.LogDate = DateTime.SpecifyKind(Log.LogDate, DateTimeKind.Utc);
            //aendert die alten werte
            _original.LogDate = Log.LogDate;
            _original.LogComment = Log.LogComment;
            _original.LogDifficulty = Log.LogDifficulty;
            _original.LogDistance = Log.LogDistance;
            _original.LogTime = Log.LogTime;
            _original.Rating = Log.Rating;

            await _logic.ModifyAsync(_original);
            LogSaved?.Invoke(_original);
            RequestClose?.Invoke();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
