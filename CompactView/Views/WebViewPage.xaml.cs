using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;

namespace CompactView.Views
{
    public sealed partial class WebViewPage : Page, INotifyPropertyChanged
    {
        // TODO UWPTemplates: Set your hyperlink default here
        private const string defaultUrl = "https://developer.microsoft.com/en-us/windows/apps";

        private Uri _source;
        public Uri Source
        {
            get { return _source; }
            set { Set(ref _source, value); }
        }

        public WebViewPage()
        {
            Source = new Uri(defaultUrl);
            InitializeComponent();
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
    }
}
