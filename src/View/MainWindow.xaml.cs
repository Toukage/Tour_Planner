using log4net;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TourPlanner.Model;
using TourPlanner.ViewModel;

namespace TourPlanner.View
{
    public partial class MainWindow : Window
    {
        //instanzen damit ich diese sahcen in der class usen kann! 
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow)); //fuers logs schreiben
        private readonly MainVM _viewModel; //fuers Viewmodel
        private readonly IServiceProvider _sp; //fuer die Dependency Injection services , also alle klasses.
        public MainWindow(MainVM vm, IServiceProvider sp)
        {
            InitializeComponent();
            _viewModel = vm; 
            _sp = sp;

            DataContext = _viewModel;//binding zwischen xaml und ViewModels

            // verbindet events an actions by: action -> VM -> event triggert -> MainVM -> Shell -> action
            _viewModel.RequestOpenCreateTour += OpenCreateTour;
            _viewModel.ReportRequested += OnReport;
            _viewModel.ReverseRequested += OnReverse;
            _viewModel.RequestOpenModifyTour += OnModifyTour;
            _viewModel.LogListVM.CreateRequested += OnCreateLog;
            _viewModel.LogListVM.ModifyRequested += OnModifyLog;
            _viewModel.ErrorOccurred += ShowErrorMessage;
            _viewModel.MapVM.ErrorOccurred += ShowErrorMessage;
            _viewModel.TourListVM.ErrorOccurred += ShowErrorMessage;
            _viewModel.LogListVM.ErrorOccurred += ShowErrorMessage;

            Loaded += MainWindowLoaded; //sobald seite zuende geladen wird soll es die tours laden

            Closed += MainWindowClosed;//das ist nur dafuer da das wenn das fenster geschlossen is, das wir alles unsubscriben undso
        }
        private async void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            //beim Start laeds alle touren
            await _viewModel.LoadToursAsync();
        }
        private void MainWindowClosed(object? sender, EventArgs e)
        {
            //hier wird abgemeldet, des verhindert Memory-Leaks
            _viewModel.RequestOpenCreateTour -= OpenCreateTour;
            _viewModel.ReportRequested -= OnReport;
            _viewModel.ReverseRequested -= OnReverse;
            _viewModel.RequestOpenModifyTour -= OnModifyTour;
            _viewModel.LogListVM.CreateRequested -= OnCreateLog;
            _viewModel.LogListVM.ModifyRequested -= OnModifyLog;
            _viewModel.ErrorOccurred -= ShowErrorMessage;
            _viewModel.MapVM.ErrorOccurred -= ShowErrorMessage;
            _viewModel.TourListVM.ErrorOccurred -= ShowErrorMessage;
            _viewModel.LogListVM.ErrorOccurred -= ShowErrorMessage;
        }

        //-------------Tour-------------
        private void OpenCreateTour()
        {
            var vm = _sp.GetRequiredService<CreateTourVM>();
            var win = _sp.GetRequiredService<CreateTour>();

            //schliest das create window sobald gesaved wird 
            void OnSaved(Tour t)
            {
                _viewModel.OnTourCreated(t);
                win.Close();
            }

            void OnVMClose() => win.Close();
            vm.TourSaved += OnSaved;
            vm.RequestClose += OnVMClose;

            win.Owner = this;
            win.DataContext = vm;//bindet die createview an ihre eigene vm

            win.Closed += Win_Closed;

            win.ShowDialog();//des macht es so das man nicht aufs mainwindow usen kann wenn Create Window offen is.

            void Win_Closed(object? sender, EventArgs e)
            {
                vm.TourSaved -= OnSaved;
                vm.RequestClose -= OnVMClose;
            }
        }

        private void OnModifyTour(Tour? tour)
        {
            if (tour == null) return;

            var vm = _sp.GetRequiredService<ModifyTourVM>();
            var dlg = _sp.GetService<ModifyTour>() ?? new ModifyTour();

            vm.LoadFrom(tour);

            vm.TourSaved += updated =>
            {
                _viewModel.OnTourModified(updated);
                dlg.Close();
            };
            vm.RequestClose += () => dlg.Close();

            dlg.Owner = this;
            dlg.DataContext = vm;
            dlg.ShowDialog();
        }

        //-------------Tour-Logs-------------
        private void OnCreateLog()
        {
            var vm = _sp.GetRequiredService<CreateLogVM>();
            var dlg = _sp.GetService<CreateTourLog>() ?? new CreateTourLog();

            var tour = _viewModel.TourListVM.SelectedItem;
            if (tour == null) return;

            vm.SetTour(tour);
            vm.LogSaved += savedLog =>
            {
                _viewModel.OnLogCreated(savedLog);
                dlg.Close();
            };
            vm.RequestClose += () => dlg.Close();

            dlg.Owner = this;
            dlg.DataContext = vm;
            dlg.ShowDialog();
        }

        private void OnModifyLog(TourPlanner.Model.TourLog? log)
        {
            if (log == null) return;

            var vm = _sp.GetRequiredService<ModifyLogVM>();
            var dlg = _sp.GetService<ModifyLog>() ?? new ModifyLog();

            vm.LoadFrom(log);

            vm.LogSaved += updated =>
            {
                _viewModel.OnLogModified(updated);
                dlg.Close();
            };
            vm.RequestClose += () => dlg.Close();

            dlg.Owner = this;
            dlg.DataContext = vm;
            dlg.ShowDialog();
        }

        //-------------Report-------------
        private async void OnReport(TourPlanner.Model.Tour? tour)
        {
            if (tour == null) return;

            var sfd = new Microsoft.Win32.SaveFileDialog
            {
                FileName = $"Tour_{tour.TourName}_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
                Filter = "PDF (*.pdf)|*.pdf",
                AddExtension = true,
                OverwritePrompt = true
            };
            if (sfd.ShowDialog(this) != true) return;

            byte[]? png = null;
            try
            {
                png = await MapHost.CaptureMapPngAsync(250);
            }
            catch { png = null;  }//map screenshot failed aber keine zeit dafuer also wirds einfach weggelassen 

            try
            {
                var path = await _viewModel.CreateReportAsync(tour, png, sfd.FileName);
                MessageBox.Show(this, $"Report saved:\n{path}", "Report",MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Failed to generate report:\n{ex.Message}", "Error",MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //-------------Unique-Feature-------------
       private async void OnReverse(Tour? tour)
       {
            if (tour == null) return;
            try
            {
                var reversed = await _viewModel.ReverseTourAsync(tour);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Failed to reverse tour:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
       }

        //-------------Error-Handling-------------
        private void ShowErrorMessage(string msg)
        {
            MessageBox.Show(this, msg, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}