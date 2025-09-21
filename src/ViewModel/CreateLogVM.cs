using BusinessLayer;
using log4net;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using TourPlanner.Model;

namespace TourPlanner.ViewModel
{
    public sealed class CreateLogVM : INotifyPropertyChanged
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CreateLogVM));

        private readonly LogLogic _logic;
        private Tour? _tour;
        private TourLog _log = new TourLog();
        private bool _isSaving;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? RequestClose;
        public event Action<TourLog>? LogSaved;
        public event Action<string>? ErrorOccurred;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public CreateLogVM(LogLogic logic)
        {
            _logic = logic;
            _log.LogDate = DateTime.Now;
            SaveCommand = new Relay(async _ => await SaveAsync(), _ => CanSave() && !_isSaving);
            CancelCommand = new Relay(_ => RequestClose?.Invoke());
        }

        public TourLog Log
        {
            get => _log;
            set
            {
                if (!ReferenceEquals(_log, value) && value != null)
                {
                    _log = value;
                    OnPropertyChanged(nameof(Log));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }
        private string _logDistanceInput = "0";
        public string LogDistanceInput
        {
            get => _logDistanceInput;
            set
            {
                _logDistanceInput = value;
                if (IsValidFloat(value))
                    _log.LogDistance = float.Parse(value, CultureInfo.InvariantCulture);
                else
                    _log.LogDistance = float.NaN;

                OnPropertyChanged(nameof(LogDistanceInput));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _logTimeInput = "0";
        public string LogTimeInput
        {
            get => _logTimeInput;
            set
            {
                _logTimeInput = value;
                if (IsValidFloat(value))
                    _log.LogTime = float.Parse(value, CultureInfo.InvariantCulture);
                else
                    _log.LogTime = float.NaN;

                OnPropertyChanged(nameof(LogTimeInput));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // -----------------------------------------------

        private bool CanSave()
        {
            if (Log == null) return false;
            if (Log.LogDifficulty < 1 || Log.LogDifficulty > 5) return false;
            if (Log.Rating < 1 || Log.Rating > 5) return false;
            if (!IsValidFloat(LogDistanceInput)) return false;
            if (!IsValidFloat(LogTimeInput)) return false;
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


        public void SetTour(Tour tour) 
        {
            _tour = tour ?? throw new ArgumentNullException(nameof(tour));
            CommandManager.InvalidateRequerySuggested();
        }
       
        private async Task SaveAsync()
        {
            if (_isSaving) return;
            _isSaving = true;
            CommandManager.InvalidateRequerySuggested();

            try
            {
                if (_tour == null)
                {
                    log.Warn("No Tour selectd to save.");
                    RequestClose?.Invoke();
                    return;
                }
                var dt = Log.LogDate;
                var utc = dt.Kind switch
                {
                    DateTimeKind.Utc => dt,
                    DateTimeKind.Local => dt.ToUniversalTime(),
                    _ => DateTime.SpecifyKind(dt, DateTimeKind.Local).ToUniversalTime()
                };

                log.Debug($"Log Date set as: {dt} ({dt.Kind}), UTC: {utc}");

                var newLog = new TourLog
                {
                    TourID = _tour.TourID,
                    LogDate = utc,
                    LogComment = Log.LogComment,
                    LogDifficulty = Log.LogDifficulty,
                    LogDistance = Log.LogDistance,
                    LogTime = Log.LogTime,
                    Rating = Log.Rating
                };
                LogDistanceInput = Log.LogDistance.ToString(CultureInfo.InvariantCulture);
                LogTimeInput = Log.LogTime.ToString(CultureInfo.InvariantCulture);
                TourLog saved;

                try
                {
                    saved = await _logic.CreateLogAsync(newLog);
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke("Failed to save log. " + ex.Message);
                    throw new VMExceptions.CreateLogVMException("Failed to save log. " + ex.Message, ex);
                }

                LogSaved?.Invoke(saved);
                RequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                log.Error("Failed to create Log", ex);
                ErrorOccurred?.Invoke("An unexpected error occurred while saving the log.");
                throw new VMExceptions.CreateLogVMException("An unexpected error occurred while saving the log.", ex);
            }
            finally
            {
                _isSaving = false;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
