using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the RadSlideHubTile class.
    /// </summary>
    public class RadSlideHubTileAutomationPeer : HubTileBaseAutomationPeer
    {
        /// <inheritdoc />
        public RadSlideHubTileAutomationPeer(RadSlideHubTile owner) 
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
            return "RadSlideHubTile";
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad slide hub tile";
        }
    }
}
