using System.Windows;
using TourPlanner.ViewModel;
using log4net;

namespace TourPlanner.View
{
    public partial class ModifyTour : Window
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));
        public ModifyTour()
        {
            InitializeComponent();
            log.Info("in ModifyTour.cs ModifyTour");
            var viewModel = new ModifyTourViewModel();
            viewModel.RequestClose += () => this.Close();
            this.DataContext = viewModel;
        }
    }
}
