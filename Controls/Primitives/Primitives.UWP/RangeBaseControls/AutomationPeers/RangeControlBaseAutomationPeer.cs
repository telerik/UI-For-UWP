using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the RangeControlBase class.
    /// </summary>
    public class RangeControlBaseAutomationPeer : RadHeaderedControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeControlBaseAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public RangeControlBaseAutomationPeer(RangeControlBase owner) 
            : base(owner)
        {
        }

        private RangeControlBase RangeControlBase
        {
            get
            {
                return (RangeControlBase)this.Owner;
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
            return "range control base";
        }
    }
}
