using System.Configuration;
using System.Data;
using System.Windows;

namespace TourPlanner.View
{
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }

}
