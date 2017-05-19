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

        private RadBusyIndicator BusyIndicatorOwner
        {
            get
            {
                return (RadBusyIndicator)this.Owner;
            }
        }

        /// <summary>
        /// Cycles through the toggle states of a control.
        /// </summary>
        public void Toggle()
        {
            this.BusyIndicatorOwner.IsActive = !this.BusyIndicatorOwner.IsActive;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseToggleStatePropertyChangedEvent(bool oldState, bool newState)
        {
            ToggleState oldToggle = oldState ? ToggleState.On : ToggleState.Off;
            ToggleState newToggle = newState ? ToggleState.On : ToggleState.Off;

            this.RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, oldState, newToggle);
            this.RaisePropertyChangedEvent(AutomationElementIdentifiers.ItemStatusProperty, oldToggle.ToString(), newToggle.ToString());
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
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad busy indicator";
        }

        /// <inheritdoc />
        protected override string GetItemStatusCore()
        {
            if (this.ToggleState == ToggleState.On)
            {
                return nameof(ToggleState.On);
            }
            else if (this.ToggleState == ToggleState.Off)
            {
                return nameof(ToggleState.Off);
            }

            return base.GetItemStatusCore();
        }
    }
}
