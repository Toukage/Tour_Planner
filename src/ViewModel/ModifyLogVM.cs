using BusinessLayer;
using log4net;
using System.ComponentModel;
using System.Globalization;
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
        public event Action<string>? ErrorOccurred;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ModifyLogVM(LogLogic logic)
        {
            _logic = logic;
            SaveCommand = new Relay(async _ => await SaveAsync(), _ => CanSave());
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
        private string _logDistance = "0";
        public string LogDistance
        {
            get => _logDistance;
            set
            {
                _logDistance = value;
                if (IsValidFloat(value))
                    _log.LogDistance = float.Parse(value, CultureInfo.InvariantCulture);
                else
                    _log.LogDistance = float.NaN;

                OnPropertyChanged(nameof(LogDistance));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _logTime = "0";
        public string LogTime
        {
            get => _logTime;
            set
            {
                _logTime = value;
                if (IsValidFloat(value))
                    _log.LogTime = float.Parse(value, CultureInfo.InvariantCulture);
                else
                    _log.LogTime = float.NaN;

                OnPropertyChanged(nameof(LogTime));
                CommandManager.InvalidateRequerySuggested();
            }
        }
        private bool CanSave()
        {
            if (Log == null) return false;
            if (Log.LogDifficulty < 1 || Log.LogDifficulty > 5) return false;
            if (Log.Rating < 1 || Log.Rating > 5) return false;
            if (!IsValidFloat(LogDistance)) return false;
            if (!IsValidFloat(LogTime)) return false;
            return true;
        }
        private static bool IsValidFloat(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;
            if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
            {
                return value > 0 && !float.IsInfinity(value) && !float.IsNaN(value);
            }
            return false;
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
            LogDistance = log.LogDistance.ToString(CultureInfo.InvariantCulture);
            LogTime = log.LogTime.ToString(CultureInfo.InvariantCulture);
        }

        private async Task SaveAsync()
        {
            if (_original == null)
            {
                ErrorOccurred?.Invoke("Could not load the original log entry.");
                log.Warn("Original log entry not loaded.");
                RequestClose?.Invoke();
                return;
            }

            //schaut ob sich iwas geaendert hat
            bool changed =
               !DateTime.Equals(Log.LogDate, _original.LogDate) ||
               !string.Equals(Log.LogComment, _original.LogComment) ||
               !int.Equals(Log.LogDifficulty, _original.LogDifficulty) ||
               !float.Equals(Log.LogDistance, _original.LogDistance) ||
               !float.Equals(Log.LogTime, _original.LogTime) ||
               !int.Equals(Log.Rating, _original.Rating);

            if (!changed)
            {
                ErrorOccurred?.Invoke("No changes have been made.");
                RequestClose?.Invoke();
                return;
            }

            if (!CanSave())
            {
                ErrorOccurred?.Invoke("Please enter valid values for all fields.");
                log.Warn("Invalid log entry data for saving.");
                return;
            }
            Log.LogDate = DateTime.SpecifyKind(Log.LogDate, DateTimeKind.Utc);
            //aendert die alten werte
            _original.LogDate = Log.LogDate;
            _original.LogComment = Log.LogComment;
            _original.LogDifficulty = Log.LogDifficulty;
            _original.LogDistance = Log.LogDistance;
            _original.LogTime = Log.LogTime;
            _original.Rating = Log.Rating;

            try
            {
                await _logic.ModifyLogAsync(_original);
                LogSaved?.Invoke(_original);
                RequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                log.Error("Failed to modify log entry.", ex);
                ErrorOccurred?.Invoke("The log entry could not be modified. Please try again.");
                throw new VMExceptions.ModifyLogVMException("Failed to modify log entry.", ex);
            }
            finally
            {
                OnPropertyChanged(nameof(Log));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
