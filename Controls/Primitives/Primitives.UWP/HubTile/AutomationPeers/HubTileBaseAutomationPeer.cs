using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the HubTileBase class.
    /// </summary>
    public class HubTileBaseAutomationPeer : RadControlAutomationPeer, IInvokeProvider, IToggleProvider
    {
        /// <summary>
        /// Initializes a new instance of the HubTileBaseAutomationPeer class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public HubTileBaseAutomationPeer(HubTileBase owner) 
            : base(owner)
        {
        }

        /// <inheritdoc/>
        public ToggleState ToggleState
        {
            get
            {
                return this.HubTileBase.IsFrozen ? ToggleState.Off : ToggleState.On;
            }
        }

        private HubTileBase HubTileBase
        {
            get
            {
                return (HubTileBase)this.Owner;
            }
        }

        /// <inheritdoc/>
        public void Invoke()
        {
            this.HubTileBase.ExecuteCommand();
        }

        /// <inheritdoc/>
        public void Toggle()
        {
            this.HubTileBase.IsFrozen = !this.HubTileBase.IsFrozen;
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseToggleStatePropertyChangedEvent(bool oldValue, bool newValue)
        {
            this.RaisePropertyChangedEvent(
                TogglePatternIdentifiers.ToggleStateProperty,
                oldValue ? ToggleState.On : ToggleState.Off,
                newValue ? ToggleState.On : ToggleState.Off);
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return "HubTileBase";
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "hub tile base";
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
            {
                return nameCore;
            }

            var title = this.HubTileBase.Title as string;
            if (!string.IsNullOrEmpty(title))
            {
                return title;
            }

            return string.Empty;
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Invoke || patternInterface == PatternInterface.Toggle)
            {
                return this;
            }

            return null;
        }

        /// <summary>
        /// Gets the control type for the HubTileBase that is associated with this HubTileBaseAutomationPeer.
        /// </summary>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        /// <inheritdoc />
        protected override bool HasKeyboardFocusCore()
        {
            return true;
        }
    }
}
