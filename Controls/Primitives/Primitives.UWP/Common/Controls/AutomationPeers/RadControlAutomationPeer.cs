using Telerik.UI.Xaml.Controls;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the RadControl class.
    /// </summary>
    public class RadControlAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadControlAutomationPeer class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public RadControlAutomationPeer(RadControl owner) 
            : base(owner)
        {
        }
        
        private RadControl Control
        {
            get
            {
                return this.Owner as RadControl;
            }
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();
            if (!string.IsNullOrEmpty(nameCore))
            {
                return nameCore;
            }

            return string.Empty;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad control";
        }
    }
}
