using System.Windows;
using TourPlanner.ViewModel;

namespace TourPlanner.View
{
    public partial class CreateTour : Window
    {
        public CreateTour()
        {
            InitializeComponent();
            this.DataContextChanged += (_, __) =>
            {
                if (DataContext is CreateTourVM vm)//schaut ob eh die richte vm das requestclose mitgibt damit das richtige fenster geschlossen wird
                {
                    vm.RequestClose -= OnVmRequestClose;
                    vm.RequestClose += OnVmRequestClose;
                }
            };
            this.Closed += (_, __) =>
            {
                if (DataContext is CreateTourVM vm)
                    vm.RequestClose -= OnVmRequestClose;
            };
        }
        private void OnVmRequestClose() => this.Close();
    }
}

// gerade etwas schlampig, maybe interface machen damit es nicht hardcoded die VM anschaut? wegen Dependency Inversion Principle?