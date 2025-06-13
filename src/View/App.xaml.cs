using log4net;
using log4net.Config;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows;

namespace TourPlanner.View
{
    public partial class App : Application
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));

        [STAThread]
        public static void Main()
        {
            var logConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()), new FileInfo(logConfigFile));

            log.Info("----------------> NEW LOG START <----------------");

            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}