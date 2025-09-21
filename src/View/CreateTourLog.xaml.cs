using log4net;
using System.Windows;
using TourPlanner.ViewModel;

namespace TourPlanner.View
{
    public partial class CreateTourLog : Window
    {
        public CreateTourLog()
        {
            InitializeComponent();
            this.DataContextChanged += (_, __) =>
            {
                if (DataContext is CreateLogVM vm)
                {
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
