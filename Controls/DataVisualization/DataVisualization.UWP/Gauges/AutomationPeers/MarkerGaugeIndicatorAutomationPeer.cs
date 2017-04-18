using Telerik.UI.Xaml.Controls.DataVisualization;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    public class MarkerGaugeIndicatorAutomationPeer : GaugeIndicatorAutomationPeer, IToggleProvider
    {
        public MarkerGaugeIndicatorAutomationPeer(MarkerGaugeIndicator owner) 
            : base(owner)
        {
        }

        private MarkerGaugeIndicator OwnerMarkerGaugeIndicator
        { 
            get
            {
                return this.Owner as MarkerGaugeIndicator;
            }
        }

        /// <summary>
        /// IToggleProvider implementation.
        /// </summary>
        public ToggleState ToggleState
        {
            get
            {
                return this.OwnerMarkerGaugeIndicator.IsRotated ? ToggleState.On : ToggleState.Off;
            }
        }

        /// <summary>
        /// IToggleProvider implementation.
        /// </summary>
        public void Toggle()
        {
            this.OwnerMarkerGaugeIndicator.IsRotated = !this.OwnerMarkerGaugeIndicator.IsRotated;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(MarkerGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(MarkerGaugeIndicator);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "marker gauge indicator";
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Toggle)
            {
                return this;
            }
            return base.GetPatternCore(patternInterface);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseToggleStatePropertyChangedEvent(bool oldValue, bool newValue)
        {
            this.RaisePropertyChangedEvent(
                TogglePatternIdentifiers.ToggleStateProperty,
                oldValue ? ToggleState.On : ToggleState.Off,
                newValue ? ToggleState.On : ToggleState.Off);
        }
    }
}
