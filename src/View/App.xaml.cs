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
            // 1) BaseDir = Ordner der laufenden EXE (bin\Debug\...\)
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // 2) logs-Unterordner erzeugen
            var logsDir = Path.Combine(baseDir, "logs");
            Directory.CreateDirectory(logsDir);

            // 3) log4net-Property setzen, die in der XML verwendet wird
            log4net.GlobalContext.Properties["LogDir"] = logsDir;

            // 4) log4net.config laden
            var logConfigFile = Path.Combine(baseDir, "log4net.config");
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()),
                                      new FileInfo(logConfigFile));

            log.Info("----------------> NEW LOG START <----------------");

            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}