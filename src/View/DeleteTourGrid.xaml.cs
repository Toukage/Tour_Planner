using log4net;
using System.Windows;
using System.Windows.Controls;
using TourPlanner.ViewModel;


namespace TourPlanner.View
{
    public partial class DeleteTourGrid : UserControl
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));
        public DeleteTourGrid()
        {
            InitializeComponent();
            log.Info("DeleteTourGrid");
            this.DataContext = new TourViewModel();
        }
        private void DropTourBtn_Click(object sender, RoutedEventArgs e)
        {
            log.Info("dropping the tour");
            if (Window.GetWindow(this) is DeleteTour deleteTourWindow)
            {
                deleteTourWindow.Close();
            }
            else
            {
                log.Warn("could not find parent DeleteTour window.");
            }
        }
    }
}
