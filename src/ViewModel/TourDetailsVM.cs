using System.ComponentModel;
using TourPlanner.Model;

namespace TourPlanner.ViewModel
{
    public class TourDetailsVM : INotifyPropertyChanged
    {
        private Tour? _selectedTour;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Tour? SelectedTour
        {
            get => _selectedTour;
            set
            {
                if (_selectedTour == value) return;
                _selectedTour = value;
                OnPropertyChanged(nameof(SelectedTour));
            }
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
