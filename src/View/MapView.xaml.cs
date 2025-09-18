using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using log4net;
using Microsoft.Web.WebView2.Core;
using System.Runtime.Versioning;
using BusinessLayer;

namespace TourPlanner.View
{
    public partial class MapView : UserControl
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MapView));
        private string? _lastGeoJson;   //vermeidet unnötige Writes <- ins protocol pls
        private bool _coreReady; //wartet drauf das die leaflet seite zuende laedt bevor wir screenshot machen sonst kann es unfinished sein

        public MapView()
        {
            InitializeComponent(); 
            webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;

            Loaded += MapViewLoaded; 
            Unloaded += MapViewUnloaded;
        }
        private async void MapViewLoaded(object sender, RoutedEventArgs e)
        {
            await webView.EnsureCoreWebView2Async(null);
            _coreReady = webView.CoreWebView2 != null;
            if (!string.IsNullOrWhiteSpace(GeoJson)){
                await RenderAsync("Loaded");
            }
        }
        private void MapViewUnloaded(object? sender, EventArgs e)
        {
            webView.CoreWebView2InitializationCompleted -= WebView_CoreWebView2InitializationCompleted;
        }

        public string GeoJson
        {
            get => (string)GetValue(GeoJsonProperty);
            set => SetValue(GeoJsonProperty, value);
        }

        public static readonly DependencyProperty GeoJsonProperty =
             DependencyProperty.Register(
                 nameof(GeoJson),
                 typeof(string),
                 typeof(MapView),
                 new PropertyMetadata("", OnGeoJsonChanged)
             );

        private async static void OnGeoJsonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mv = d as MapView;
            if (mv != null && mv.IsLoaded && !string.IsNullOrWhiteSpace(mv.GeoJson))
            {
                await mv.RenderAsync("GeoJsonChanged");
            }
        }

        private void WebView_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                log.Warn($"[MapView] WebView2 init failed: {e.InitializationException?.Message}");
                _coreReady = false;
                return;
            }

            log.Info("[MapView] WebView2 ready.");

            _coreReady = true;

        }

        private int _renderCount;
        private async Task RenderAsync(string reason)
        {
            log.Info($"[MapView] Render start (reason={reason}) .");
            log.Info($"[MapView] Render #{++_renderCount} .");

            if (!_coreReady || webView.CoreWebView2 == null)
            {
                await webView.EnsureCoreWebView2Async(null);
                if (webView.CoreWebView2 == null) return;
                _coreReady = true;
            }

            //gets resources for rendering the Maps
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var mapDir = Path.Combine(baseDir, "Resources", "Map");
            Directory.CreateDirectory(mapDir);
            var jsPath = Path.Combine(mapDir, "directions.js");
            var html = Path.Combine(mapDir, "leaflet.html");

            //write directions.js
            var json = string.IsNullOrWhiteSpace(GeoJson) ? "{}" : GeoJson;
            log.Info($"[MapView] GeoJson length={json.Length}");

            if (!string.Equals(json, _lastGeoJson, StringComparison.Ordinal))
            {
                try
                {
                    File.WriteAllText(jsPath, "var directions = " + json + ";");
                    _lastGeoJson = json;
                }
                catch (Exception ex)
                {
                    log.Error("[MapView] Failed to write directions.js", ex);
                }
            }

            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            EventHandler<CoreWebView2NavigationCompletedEventArgs>? handler = null;
            handler = (s, e) =>
            {
                try { tcs.TrySetResult(e.IsSuccess); }
                finally
                {                   
                    if (webView?.CoreWebView2 != null)
                        webView.CoreWebView2.NavigationCompleted -= handler;
                }
            };

            webView.CoreWebView2.NavigationCompleted += handler;

            var uri = new Uri(html).AbsoluteUri + "?v=" + DateTime.UtcNow.Ticks;
            webView.CoreWebView2.Navigate(uri);

            var finished = await Task.WhenAny(tcs.Task, Task.Delay(8000));
            if (finished != tcs.Task || !tcs.Task.Result)
            {
                log.Warn("[MapView] Navigation timeout or error.");
            }
        }

        public async Task<byte[]?> CaptureMapPngAsync(int delayAfterLoadMs = 250, CancellationToken ct = default)
        {
            if (webView?.CoreWebView2 == null) return null;

            await Task.Delay(delayAfterLoadMs, ct);//wartet bis es fertig is

            using (var ms = new MemoryStream())
            {
                await webView.CoreWebView2.CapturePreviewAsync(CoreWebView2CapturePreviewImageFormat.Png, ms);
                return ms.ToArray();
            }
        }

    }
}
