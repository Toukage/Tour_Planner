using log4net;
using System.Windows;
using TourPlanner.ViewModel;

namespace TourPlanner.View
{
    public partial class CreateTourLog : Window
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(CreateTourLog));
        public CreateTourLog()
        {
            InitializeComponent();
            log.Info("in CreateTour.cs CreateTour");

            //kann man das auslassen? oder muss das in jedes window rein was so up poppt und dann wieder weg muss?
            this.DataContextChanged += (_, __) =>
            {
                if (DataContext is CreateLogVM vm)
                {
                    // doppelte Abos vermeiden: erst abklemmen, dann anklemmen
                    vm.RequestClose -= OnVmRequestClose;
                    vm.RequestClose += OnVmRequestClose;
                }
            };
            this.Closed += (_, __) =>
            {
                if (DataContext is CreateLogVM vm)
                    vm.RequestClose -= OnVmRequestClose;
            };
        }
        private void OnVmRequestClose() => this.Close();
    }
}
