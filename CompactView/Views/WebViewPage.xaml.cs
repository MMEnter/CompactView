using CompactView.Helpers;
using CompactView.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CompactView.Views
{
    public sealed partial class WebViewPage : Page, INotifyPropertyChanged
    {
        // TODO UWPTemplates: Set your hyperlink default here
       static AppSettings appSettings = new AppSettings();

        private Uri _source = new Uri(appSettings.Uri);
        public Uri Source
        {
            get { return _source; }
            set { Set(ref _source, value); }
        }

        public WebViewPage()
        {
            InitializeComponent();
            if (ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.CompactOverlay))
            {
                MiniMode.Visibility = Visibility.Visible;
            }

            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            TitleBar.Height = coreTitleBar.Height;
            Window.Current.SetTitleBar(MainTitleBar);

            Window.Current.Activated += Current_Activated;
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            webView1.Source = _source;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState != CoreWindowActivationState.Deactivated)
            {
                BackButtonGrid.Visibility = Visibility.Visible;
                MainTitleBar.Opacity = 1;

                TitleBar.Visibility = Visibility.Visible;
            }
            else
            {
                BackButtonGrid.Visibility = Visibility.Collapsed;
                MainTitleBar.Opacity = 0.5;

                var view = ApplicationView.GetForCurrentView();
                if (view.ViewMode.ToString() == "CompactOverlay")
                {
                    TitleBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar titleBar, object args)
        {
            var view = ApplicationView.GetForCurrentView();
            if (view.ViewMode.ToString() != "CompactOverlay")
            {
                TitleBar.Visibility = titleBar.IsVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            TitleBar.Height = sender.Height;
            RightMask.Width = sender.SystemOverlayRightInset;
        }

        private async void MiniView_Click(object sender, RoutedEventArgs e)
        {
            var view = ApplicationView.GetForCurrentView();
            if (view.ViewMode.ToString() != "CompactOverlay")
            {
                bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
            }
            else
            {
                bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);
            }
        }

        private void Focus_Click(object sender, RoutedEventArgs e)
        {
            var view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode)
            {
                view.ExitFullScreenMode();
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
                // The SizeChanged event will be raised when the exit from full-screen mode is complete.
                //CommandBar.Visibility = Visibility.Visible;
            }
            else
            {
                if (view.TryEnterFullScreenMode())
                {
                    ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
                    // The SizeChanged event will be raised when the entry to full-screen mode is complete.
                    //CommandBar.Visibility = Visibility.Collapsed;
                }
            }

        }

        private bool IsFullscreen()
        {
            var view = ApplicationView.GetForCurrentView();

            if (view.IsFullScreenMode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!IsFullscreen())
            {
                //CommandBar.Visibility = Visibility.Visible;
            }
        }

        private async Task SourceUpdated()
        {
            var appSettings = new AppSettings();
            bool validUri = true;
            string newUri = textBox.Text.ToString();
            Uri uri;

            try
            {
                uri = new Uri(newUri);
            }
            catch
            {
                // Create the message dialog and set its content
                var messageDialog = new MessageDialog("The URL entered was not a valid URL.");
                appSettings.Uri = "https://www.netflix.com";
                validUri = false;
            }

            if (validUri)
            {
                appSettings.Uri = newUri;
                _source = new Uri(newUri);
                webView1.Source = _source;
            }
        }

        private void textBox_TextCompositionEnded(TextBox sender, TextCompositionEndedEventArgs args)
        {
            SourceUpdated();
        }

        private void textBox_ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
            SourceUpdated();
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            SourceUpdated();
        }
    }
}
