using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadBusyIndicator"/>.
    /// </summary>
    public class RadBusyIndicatorAutomationPeer : RadContentControlAutomationPeer, IToggleProvider
    {
        private RadBusyIndicator BusyIndicatorOwner
        {
            get
            {
                return (RadBusyIndicator)this.Owner;
            }
        }

        /// <summary>
        /// Initializes a new instance of the RadBusyIndicatorAutomationPeer class.
        /// </summary>
        /// <param name="owner">The RadBusyIndicator that is associated with this RadBusyIndicatorAutomationPeer.</param>
        public RadBusyIndicatorAutomationPeer(RadBusyIndicator owner) : base(owner)
        {

        }

        /// <summary>
        ///  Gets the toggle state of the control. 
        /// </summary>
        public ToggleState ToggleState
        {
            get
            {
                return this.BusyIndicatorOwner.IsActive ? ToggleState.On : ToggleState.Off;
            }
        }

        /// <summary>
        /// Cycles through the toggle states of a control.
        /// </summary>
        public void Toggle()
        {
            ToggleState oldValue = this.ToggleState;
            this.BusyIndicatorOwner.IsActive = !this.BusyIndicatorOwner.IsActive;
            ToggleState newValue = this.ToggleState;

            this.RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, oldValue, newValue);
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

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad busy indicator";
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseToggleStatePropertyChangedEvent(bool oldState, bool newState)
        {
            ToggleState oldToggle = oldState ? ToggleState.On : ToggleState.Off;
            ToggleState newToggle = newState ? ToggleState.On : ToggleState.Off;

            this.RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, oldState, newToggle);
        }
    }
}
