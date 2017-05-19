using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the RadPictureRotatorHubTile class.
    /// </summary>
    public class RadPictureRotatorHubTileAutomationPeer : HubTileBaseAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadPictureRotatorHubTileAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
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
