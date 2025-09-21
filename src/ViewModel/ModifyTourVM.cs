using BusinessLayer;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.Model;
using log4net;

namespace TourPlanner.ViewModel
{
    public class ModifyTourVM : INotifyPropertyChanged 
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ModifyTourVM));
        private readonly TourLogic _logic;
        private Tour? _original;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? RequestClose;
        public event Action<Tour>? TourSaved;
        public event Action<string>? ErrorOccurred;

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
            SaveCommand = new Relay(async _ => await SaveAsync(), _ => CanSave());
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
        private bool CanSave() //schaut nach das eh alle felder ausgefullt sind. kann man noch ausarbeiten damit es weniger user bedinge errors geben kann
        {
            return IsValidAddress(TourStart) &&
                   IsValidAddress(TourEnd) &&
                   !string.IsNullOrWhiteSpace(TourName) &&
                   !string.IsNullOrWhiteSpace(TourStart) &&
                   !string.IsNullOrWhiteSpace(TourEnd) &&
                   !string.IsNullOrWhiteSpace(TourDescription) &&
                   !string.IsNullOrWhiteSpace(Transport);
        }

        private bool IsValidAddress(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;
            return input.Any(char.IsLetter);
        }
        private async Task SaveAsync()
        {
            if (_original == null)
            {
                ErrorOccurred?.Invoke("Could not load Tour.");
                log.Warn("Could not load Tour..");
                RequestClose?.Invoke();
                return;
            }


            //schaut ob sich iwas geaendert hat
            bool changed =
                !string.Equals(TourName, _original.TourName) ||
                !string.Equals(TourDescription, _original.TourDescription) ||
                !string.Equals(Transport, _original.Transport) ||
                !string.Equals(TourStart, _original.TourStart) ||
                !string.Equals(TourEnd, _original.TourEnd);

            if (!changed)
            {
                ErrorOccurred?.Invoke("Es wurden keine Änderungen durchgeführt.");
                RequestClose?.Invoke();
                return;
            }

            //aendert die alten werte
            _original.TourName = TourName ?? "";
            _original.TourDescription = TourDescription;
            _original.Transport = Transport ?? "Walking";
            _original.TourStart = TourStart ?? "";
            _original.TourEnd = TourEnd ?? "";

            try
            {
                await _logic.ModifyTourAsync(_original);
                TourSaved?.Invoke(_original);
                RequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                log.Error("Failed to modify tour.", ex);
                ErrorOccurred?.Invoke("Tour could not be modified");
                throw new VMExceptions.ModifyTourVMException("Failed to modify tour.", ex);
            }
            finally
            {
                NotifyAll();
            }
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
