using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Automation.Peers
{
    public class RadPictureRotatorHubTileAutomationPeer : HubTileBaseAutomationPeer
    {
        public RadPictureRotatorHubTileAutomationPeer(RadPictureRotatorHubTile owner) 
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
            return "RadPictureRotatorHubTile";
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad picture rotator hub tile";
        }
    }
}
