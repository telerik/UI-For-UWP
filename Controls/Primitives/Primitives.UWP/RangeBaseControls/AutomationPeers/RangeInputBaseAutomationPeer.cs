using System;
using System.Collections.Generic;
using System.Text;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the RangeInputBase class.
    /// </summary>
    public class RangeInputBaseAutomationPeer : RangeControlBaseAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RangeInputBaseAutomationPeer class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public RangeInputBaseAutomationPeer(RangeInputBase owner)
            : base(owner)
        {
        }

        internal RangeInputBase RangeInputBase
        {
            get
            {
                return (RangeInputBase)this.Owner;
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
            return "range input base";
        }
    }
}
