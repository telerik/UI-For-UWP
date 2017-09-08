using Telerik.UI.Xaml.Controls.Primitives.Menu;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the NavigationItemButton class.
    /// </summary>
    public class NavigationItemButtonAutomationPeer : RadControlAutomationPeer, IInvokeProvider
    {
        /// <summary>
        /// Initializes a new instance of the NavigationItemButtonAutomationPeer class.
        /// </summary>
        /// <param name="owner">The NavigationItemButton that is associated with this NavigationItemButtonAutomationPeer.</param>
        public NavigationItemButtonAutomationPeer(NavigationItemButton owner) 
            : base(owner)
        {
        }

        private NavigationItemButton NavigationItemButton
        {
            get
            {
                return this.Owner as NavigationItemButton;
            }
        }

        /// <inheritdoc/>
        public void Invoke()
        {
            if (this.NavigationItemButton != null)
            {
                this.NavigationItemButton.ExecuteNavigation();
            }
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
            {
                return nameCore;
            }

            return nameof(NavigationItemButton);
        }

        /// <summary>
        /// Gets the control type for the NavigationItemButton that is associated with this NavigationItemButtonAutomationPeer.
        /// </summary>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Button;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }
        
        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "navigation item button";
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Invoke)
            {
                return this;
            }

            return null;
        }
    }
}
