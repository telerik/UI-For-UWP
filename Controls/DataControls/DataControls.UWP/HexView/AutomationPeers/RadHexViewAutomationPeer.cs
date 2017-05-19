using Telerik.UI.Xaml.Controls.Data;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadHexView"/>.
    /// </summary>
    public class RadHexViewAutomationPeer : RadControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadHexViewAutomationPeer class.
        /// </summary>
        /// <param name="owner">The RadHexView that is associated with this RadHexViewAutomationPeer.</param>
        public RadHexViewAutomationPeer(RadHexView owner)
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.List;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad hex view";
        }
    }
}
