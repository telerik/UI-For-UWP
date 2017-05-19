using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the RadCycleHubTile class.
    /// </summary>
    public class RadCycleHubTileAutomationPeer : HubTileBaseAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadCycleHubTileAutomationPeer class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public RadCycleHubTileAutomationPeer(RadCycleHubTile owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return "RadCycleHubTile";
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad cycle hub tile";
        }
    }
}
