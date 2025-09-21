using BusinessLayer;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.Model;
using log4net;

namespace TourPlanner.ViewModel
{
    public class CreateTourVM : INotifyPropertyChanged //Signals changes to the View via INotifyPropertyChanged
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CreateTourVM));

        private readonly TourLogic _tourLogic; //Von services 
        private Tour _tour = new Tour();
        private bool _isCreating; //schaut ob gerade eine Tour Created wird
        
       
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action? RequestClose;
        public event Action<Tour>? TourSaved;
        public event Action<string>? ErrorOccurred;

        public ICommand CreateTourCommand { get; } 
        
        public CreateTourVM(TourLogic tourLogic)
        {
            _tourLogic = tourLogic;
            CreateTourCommand = new Relay(async _ =>await CreateTourAsync(), _ => CanCreateTour() && !_isCreating);//relay fuer den binding command im view , schaut das nur dann eine tour created wird wenn alle felder befüllt werden und gerade keine andere ceated wird ( sollte eh nicht der fall sein aber trd)
        }

        public Tour Tour
        {
            get => _tour;
            set
            {
                if (!Equals(_tour, value))//checks ob die tour die gleiche ist, passiert eigentlich nicht aber just in case
                {
                    _tour = value ?? new Tour();
                    OnPropertyChanged(nameof(Tour));
                    CommandManager.InvalidateRequerySuggested();//updated den button der an createtourcommand gebunden ist
                }
            }
        }

        private bool CanCreateTour() //schaut nach das eh alle felder ausgefullt sind. kann man noch ausarbeiten damit es weniger user bedinge errors geben kann
        {
            return IsValidAddress(Tour?.TourStart) &&
            IsValidAddress(Tour?.TourEnd) && 
            !string.IsNullOrWhiteSpace(Tour?.TourName) &&
            !string.IsNullOrWhiteSpace(Tour?.TourStart) &&
            !string.IsNullOrWhiteSpace(Tour?.TourEnd) &&
            !string.IsNullOrWhiteSpace(Tour?.TourDescription) &&
            !string.IsNullOrWhiteSpace(Tour?.Transport);
        }

        private bool IsValidAddress(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;
            //Mindestens ein Buchstabe (A-Z, a-z) muss drin sein
            return input.Any(char.IsLetter);
        }
        private async Task CreateTourAsync()
        {
            if (_isCreating) return; //stoppt wenn schon eine tour created wird      
            _isCreating = true; //sagt das eine tour gerade created wird
            CommandManager.InvalidateRequerySuggested(); //disabled button sobald eine tour creation gestartet wird
            
            try
            {
                log.Info($"Creating Tour: {Tour.TourName} / {Tour.TourDescription} / {Tour.TourStart} / {Tour.TourEnd} / {Tour.Transport}");

                if (!CanCreateTour())
                {
                    ErrorOccurred?.Invoke("Please fill in all fields to create a tour.");
                    return;
                }
                await _tourLogic.CreateTourAsync(Tour); //ruft die logik im BL auf welches die tour speichert

                TourSaved?.Invoke(Tour); //sagt dem Shell aka dem MainWindow das es eine neue tour gibt damit diese displayed wird.

                RequestClose?.Invoke();      
            }
            catch (Exception ex)
            {
                log.Error("Toue Creation failed", ex);
                ErrorOccurred?.Invoke("The tour could not be created. Please try again.");
                throw new VMExceptions.CreateTourVMException("Failed to create tour.", ex);
            }
            finally
            {
                _isCreating = false;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
