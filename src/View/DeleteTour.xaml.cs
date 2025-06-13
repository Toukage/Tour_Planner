using System.Windows;
using TourPlanner.ViewModel;
using log4net;

namespace TourPlanner.View
{
    public partial class DeleteTour : Window
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));
        public DeleteTour()
        {
            InitializeComponent();
            log.Info("in DeleteTour.cs DeleteTour");
            this.DataContext = new TourViewModel();
        }
    }
}
