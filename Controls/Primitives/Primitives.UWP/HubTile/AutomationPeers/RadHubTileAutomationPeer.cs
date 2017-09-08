using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the RadHubTile class.
    /// </summary>
    public class RadHubTileAutomationPeer : HubTileBaseAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the RadHubTileAutomationPeer class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public RadHubTileAutomationPeer(RadHubTile owner) 
            : base(owner)
        {
        }
        
        private RadHubTile RadHubTile
        {
            get
            {
                return (RadHubTile)this.Owner;
            }
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string GetHelpTextCore()
        {
            return "RadHubTile";
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad hub tile";
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            string name = base.GetNameCore();
            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }

            name = this.RadHubTile.Message as string;
            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }

            name = this.RadHubTile.Notification as string;
            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }

            return "RadHubTile";
        }
    }
}
