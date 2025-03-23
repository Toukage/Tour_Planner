using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TourPlanner.ViewModel;


namespace TourPlanner.View
{
    public partial class CreateTourGrid : UserControl
    {
        public CreateTourGrid()
        {
            InitializeComponent();
            this.DataContext = new TourViewModel();
        }

        private void SaveTourBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is CreateTour createTourWindow)
            {
                createTourWindow.Close();
            }
        }
    }
}