using System.ComponentModel;
using BusinessLayer;
using TourPlanner.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using log4net;

namespace TourPlanner.ViewModel
{
    public class TourListVM : INotifyPropertyChanged
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TourListVM));

        private readonly TourLogic _logic;
        private bool _muteEvents;
        private Tour? _selectedItem;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? CreateRequested;
        public event Action<Tour?>? SelectionChanged;
        public event Action<Tour?>? ReportRequested;
        public event Action<Tour?>? ModifyRequested;
        public event Action<Tour?>? ReverseRequested;
        public event Action<string>? ErrorOccurred;
        //Tour buttons
        public ICommand OpenCreateTourCommand { get; }
        public ICommand ReportTourCommand { get; }
        public ICommand DeleteTourCommand { get; }
        public ICommand ModifyTourCommand { get; }
        public ICommand ReverseTourCommand { get; }

        public ObservableCollection<Tour> Tours { get; } = new();

        public Tour? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value) return;

                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                CommandManager.InvalidateRequerySuggested();

                if (!_muteEvents) SelectionChanged?.Invoke(_selectedItem);

                log.Info($"SelectedItem => {_selectedItem?.TourID}:{_selectedItem?.TourName}");
            }
        }

        public TourListVM(TourLogic logic)//btn logik
        {
            _logic = logic;
            OpenCreateTourCommand = new Relay(_ => CreateRequested?.Invoke());
            ReportTourCommand = new Relay(_ => ReportRequested?.Invoke(SelectedItem),_ => SelectedItem != null);
            ReverseTourCommand = new Relay(_ => ReverseRequested?.Invoke(SelectedItem), _ => SelectedItem != null);
            DeleteTourCommand = new Relay(async _ =>
            {
                if (SelectedItem == null) return;
                var toDelete = SelectedItem;
                try
                {
                    await _logic.DeleteTourAsync(toDelete);
                    Tours.Remove(toDelete);
                    SelectedItem = Tours.OrderByDescending(t => t.TourID).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    log.Error("Failed to delete tour.", ex);
                    ErrorOccurred?.Invoke("Failed to delete tour.");
                    throw new VMExceptions.TourListVMException("Failed to delete tour.", ex);
                }
            }, _ => SelectedItem != null);

            ModifyTourCommand = new Relay(_ =>
            {
                if (SelectedItem == null) return;
                ModifyRequested?.Invoke(SelectedItem); 
            },_ => SelectedItem != null);
        }

        public async Task LoadAsync()//laedt alle tours fuer die List-View
        {
            try
            {

                var list = await _logic.GetAllToursAsync();
                _muteEvents = true;
                try
                {
                    Tours.Clear();
                    foreach (var t in list) Tours.Add(t);

                    SelectedItem = Tours.OrderByDescending(t => t.TourID).FirstOrDefault();
                }
                finally
                {
                    _muteEvents = false;
                }

                SelectionChanged?.Invoke(SelectedItem);
            }
            catch (Exception ex)
            {
                log.Error("Failed to load tours.", ex);
                ErrorOccurred?.Invoke("Failed to load tours.");
                throw new VMExceptions.TourListVMException("Failed to load tours.", ex);
            }
        }

        public void RefreshItem(Tour updated)//re-laedt selection wenn die tour zb modified wurde und die neue version angezeigt werden soll.
        {
            var idx = Tours.ToList().FindIndex(t => t.TourID == updated.TourID);
            if (idx < 0) return;

            var wasSelected = ReferenceEquals(SelectedItem, Tours[idx]);
            Tours.RemoveAt(idx);
            Tours.Insert(idx, updated);

            if (wasSelected) SelectedItem = updated;
        }

        public void AddTour(Tour tour) //adds tour ins model aka observable
        {
            Tours.Add(tour);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
