using System.ComponentModel;
using log4net;
using BusinessLayer;
using TourPlanner.Model;
using System.Threading;

namespace TourPlanner.ViewModel
{
    public class MapVM : INotifyPropertyChanged
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MapVM));
        private readonly TourLogic _logic;
        private CancellationTokenSource? _cts;
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action<string>? ErrorOccurred;

        public string CurrentRouteGeoJson { get; set; } = "";

        private Tour? _selectedTour;
        public Tour? SelectedTour
        {
            get => _selectedTour;
            set
            {
                if (_selectedTour == value) return;
                _selectedTour = value;
                log.Info($"Selected Tour:  {value?.TourID} - {value?.TourName}");
                LoadRoute();
            }
        }

        public MapVM(TourLogic logic)
        {
            _logic = logic;
        }

        public void SetRoute(string? geoJson)
        {
            CurrentRouteGeoJson = geoJson ?? "";
            OnPropertyChanged(nameof(CurrentRouteGeoJson));
        }

        private async void LoadRoute()
        {
            _cts?.Cancel(); 
            log.Info("Loading Route.");
            if (_selectedTour == null)
            {
                SetRoute("");
                return;
            }

            _cts = new CancellationTokenSource();
            try
            {
                await Task.Delay(200, _cts.Token); //debounce, fuer wenn wer schnell klickt
                var geoJson = await _logic.GetRouteAsync(_selectedTour, _cts.Token);
                if (_cts.Token.IsCancellationRequested) return;
                SetRoute(geoJson);
            }
            catch (OperationCanceledException)
            {
                log.Warn("Route rendering was cancelled by user.");
                SetRoute("");
                //Kein ErrorOccurred, da kein eigentlicher Fehler!
            }
            catch 
            {
                log.Error("Failed to load route for the selected tour.");
                SetRoute("");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}