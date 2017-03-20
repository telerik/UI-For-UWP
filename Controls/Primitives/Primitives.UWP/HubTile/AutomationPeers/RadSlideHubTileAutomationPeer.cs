using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Automation.Peers
{
    public class RadSlideHubTileAutomationPeer : HubTileBaseAutomationPeer
    {
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
