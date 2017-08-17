using System;
using System.Threading.Tasks;

using CompactView.Services;

using Windows.ApplicationModel.Activation;

namespace CompactView.Activation
{
    internal class ProtocolActivationHandler : ActivationHandler<ProtocolActivatedEventArgs>
    {
        private readonly Type _navElement;
        private readonly long _iD;

        public ProtocolActivationHandler(Type navElement, long iD)
        {
            _navElement = navElement;
            _iD = iD;
        }
    
        protected override async Task HandleInternalAsync(ProtocolActivatedEventArgs args)
        {
            // When the navigation stack isn't restored navigate to the first page,
            // configuring the new page by passing required information as a navigation
            // parameter
            NavigationService.Navigate(_navElement, _iD.ToString());

            await Task.CompletedTask;
        }

        protected override bool CanHandleInternal(ProtocolActivatedEventArgs args)
        {
            // Always accept new share intents
            return true;
        }
    }
}
