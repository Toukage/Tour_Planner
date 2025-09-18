using BusinessLayer;
using System.ComponentModel;
using TourPlanner.Model;
using log4net;

namespace TourPlanner.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainVM));
        private readonly TourLogic _logic;
        public TourListVM TourListVM { get; }
        public TourDetailsVM DetailsVM { get; }
        public MapVM MapVM { get; }
        public LogListVM LogListVM { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? RequestOpenCreateTour;
        public event Action<Tour>? RequestOpenModifyTour;
        public event Action<TourLog>? RequestOpenModifyLog;
        public event Action<Tour?>? ReportRequested;

        public MainVM(TourLogic logic, TourListVM listVm, TourDetailsVM detailsVm, MapVM mapVm, LogListVM logListVm)
        {
            _logic = logic;
            TourListVM = listVm;
            DetailsVM = detailsVm;
            MapVM = mapVm;
            LogListVM = logListVm;

            TourListVM.CreateRequested += () => RequestOpenCreateTour?.Invoke();

            TourListVM.SelectionChanged += async tour => //wenn was anderes ausgeweahlt wird dann aendert es die displayed tour info
            {
                DetailsVM.SelectedTour = tour; //updates Details
                MapVM.SelectedTour = tour;  //updates Map
                await LogListVM.LoadForTourAsync(tour); //loads Logs
            };

            TourListVM.ModifyRequested += tour => 
            {
                if (tour != null) RequestOpenModifyTour?.Invoke(tour); //oeffnet ModifyTour Window
            };

            LogListVM.ModifyRequested += tourlog =>
            {
                if (tourlog != null) RequestOpenModifyLog?.Invoke(tourlog); //oeffnet ModifyLog Window
            };

            TourListVM.ReportRequested += t => ReportRequested?.Invoke(t); //startet Report flow
        }
        public void OnTourCreated(Tour tour)
        {
            TourListVM.AddTour(tour);
            TourListVM.SelectedItem = tour;           
        }

        public async Task LoadToursAsync()
        {
            try
            {
                await TourListVM.LoadAsync();//hollt alle Tours
            }
            catch (Exception ex)
            {
                log.Error("[MainViewModel] failed to load tours", ex);
            }
        }

        //wenn ne tour Modified wird wird selection geupdated
        public void OnTourModified(Tour updated)
        {
            TourListVM.RefreshItem(updated);
            TourListVM.SelectedItem = updated;
        }

        public void OnLogModified(TourLog updated)
        {
            LogListVM.RefreshItem(updated);
            LogListVM.Selected = updated;
        }

        public void OnLogCreated(TourLog newLog)//added den neuen Log und selected ihn
        {
            LogListVM.Logs.Insert(0, newLog);
            LogListVM.Selected = newLog;
        }

        public async Task<string> CreateReportAsync(Tour tour, byte[]? mapPng, string path) //Report flow
        {
            await _logic.CreateReportAsync(tour, mapPng, path);
            return path;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}