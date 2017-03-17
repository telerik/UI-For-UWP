using System.Linq;
using Telerik.UI.Xaml.Controls.Primitives;
using Telerik.UI.Xaml.Controls.Primitives.Menu;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    public class RadialMenuItemControlAutomationPeer : RadControlAutomationPeer, IToggleProvider, IInvokeProvider
    {
        private RadRadialMenu parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialMenuItemControlAutomationPeer"/> class.
        /// Automation Peer for the <see cref="Telerik.UI.Xaml.Controls.Primitives.Menu.RadialMenuItemControl"/> class.
        /// </summary>
        /// <param name="owner">The object that is associated with this AutomationPeer.</param>
        public RadialMenuItemControlAutomationPeer(RadialMenuItemControl owner, RadRadialMenu parent) 
            : base(owner)
        {
            this.parent = parent;
        }
        
        private RadialMenuItemControl OwnerAsRadialMenuItemControl
        {
            get
            {
                return this.Owner as RadialMenuItemControl;
            }
        }

        /// <summary>
        /// IToggleProvider implementation.
        /// </summary>
        public ToggleState ToggleState
        {
            get
            {
                if (this.OwnerAsRadialMenuItemControl == null && this.OwnerAsRadialMenuItemControl.Segment != null)
                {
                    return ToggleState.Indeterminate;
                }

                return this.OwnerAsRadialMenuItemControl.Segment.TargetItem.IsSelected
                    ? ToggleState.On
                    : ToggleState.Off;
            }
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Invoke || patternInterface == PatternInterface.Toggle)
            {
                return this;
            }
            return base.GetPatternCore(patternInterface);
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.MenuItem;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(RadialMenuItemControl);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(RadialMenuItemControl);
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "radial menu item control";
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            string nameCore = base.GetNameCore();
            if (string.IsNullOrEmpty(nameCore) && this.OwnerAsRadialMenuItemControl.Header != null
                && this.OwnerAsRadialMenuItemControl.Header is string)
            {
                nameCore = this.OwnerAsRadialMenuItemControl.Header.ToString();
            }

            return nameCore;
        }

        /// <inheritdoc />
        protected override string GetAutomationIdCore()
        {
            var automationId = base.GetAutomationIdCore();
            if (!string.IsNullOrEmpty(automationId))
            {
                return automationId;
            }

            return nameof(RadialMenuItemControl);
        }

        /// <summary>
        /// IInvokeProvider implementation.
        /// </summary>
        public void Invoke()
        {
            if (this.OwnerAsRadialMenuItemControl != null && this.OwnerAsRadialMenuItemControl.Segment != null
                && this.OwnerAsRadialMenuItemControl.Segment.TargetItem.CanNavigate && this.parent != null)
            {
                this.parent.RaiseNavigateCommand(this.OwnerAsRadialMenuItemControl.Segment.TargetItem, this.parent.model.viewState.MenuLevels.FirstOrDefault(),
                    this.OwnerAsRadialMenuItemControl.Segment.LayoutSlot.StartAngle);
            }
        }

        /// <summary>
        /// IToggleProvider implementation.
        /// </summary>
        public void Toggle()
        {
            if (this.OwnerAsRadialMenuItemControl != null && this.OwnerAsRadialMenuItemControl.Segment != null
                && this.OwnerAsRadialMenuItemControl.Segment.TargetItem.Selectable)
            {
                this.OwnerAsRadialMenuItemControl.Segment.TargetItem.IsSelected = !this.OwnerAsRadialMenuItemControl.Segment.TargetItem.IsSelected;
            }
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
