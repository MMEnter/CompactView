using System;
using System.Threading.Tasks;

using CompactView.Helpers;
using CompactView.Views;

using Windows.ApplicationModel;

namespace CompactView.Services
{
    public class FirstRunDisplayService
    {
        internal static async Task ShowIfAppropriate()
        {
            bool hasShownFirstRun = false;
            hasShownFirstRun = await Windows.Storage.ApplicationData.Current.LocalSettings.ReadAsync<bool>(nameof(hasShownFirstRun));

            if (!hasShownFirstRun)
            {
                await Windows.Storage.ApplicationData.Current.LocalSettings.SaveAsync(nameof(hasShownFirstRun), true);
                var dialog = new FirstRunDialog();
                await dialog.ShowAsync();
            }
        }
    }
}
