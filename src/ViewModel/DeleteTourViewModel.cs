using BusinessLayer;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.Model;
using log4net;
using log4net.Config;

namespace TourPlanner.ViewModel
{
    public class DeleteTourViewModel : INotifyPropertyChanged //1. notifies when something changes in code-behind
    {
        private readonly TourLogic _tourLogic;
        private Tour _tour;
        private static readonly ILog log = LogManager.GetLogger(typeof(DeleteTourViewModel));

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? RequestClose;

        public DeleteTourViewModel()
        {
            _tourLogic = new TourLogic();
            Tour = new Tour();
            DeleteTourCommand = new Relay(_ => DeleteTour(), _ => CanDeleteTour());
        }

        public Tour Tour
        {
            get => _tour;
            set { _tour = value; OnPropertyChanged(nameof(Tour)); }
        }

        //all the Bindings to commands from the View are called here 
        public ICommand DeleteTourCommand { get; private set; }

        private bool CanDeleteTour()
        {
            return 
            !string.IsNullOrWhiteSpace(Tour?.TourName);
        }
        private void DeleteTour()
        {
            try
            {
                log.Debug("DeleteTourCommand executed.");
                log.Info($"Deleting Tour: {Tour.TourName}");
                _tourLogic.DeleteTour(Tour);
                RequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                log.Error("Delete failed", ex);
            }
           
            
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            CommandManager.InvalidateRequerySuggested();
        }
    }
}
