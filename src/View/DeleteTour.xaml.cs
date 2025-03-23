using System.Windows;
using TourPlanner.ViewModel;

namespace TourPlanner.View
{
    public partial class DeleteTour : Window
    {
        public DeleteTour()
        {
            InitializeComponent();
            this.DataContext = new TourViewModel();
        }
    }
}