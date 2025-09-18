using BusinessLayer;
using log4net;
using System.ComponentModel;
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

        public void SetTour(Tour tour) 
        {
            _tour = tour ?? throw new ArgumentNullException(nameof(tour));
            CommandManager.InvalidateRequerySuggested();
        }
        private bool CanSave()
        {
            if (_tour == null) return false;
            if (Log.LogDifficulty < 1 || Log.LogDifficulty > 5) return false;
            if (Log.Rating < 1 || Log.Rating > 5) return false;
            if (Log.LogDistance < 0) return false; 
            if (Log.LogTime < 0) return false;
            return true;
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

                var saved = await _logic.CreateAsync(_tour.TourID, utc, Log.LogComment, Log.LogDifficulty, Log.LogDistance, Log.LogTime, Log.Rating);

                LogSaved?.Invoke(saved);
                RequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                log.Error("CreateLog failed", ex);
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
