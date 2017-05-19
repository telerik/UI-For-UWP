using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the RadCustomHubTile class.
    /// </summary>
    public class RadCustomHubTileAutomationPeer : HubTileBaseAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadCustomHubTileAutomationPeer class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public RadCustomHubTileAutomationPeer(RadCustomHubTile owner) 
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
            return "RadCustomHubTile";
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad custom hub tile";
        }
    }
}
