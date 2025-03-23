using System.Windows;
using System.Windows.Controls;
using TourPlanner.ViewModel;


namespace TourPlanner.View
{
    public partial class DeleteTourGrid : UserControl
    {
        public DeleteTourGrid()
        {
            InitializeComponent();
            this.DataContext = new TourViewModel();
        }
        private void DropTourBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is DeleteTour deleteTourWindow)
            {
                deleteTourWindow.Close();
            }
        }
    }
}