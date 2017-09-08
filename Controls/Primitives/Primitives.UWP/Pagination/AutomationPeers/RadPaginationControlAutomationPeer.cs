using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the RadPaginationControl class.
    /// </summary>
    public class RadPaginationControlAutomationPeer : RadControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadPaginationControlAutomationPeer class.
        /// </summary>
        /// <param name="owner">The RadPaginationControl that is associated with this RadPaginationControlAutomationPeer.</param>
        public RadPaginationControlAutomationPeer(RadPaginationControl owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Primitives.RadPaginationControl);
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return nameof(Telerik.UI.Xaml.Controls.Primitives.RadPaginationControl);
        }

        /// <inheritdoc />
        protected override string GetAutomationIdCore()
        {
            var automationId = base.GetAutomationIdCore();
            if (!string.IsNullOrEmpty(automationId))
            {
                return automationId;
            }

            return nameof(Telerik.UI.Xaml.Controls.Primitives.RadPaginationControl);
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Group;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad pagination control";
        }
    }
}
