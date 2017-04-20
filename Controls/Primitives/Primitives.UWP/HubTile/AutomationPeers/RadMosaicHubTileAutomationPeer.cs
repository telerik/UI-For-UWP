using System;
using System.Collections.Generic;
using System.Text;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// Automation Peer for the RadMosaicHubTile class.
    /// </summary>
    public class RadMosaicHubTileAutomationPeer : HubTileBaseAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadMosaicHubTileAutomationPeer" /> class.
        /// </summary>
        public RadMosaicHubTileAutomationPeer(RadMosaicHubTile owner) 
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
            return "RadMosaicHubTile";
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad mosaic hub tile";
        }
    }
}
