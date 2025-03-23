using BusinessLayer;
using log4net.Config;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.Model;

namespace TourPlanner.ViewModel
{
    public class TourViewModel : INotifyPropertyChanged
    {
        private readonly TourLogic _tourLogic;
        private Tour _tour;

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
            _tourLogic.CreateNewTour(Tour);
        }

        private void DeleteTour()
        {
            _tourLogic.DeleteTour(Tour);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}