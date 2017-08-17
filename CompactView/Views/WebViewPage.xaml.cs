using CompactView.Data;
using CompactView.Helpers;
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
        private static DependencyProperty s_websiteProperty
            = DependencyProperty.Register("Website", typeof(WebsiteViewModel), typeof(WebViewPage), new PropertyMetadata(null));

        public static DependencyProperty WebsiteProperty
        {
            get { return s_websiteProperty; }
        }

        public WebsiteViewModel Website
        {
            get { return (WebsiteViewModel)GetValue(s_websiteProperty); }
            set { SetValue(s_websiteProperty, value); }
        }

        public Uri uri;

        public WebViewPage()
        {
            InitializeComponent();
            //Checks if the OS version is supporting CompactOverlay.
            if (ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.CompactOverlay))
            {
                MiniMode.Visibility = Visibility.Visible;
            }
            //replaces the Title Bar with a custom version. Taken from:
            //https://www.eternalcoding.com/?p=1952
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            TitleBar.Height = coreTitleBar.Height;
            Window.Current.SetTitleBar(MainTitleBar);

            Window.Current.Activated += Current_Activated;
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            if (Website == null)
            {
                Website = WebsiteViewModel.FromWebsite(WebsiteDataSource.GetDefault());
            }
        }

    public event PropertyChangedEventHandler PropertyChanged;

        //Checks for the Navigation Parameter and changes the WebView.Source to the requested Website.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            long iD;

            if (e.Parameter.ToString().StartsWith("ID:"))
            {
                try
                {
                    iD = Convert.ToInt64(e.Parameter.ToString().Remove(0,3));
                    Website = WebsiteViewModel.FromWebsite(WebsiteDataSource.GetWebsite(iD));
                }
                catch
                {
                    Website = WebsiteViewModel.FromWebsite(WebsiteDataSource.GetDefault());
                }
            }
            else if(false)
            {
                Website = WebsiteViewModel.FromWebsite(WebsiteDataSource.GetTempSite());
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
            IsActive(e);
        }

        private void IsActive(WindowActivatedEventArgs e)
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
                if (IsCompactview())
                {
                    TitleBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar titleBar, object args)
        {
            if (!IsCompactview())
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
            if (!IsCompactview())
            {
                ViewModePreferences compactOptions = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
                compactOptions.CustomSize = new Windows.Foundation.Size(500, 281.25);
                bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, compactOptions);
            }
            else
            {
                //Should not be reached.
            }
            ModeChanged();
        }

        private void Fullscreen_Click(object sender, RoutedEventArgs e)
        {
            var view = ApplicationView.GetForCurrentView();
            if (IsFullscreen())
            {
                view.ExitFullScreenMode();
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
                // The SizeChanged event will be raised when the exit from full-screen mode is complete.
            }
            else
            {
                if (view.TryEnterFullScreenMode())
                {
                    ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
                    // The SizeChanged event will be raised when the entry to full-screen mode is complete.
                }
            }
            ModeChanged();
        }

        private async void Normal_Click(object sender, RoutedEventArgs e)
        {
            var view = ApplicationView.GetForCurrentView();
            if (!IsNormalscreen())
            {
                bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);
            }
            else
            {
                //Should not be reached.
            }
            ModeChanged();
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
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

        //This method ensures that only the options for the not current mode are available in the UI.
        private void ModeChanged()
        {
            //FocuseMode
            if (!IsFullscreen())
            {
                FullscreenMode.Visibility = Visibility.Visible;
            }
            else
            {
                FullscreenMode.Visibility = Visibility.Collapsed;
            }
            //MiniMode
            if (!IsCompactview())
            {
                MiniMode.Visibility = Visibility.Visible;
                UrlTextBox.Visibility = Visibility.Visible;
                Go.Visibility = Visibility.Visible;
                SpaceForHamburger.Width = new GridLength(0, GridUnitType.Star);
            }
            else
            {
                MiniMode.Visibility = Visibility.Collapsed;
                UrlTextBox.Visibility = Visibility.Collapsed;
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
            string newUri = UrlTextBox.Text.ToString();
            NewUri(newUri);
        }

        public void NewUri(string newUri)
        {
            bool validUri = true;

            try
            {
                uri = new Uri(newUri);
            }
            catch
            {
                // Create the message dialog and set its content
                DisplayDialog("Error", "The URL entered was not a valid URL. A URL has to look like http://www.bing.com ");
                validUri = false;
                uri = webView.Source;
            }

            if (validUri)
            {
                webView.Source = uri;
                WebsiteDataSource.SetTempSite(uri.Host.ToString(), uri);
            }
        }

        public static void Test(Uri fu)
        {
            var test = WebViewPage;
            if (Page is test)
            ((test)Page).SendSessionVariables();
            webView.Source = fu;
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            SourceUpdated();
        }

        private async void PastAndGo_ClickAsync(object sender, RoutedEventArgs e)
        {
            var dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                try
                {
                    var text = await dataPackageView.GetTextAsync();
                    UrlTextBox.Text = text.ToString();
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

        private void UrlTextBox_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                SourceUpdated();
            }
        }

        private async void DisplayDialog(string title, string content)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "Ok"
            };

            ContentDialogResult result = await dialog.ShowAsync();
        }

        private void webView1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ModeChanged();
        }

    }
}
