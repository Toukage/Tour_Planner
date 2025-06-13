using log4net;
using log4net.Config;
using System.Windows;


namespace TourPlanner.View
{
    public partial class MainWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            log.Info("creaing Tour");
            CreateTour createTourWindow = new CreateTour();
            createTourWindow.ShowDialog();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            log.Info("removing tour");
            DeleteTour deleteTourWindow = new DeleteTour();
            deleteTourWindow.ShowDialog();
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            log.Info("Editing Tour");
        }
    }
}