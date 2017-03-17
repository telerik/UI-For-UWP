using System;
using System.Collections.Generic;
using System.Text;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Automation.Peers
{
    public class RadMosaicHubTileAutomationPeer : HubTileBaseAutomationPeer
    {
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
