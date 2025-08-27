using BusinessLayer;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.Model;
using log4net;
using log4net.Config;

namespace TourPlanner.ViewModel
{
    public class CreateTourViewModel : INotifyPropertyChanged //1. notifies when something changes in code-behind
    {
        private readonly TourLogic _tourLogic;
        private Tour _tour;
        private static readonly ILog log = LogManager.GetLogger(typeof(CreateTourViewModel));

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? RequestClose;

        public CreateTourViewModel()
        {
            _tourLogic = new TourLogic();
            Tour = new Tour();
            CreateTourCommand = new Relay(_ => CreateTour(), _ => CanCreateTour());
        }

        public Tour Tour
        {
            get => _tour;
            set { _tour = value; OnPropertyChanged(nameof(Tour)); }
        }

        //all the Bindings to commands from the View are called here 
        public ICommand CreateTourCommand { get; private set; } 

        private bool CanCreateTour()
        {
            return 
            !string.IsNullOrWhiteSpace(Tour?.TourName) &&
            !string.IsNullOrWhiteSpace(Tour?.TourStart) &&
            !string.IsNullOrWhiteSpace(Tour?.TourEnd) &&
            !string.IsNullOrWhiteSpace(Tour?.Description) &&
            !string.IsNullOrWhiteSpace(Tour?.Transport);
        }
        private void CreateTour()
        {
            try
            {
                log.Debug("CreateTourCommand executed.");
                log.Info($"Creating Tour: {Tour.TourName}  /  {Tour.Description} / {Tour.TourStart} / {Tour.TourEnd} / {Tour.Transport}");
                _tourLogic.CreateNewTour(Tour);
                RequestClose?.Invoke();      
            }
            catch (Exception ex)
            {
                log.Error("Create failed", ex);
            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            CommandManager.InvalidateRequerySuggested();
        }
    }
}
