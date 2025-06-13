using System.Windows;
using TourPlanner.ViewModel;
using log4net;

namespace TourPlanner.View
{
    public partial class CreateTour : Window
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));
        public CreateTour()
        {
            InitializeComponent();
            log.Info("in CreateTour.cs CreateTour");
            this.DataContext = new TourViewModel();
        }
    }
}
