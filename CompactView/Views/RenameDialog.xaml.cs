using CompactView.Data;
using CompactView.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CompactView.Views
{
    public enum RenameResult
    {
        RenameOK,
        RenameCancel,
        Nothing
    }

    public class SymbolSelect
    {
        public Symbol Symbol { get; set; }

        public char SymbolAsChar { get { return (char)Symbol; } }
    }

    public sealed partial class RenameDialog : ContentDialog
    {
        ObservableCollection<SymbolSelect> symbols = new ObservableCollection<SymbolSelect>();

        public RenameResult Result { get; private set; }

        private static DependencyProperty s_websiteProperty
    = DependencyProperty.Register("Website", typeof(WebsiteViewModel), typeof(WebViewPage), new PropertyMetadata(null));

        public static DependencyProperty ItemProperty
        {
            get { return s_websiteProperty; }
        }

        public WebsiteViewModel Website
        {
            get { return (WebsiteViewModel)GetValue(s_websiteProperty); }
            set { SetValue(s_websiteProperty, value); }
        }

        public RenameDialog()
        {
            this.InitializeComponent();

            foreach (Symbol symbol in Enum.GetValues(typeof(Symbol)))
            {
                SymbolSelect newSymbolSelect = new SymbolSelect();
                newSymbolSelect.Symbol = symbol;
                symbols.Add(newSymbolSelect);
            }

        }

        //Checks for the Navigation Parameter and changes the ViewModel to the requested Website.
        private void ContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            Result = RenameResult.Nothing;
            long iD = Convert.ToInt64(sender.AccessKey);

            try
            {
               Website = WebsiteViewModel.FromWebsite(WebsiteDataSource.GetWebsite(iD));
            }
            catch
            {
                Website = WebsiteViewModel.FromWebsite(WebsiteDataSource.GetDefault());
            }
        }

        private void Name_TextBox_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                long iD = Website.ID;
                string name = Name_TextBox.Text.ToString();

                if (name == "")
                {
                    name = Website.Name;
                }

                WebsiteDataSource.Rename(iD, name);
                Result = RenameResult.RenameOK;
            }
        }

        private void OK_ButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            long iD = Website.ID;
            string name = Name_TextBox.Text.ToString();

            if (name == "")
            {
                name = Website.Name;
            }

            WebsiteDataSource.Rename(iD, name);
            Result = RenameResult.RenameOK;
        }

        private void URL_TextBox_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            bool validURL = false;
            Uri newUri = Website.URL;

            try
            {
                newUri = new Uri(URL_TextBox.Text.ToString());
                validURL = true;
            }
            catch
            {
                // Create the message dialog and set its content
                DisplayDialog("Error", "The URL entered was not a valid URL. A URL has to look like http://www.bing.com ");
            }

            if (validURL)
            {
                WebsiteDataSource.ChangeUrl(Website.ID, newUri);
                Result = RenameResult.RenameOK;
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

        private void SymbolListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var icon = (sender as ListView).SelectedValue;
            SymbolSelect newSymbol = (icon as SymbolSelect);
            WebsiteDataSource.ChangeSymbol(Website.ID, newSymbol.Symbol);
        }
    }
}
