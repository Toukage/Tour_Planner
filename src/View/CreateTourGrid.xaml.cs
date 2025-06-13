using log4net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TourPlanner.ViewModel;

namespace TourPlanner.View
{
    public partial class CreateTourGrid : UserControl
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));
        public CreateTourGrid()
        {
            InitializeComponent();
            log.Info("CreateTourGrid");
            this.DataContext = new TourViewModel();
        }
        private void SaveTourBtn_Click(object sender, RoutedEventArgs e)
        {
            log.Info("saving the tour");
            if (Window.GetWindow(this) is CreateTour createTourWindow)
            {
                createTourWindow.Close();
            }
            else
            {
                log.Warn("could not find parent CreateTour window.");
            }
        }
    }
}
