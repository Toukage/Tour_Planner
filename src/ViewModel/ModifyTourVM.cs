using BusinessLayer;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.Model;
using System.Globalization;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace TourPlanner.ViewModel
{
    public class ModifyTourVM : INotifyPropertyChanged 
    {
        private readonly TourLogic _logic;
        private Tour? _original;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? RequestClose;
        public event Action<Tour>? TourSaved;

        public string? TourName { get; set; }
        public string? TourDescription { get; set; }
        public string? Transport { get; set; }
        public string? TourStart { get; set; }
        public string? TourEnd { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ModifyTourVM(TourLogic logic)
        {
            _logic = logic;
            SaveCommand = new Relay(async _ => await SaveAsync());
            CancelCommand = new Relay(_ => RequestClose?.Invoke());
        }

        public void LoadFrom(Tour tour) //populated die input felder mit den tour daten
        {
            _original = tour;
            TourName = tour.TourName;
            TourDescription = tour.TourDescription;
            Transport = tour.Transport;
            TourStart = tour.TourStart;
            TourEnd = tour.TourEnd;

            NotifyAll();
        }

        private async Task SaveAsync()
        {
            if (_original == null) { RequestClose?.Invoke(); return; }

            //schaut ob sich iwas geaendert hat
            bool changed =
                !string.Equals(TourName, _original.TourName) ||
                !string.Equals(TourDescription, _original.TourDescription) ||
                !string.Equals(Transport, _original.Transport) ||
                !string.Equals(TourStart, _original.TourStart) ||
                !string.Equals(TourEnd, _original.TourEnd);

            if (!changed) { RequestClose?.Invoke(); return; }

            //aendert die alten werte
            _original.TourName = TourName ?? "";
            _original.TourDescription = TourDescription;
            _original.Transport = Transport ?? "Walking";
            _original.TourStart = TourStart ?? "";
            _original.TourEnd = TourEnd ?? "";

            await _logic.ModifyTourAsync(_original);
            NotifyAll();
            TourSaved?.Invoke(_original);
            RequestClose?.Invoke();
        }

        private void NotifyAll()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TourName)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TourDescription)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Transport)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TourStart)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TourEnd)));
        }
    }
}
