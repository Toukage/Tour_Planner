using BusinessLayer;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.Model;
using log4net;
using log4net.Config;

namespace TourPlanner.ViewModel
{
    public class ModifyTourViewModel : INotifyPropertyChanged //1. notifies when something changes in code-behind
    {
        private readonly TourLogic _tourLogic;
        private Tour _tour;
        private static readonly ILog log = LogManager.GetLogger(typeof(ModifyTourViewModel));

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? RequestClose;

        public ModifyTourViewModel()
        {
            _tourLogic = new TourLogic();
            Tour = new Tour();
            ModifyTourCommand = new Relay(_ => ModifyTour(), _ => CanModifyTour());
        }

        public Tour Tour
        {
            get => _tour;
            set { _tour = value; OnPropertyChanged(nameof(Tour)); }
        }

        //all the Bindings to commands from the View are called here 
        public ICommand ModifyTourCommand { get; private set; }

        private bool CanModifyTour()
        {
            return
            !string.IsNullOrWhiteSpace(Tour?.TourName) &&
            !string.IsNullOrWhiteSpace(Tour?.TourStart) &&
            !string.IsNullOrWhiteSpace(Tour?.TourEnd) &&
            !string.IsNullOrWhiteSpace(Tour?.Description) &&
            !string.IsNullOrWhiteSpace(Tour?.Transport);
        }
        private void ModifyTour()
        {
            try
            {
                log.Debug("ModifyTourCommand executed.");
                log.Info($"Modifying Tour: {Tour.TourName}  /  {Tour.Description} / {Tour.TourStart} / {Tour.TourEnd} / {Tour.Transport}");
                _tourLogic.ModifyTour(Tour);
                RequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                log.Error("Modify failed", ex);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            CommandManager.InvalidateRequerySuggested();
        }
    }
}
