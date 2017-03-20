using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadSideDrawer"/>.
    /// </summary>
    public class RadSideDrawerAutomationPeer : RadControlAutomationPeer, IToggleProvider
    {
        private RadSideDrawer SideDrawerOwner
        {
            get
            {
                return (RadSideDrawer)this.Owner;
            }
        }

        /// <summary>
        /// Initializes a new instance of the RadSideDrawerAutomationPeer class.
        /// </summary>
        /// <param name="owner">The RadSideDrawer that is associated with this RadSideDrawerAutomationPeer.</param>
        public RadSideDrawerAutomationPeer(RadSideDrawer owner) : base(owner)
        {

        }

        /// <summary>
        ///  Gets the toggle state of the control. 
        /// </summary>
        public ToggleState ToggleState
        {
            get
            {
                return this.ConvertDrawerToToggleState(this.SideDrawerOwner.DrawerState);
            }
        }

        /// <summary>
        /// Cycles through the toggle states of a control.
        /// </summary>
        public void Toggle()
        {
            DrawerState state = this.SideDrawerOwner.DrawerState;
            if (state == DrawerState.Opened)
            {
                this.SideDrawerOwner.HideDrawer();
            }
            else if (state == DrawerState.Closed)
            {
                this.SideDrawerOwner.ShowDrawer();
            }
            else
            {
                if (this.SideDrawerOwner.IsOpen)
                {
                    this.SideDrawerOwner.HideDrawer();
                }
                else
                {
                    this.SideDrawerOwner.ShowDrawer();
                }
            }
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
            return "rad side drawer";
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseToggleStatePropertyChangedEvent(DrawerState oldState, DrawerState newState)
        {
            this.RaisePropertyChangedEvent(
                TogglePatternIdentifiers.ToggleStateProperty,
                this.ConvertDrawerToToggleState(oldState),
                this.ConvertDrawerToToggleState(newState));
        }

        private ToggleState ConvertDrawerToToggleState(DrawerState drawerState)
        {
            if (drawerState == DrawerState.Opened)
            {
                return ToggleState.On;
            }
            else if (drawerState == DrawerState.Closed)
            {
                return ToggleState.Off;
            }
            else
            {
                return ToggleState.Indeterminate;
            }
        }
    }
}
