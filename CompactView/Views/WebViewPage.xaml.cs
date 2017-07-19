using CompactView.Helpers;
using CompactView.Models;
using CompactView.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string Name = e.Parameter.ToString();

            List<Website> websites = UseWebsite.GetList();

            Website website = websites.Find(
                delegate(Website site)
                {
                    return site.Name == Name;
                }                
                );
            if (website != null)
            {
                _source = website.URL;
                webView1.Source = _source;
            }
        }

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
            ModeChanged();
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
            ModeChanged();
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
            ModeChanged();
        }

        private async void Normal_Click(object sender, RoutedEventArgs e)
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
            ModeChanged();
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
        private bool IsCompactview()
        {
            var view = ApplicationView.GetForCurrentView();
            if (view.ViewMode.ToString() == "CompactOverlay")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsNormalscreen()
        {
            var view = ApplicationView.GetForCurrentView();

            if (!IsFullscreen() && !IsCompactview())
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
            ModeChanged();
        }

        private void ModeChanged()
        {
            //FocuseMode
            if (!IsFullscreen())
            {
                FocuseMode.Visibility = Visibility.Visible;
            }
            else
            {
                FocuseMode.Visibility = Visibility.Collapsed;
            }
            //MiniMode
            if (!IsCompactview())
            {
                MiniMode.Visibility = Visibility.Visible;
                textBox.Visibility = Visibility.Visible;
                Go.Visibility = Visibility.Visible;
                SpaceForHamburger.Width = new GridLength(0, GridUnitType.Star);
            }
            else
            {
                MiniMode.Visibility = Visibility.Collapsed;
                textBox.Visibility = Visibility.Collapsed;
                Go.Visibility = Visibility.Collapsed;
                SpaceForHamburger.Width = new GridLength(48, GridUnitType.Pixel);
            }
            //NormalMode
            if (!IsNormalscreen())
            {
                NormalMode.Visibility = Visibility.Visible;
            }
            else
            {
                NormalMode.Visibility = Visibility.Collapsed;
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
                DisplayDialog("Error", "The URL entered was not a valid URL. A URL has to look like http://www.bing.com ");
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

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            SourceUpdated();
        }

        private async void PastAndGo_ClickAsync(object sender, RoutedEventArgs e)
        {
            var dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                try
                {
                    var text = await dataPackageView.GetTextAsync();
                    textBox.Text = text.ToString();
                    await SourceUpdated();
                }
                catch (Exception ex)
                {
                    DisplayDialog("Error", "Could not get the URL from the Clipboard.");
                }
            }
            else
            {
            }
        }

        private void textBox_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                SourceUpdated();
            }
        }

        private async void DisplayDialog(string title, string content)
        {
            ContentDialog noWifiDialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await noWifiDialog.ShowAsync();
        }

        private void webView1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ModeChanged();
        }
    }
}
