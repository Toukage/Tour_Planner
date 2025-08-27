using BusinessLayer;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.Model;
using log4net;
using log4net.Config;

namespace TourPlanner.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged //1. notifies when something changes in code-behind
    {
        private readonly TourLogic _tourLogic;
        private Tour _tour;
        private static readonly ILog log = LogManager.GetLogger(typeof(MainViewModel));

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? RequestClose;

        public MainViewModel()
        {
            _tourLogic = new TourLogic();
            Tour = new Tour();
        }

        public Tour Tour
        {
            get => _tour;
            set { _tour = value; OnPropertyChanged(nameof(Tour)); }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            CommandManager.InvalidateRequerySuggested();
        }
    }
}
