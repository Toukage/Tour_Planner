using BusinessLayer;
using log4net;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.Model;

namespace TourPlanner.ViewModel
{
    public sealed class LogListVM : INotifyPropertyChanged
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LogListVM));
        private readonly LogLogic _logic;
        private Tour? _currentTour;
        private TourLog? _selected;
        public ObservableCollection<TourLog> Logs { get; } = new();

        public TourLog? Selected
        {
            get => _selected;
            set { if (_selected == value) return; _selected = value; OnPropertyChanged(nameof(Selected)); }
        }

        public bool HasTour => _currentTour != null;
        public event Action? CreateRequested;
        public event Action<TourLog?>? ModifyRequested;
        public event Action<string>? ErrorOccurred;
        public ICommand CreateLogCommand { get; }
        public ICommand DeleteLogCommand { get; }
        public ICommand ModifyLogCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public LogListVM(LogLogic logic) //btn logik
        {
            _logic = logic;

            CreateLogCommand = new Relay(_ => { if (HasTour) CreateRequested?.Invoke(); });
            DeleteLogCommand = new Relay(async _ => {
                if (!HasTour || Selected == null) return;
                var del = Selected;
                try
                {
                    await _logic.DeleteLogAsync(del);
                    Logs.Remove(del);
                    Selected = Logs.FirstOrDefault();
                    log.Info("Log deleted.");
                }
                catch (Exception ex)
                {
                    log.Error("Failed to delete log.", ex);
                    ErrorOccurred?.Invoke("Failed to delete log.");
                    throw new VMExceptions.LogListVMException("Failed to delete log.", ex);
                }
            });
            ModifyLogCommand = new Relay(_ => { if (HasTour && Selected != null) ModifyRequested?.Invoke(Selected); });
        }

        public async Task LoadForTourAsync(Tour? tour, CancellationToken ct = default) //leadt alle logs fuer eine specific tour
        {
            _currentTour = tour;
            OnPropertyChanged(nameof(HasTour));
            Logs.Clear();
            Selected = null;
            if (tour == null) return;
            try
            {
                var data = await _logic.GetLogsAsync(tour.TourID, ct);
                foreach (var l in data) Logs.Add(l);
                Selected = Logs.FirstOrDefault();
            }
            catch (Exception ex)
            {
                log.Error("Failed to load logs.", ex);
                ErrorOccurred?.Invoke("Failed to load logs.");
                throw new VMExceptions.LogListVMException("Failed to load logs.", ex);
            }
        }

        public void RefreshItem(TourLog updated)//re-laedt selection wenn die log zb modified wurde und die neue version angezeigt werden soll.
        {
            var idx = Logs.ToList().FindIndex(l => l.LogID == updated.LogID);
            if (idx < 0) return;

            var wasSelected = ReferenceEquals(Selected, Logs[idx]);
            Logs.RemoveAt(idx);
            Logs.Insert(idx, updated);

            if (wasSelected) Selected = updated;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
