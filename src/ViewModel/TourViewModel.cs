using BusinessLayer;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.Model;
using log4net;
using log4net.Config;

namespace TourPlanner.ViewModel
{
    public class TourViewModel : INotifyPropertyChanged
    {
        private readonly TourLogic _tourLogic;
        private Tour _tour;
        private static readonly ILog log = LogManager.GetLogger(typeof(TourViewModel));

        public event PropertyChangedEventHandler PropertyChanged;

        public TourViewModel()
        {
            _tourLogic = new TourLogic();
            Tour = new Tour();
            XmlConfigurator.Configure();
            CreateTourCommand = new Relay(CreateTour);
            DeleteTourCommand = new Relay(DeleteTour);
        }

        public Tour Tour
        {
            get => _tour;
            set { _tour = value; OnPropertyChanged(nameof(Tour)); }
        }

        public ICommand CreateTourCommand { get; private set; }
        public ICommand DeleteTourCommand { get; private set; }


        private void CreateTour()
        {
            log.Debug("CreateTourCommand executed.");
            log.Info($"Creating Tour: {Tour.TourName}  /  {Tour.Description} / {Tour.TourStart} / {Tour.TourEnd} / {Tour.Transport}");
            _tourLogic.CreateNewTour(Tour);
        }

        private void DeleteTour()
        {
            log.Debug("DeleteTourCommand executed.");
            log.Info($"Deleting Tour: {Tour.TourName}");
            _tourLogic.DeleteTour(Tour);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}