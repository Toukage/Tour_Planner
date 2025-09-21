using BusinessLayer;
using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Windows;

namespace TourPlanner.View
{
    public partial class App : Application
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));
        private ServiceProvider? _services;

        [STAThread]
        public static void Main()
        {
            InitLogging();
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            this.DispatcherUnhandledException += (_, args) =>
            {
                log.Error("UI thread exception", args.Exception);
                args.Handled = true;
            };
            AppDomain.CurrentDomain.UnhandledException += (_, args2) =>
            {
                log.Fatal("Non-UI thread exception", args2.ExceptionObject as Exception);
            };
            TaskScheduler.UnobservedTaskException += (_, args3) =>
            {
                log.Error("Unobserved task exception", args3.Exception);
                args3.SetObserved();
            };

            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            _services = services.BuildServiceProvider();

            log.Info("----------------> NEW LOG START <----------------");
            var mainWindow = _services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            log.Info("Application exiting.");
            _services?.Dispose();
            log4net.LogManager.Shutdown();
            base.OnExit(e);
        }

        private static void InitLogging()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var logsDir = Path.Combine(baseDir, "Logging");
            Directory.CreateDirectory(logsDir);
            log4net.GlobalContext.Properties["LogDir"] = logsDir;

            var logConfigFile = Path.Combine(baseDir, "log4net.config");
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()), new FileInfo(logConfigFile));
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            //Config
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var orsApiKey = config["OpenRouteService:ApiKey"]
               ?? Environment.GetEnvironmentVariable("ORS_API_KEY")
               ?? "";

            if (string.IsNullOrWhiteSpace(orsApiKey))
            {
                MessageBox.Show("Kein OpenRouteService API Key gefunden!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new InvalidOperationException("OpenRouteService API key fehlt!");
            }

            services.AddSingleton(sp => new GeoCode(sp.GetRequiredService<HttpClient>(), orsApiKey));
            services.AddSingleton(sp => new Directions(sp.GetRequiredService<HttpClient>(), orsApiKey));
            services.AddSingleton<BusinessLayer.Interfaces.IGeoCode>(sp => new GeoCode(sp.GetRequiredService<HttpClient>(), orsApiKey));
            services.AddSingleton<BusinessLayer.Interfaces.IDirections>(sp => new Directions(sp.GetRequiredService<HttpClient>(), orsApiKey));

            //DAL
            var connString = config.GetConnectionString("Default");
            services.AddSingleton<IConfiguration>(config);

            services.AddDbContextFactory<DataAccessLayer.DatabaseManager>(opt =>
            {
                opt.UseNpgsql(connString);
            });

            services.AddSingleton(new HttpClient { Timeout = TimeSpan.FromSeconds(20) });
            services.AddSingleton<DataAccessLayer.TourRepo>();
            services.AddSingleton<DataAccessLayer.LogRepo>();
            services.AddSingleton<DataAccessLayer.Interfaces.ITourRepo, DataAccessLayer.TourRepo>();
            services.AddSingleton<DataAccessLayer.Interfaces.ILogRepo, DataAccessLayer.LogRepo>();

            //BL
            services.AddSingleton<Routing>();
            services.AddSingleton<TourLogic>();
            services.AddSingleton<LogLogic>();
            services.AddSingleton<Report>();
            services.AddSingleton<BusinessLayer.Interfaces.ITourLogic, TourLogic>();
            services.AddSingleton<BusinessLayer.Interfaces.ILogLogic, LogLogic>();
            services.AddSingleton<BusinessLayer.Interfaces.IRouting, Routing>();
            services.AddSingleton<BusinessLayer.Interfaces.IReport, Report>();

            //View Models
            services.AddSingleton<TourPlanner.ViewModel.TourListVM>();
            services.AddSingleton<TourPlanner.ViewModel.LogListVM>();
            services.AddSingleton<TourPlanner.ViewModel.TourDetailsVM>();
            services.AddSingleton<TourPlanner.ViewModel.MapVM>();
            services.AddSingleton<TourPlanner.ViewModel.MainVM>();
            services.AddTransient<TourPlanner.ViewModel.CreateTourVM>();
            services.AddTransient<TourPlanner.ViewModel.CreateLogVM>();
            services.AddTransient <TourPlanner.ViewModel.ModifyTourVM>();
            services.AddTransient<TourPlanner.ViewModel.ModifyLogVM>();
            //Views
            services.AddTransient<TourPlanner.View.MainWindow>();
            services.AddTransient<TourPlanner.View.CreateTour>(); 
            services.AddTransient<TourPlanner.View.CreateTourLog>();
            services.AddTransient<TourPlanner.View.ModifyTour>();
            services.AddTransient<TourPlanner.View.ModifyLog>();
            services.AddTransient<TourPlanner.View.LogListView>();
            services.AddTransient<TourPlanner.View.TourListView>();
            services.AddTransient<TourPlanner.View.TourDetailsView>();
        }
    }
}