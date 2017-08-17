using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using CompactView.Helpers;
using CompactView.Services;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using static CompactView.Views.WebViewPage;
using CompactView.Helpers;
using System.Collections.Generic;
using System;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.StartScreen;
using System.Threading.Tasks;
using CompactView.Data;

namespace CompactView.Views
{
    public sealed partial class ShellPage : Page, INotifyPropertyChanged
    {
        private const string PanoramicStateName = "PanoramicState";
        private const string WideStateName = "WideState";
        private const string NarrowStateName = "NarrowState";

        private bool _isPaneOpen;
        public bool IsPaneOpen
        {
            get { return _isPaneOpen; }
            set { Set(ref _isPaneOpen, value); }
        }

        private SplitViewDisplayMode _displayMode = SplitViewDisplayMode.CompactInline;
        public SplitViewDisplayMode DisplayMode
        {
            get { return _displayMode; }
            set { Set(ref _displayMode, value); }
        }

        private object _lastSelectedItem;

        private ObservableCollection<ShellNavigationItem> _primaryItems = new ObservableCollection<ShellNavigationItem>();
        public ObservableCollection<ShellNavigationItem> PrimaryItems
        {
            get { return _primaryItems; }
            set { Set(ref _primaryItems, value); }
        }

        private ObservableCollection<ShellNavigationItem> _secondaryItems = new ObservableCollection<ShellNavigationItem>();
        public ObservableCollection<ShellNavigationItem> SecondaryItems
        {
            get { return _secondaryItems; }
            set { Set(ref _secondaryItems, value); }
        }

        public ShellPage()
        {
            InitializeComponent();
            DataContext = this;
            Initialize();
        }

        private void Initialize()
        {
            NavigationService.Frame = shellFrame;
            NavigationService.Frame.Navigated += NavigationService_Navigated;
            PopulateNavItems();
        }

        private void PopulateNavItems()
        {
            List<Website> websites = WebsiteDataSource.GetList();

            _primaryItems.Clear();
            _secondaryItems.Clear();

            foreach (Website site in websites)
            {
                _primaryItems.Add(ShellNavigationItem.FromType<WebViewPage>(site.Name, site.Symbol, site.ID));
            }

            _secondaryItems.Add(ShellNavigationItem.FromType<SettingsPage>("Shell_Settings".GetLocalized(), Symbol.Setting));
        }

        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            var item = PrimaryItems?.FirstOrDefault(i => i.ID.ToString() == e?.Parameter.ToString());
            if (item == null)
            {
                item = SecondaryItems?.FirstOrDefault(i => i.PageType == e?.SourcePageType);
            }

            if (item != null)
            {
                ChangeSelected(_lastSelectedItem, item);
                _lastSelectedItem = item;
            }
        }

        private void ChangeSelected(object oldValue, object newValue)
        {
            if (oldValue != null)
            {
                (oldValue as ShellNavigationItem).IsSelected = false;
            }
            if (newValue != null)
            {
                (newValue as ShellNavigationItem).IsSelected = true;
            }
        }

        private void Navigate(object item)
        {
            var navigationItem = item as ShellNavigationItem;
            if (navigationItem != null)
            {
                NavigationService.Navigate(navigationItem.PageType, "ID:" + navigationItem.ID);
            }
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            var navigationButton = sender as Button;
            if (DisplayMode == SplitViewDisplayMode.CompactOverlay || DisplayMode == SplitViewDisplayMode.Overlay)
            {
                IsPaneOpen = false;
            }
            Navigate(navigationButton.DataContext);
        }

        private void SlidableListItem_Click(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var navigationButton = sender as SlidableListItem;
            if (DisplayMode == SplitViewDisplayMode.CompactOverlay || DisplayMode == SplitViewDisplayMode.Overlay)
            {
                IsPaneOpen = false;
            }
            Navigate(navigationButton.DataContext);
        }

        private void OpenPane_Click(object sender, RoutedEventArgs e)
        {
            IsPaneOpen = !_isPaneOpen;
        }

        private void WindowStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            switch (e.NewState.Name)
            {
                case PanoramicStateName:
                    DisplayMode = SplitViewDisplayMode.CompactInline;
                    break;
                case WideStateName:
                    DisplayMode = SplitViewDisplayMode.CompactInline;
                    IsPaneOpen = false;
                    break;
                case NarrowStateName:
                    DisplayMode = SplitViewDisplayMode.Overlay;
                    IsPaneOpen = false;
                    break;
                default:
                    break;
            }            
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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Website currentWebsite = WebsiteDataSource.GetTempSite();
            Uri uri = currentWebsite.URL;
            string Name = currentWebsite.Name;

            WebsiteDataSource.AddNewAsync(Name, uri);
            PopulateNavItems();
        }

        private void RightSlide_Click(object sender, EventArgs e)
        {
            long iD = Convert.ToInt64((sender as SlidableListItem).Tag.ToString());


            WebsiteDataSource.Delete(iD);
            PopulateNavItems();
        }

        private void SlidableListItem_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            long iD = Convert.ToInt64((sender as SlidableListItem).Tag.ToString());
            ShowDialog(iD);
        }

        private async Task ShowDialog(long iD)
        {
            var dialog = new EditDialog();
            dialog.AccessKey = iD.ToString();
            await dialog.ShowAsync();

            if (dialog.Result == EditResult.RenameOK)
            {
                // Rename was successful.
                PopulateNavItems();
            }
            else if (dialog.Result == EditResult.RenameCancel)
            {
                // Rename was cancelled by the user.
            }
        }

        private async void LeftSlide_Click(object sender, EventArgs e)
        {
            long iD = Convert.ToInt64((sender as SlidableListItem).Tag.ToString());
            ShowDialog(iD);
        }

        private void Pin_Click(object sender, RoutedEventArgs e)
        {
            long iD = Convert.ToInt64((sender as Button).Tag.ToString());
            PinTileAsync(iD);
        }

        private async Task PinTileAsync(long iD)
        {
            string tileId = "website" + iD;
            string displayName = WebsiteDataSource.GetWebsite(iD).Name;
            string arguments = iD.ToString();

            // Initialize the tile with required arguments
            SecondaryTile tile = new SecondaryTile(
                tileId,
                displayName,
                arguments,
                new Uri("ms-appx:///Assets/Square150x150Logo.png"),
                TileSize.Default);

            // Pin the tile
            bool isPinned = await tile.RequestCreateAsync();

            // TODO: Update UI to reflect whether user can now either unpin or pin
        }

        private void Unpin_Click(object sender, RoutedEventArgs e)
        {
            long iD = Convert.ToInt64((sender as Button).Tag.ToString());
            UnpinTile(iD);
        }

        private async Task UnpinTile(long iD)
        {
            string tileId = "website" + iD;

            // Initialize a secondary tile with the same tile ID you want removed
            SecondaryTile toBeDeleted = new SecondaryTile(tileId);

            // And then unpin the tile
            await toBeDeleted.RequestDeleteAsync();
        }
    }

}
