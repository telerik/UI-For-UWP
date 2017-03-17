using System;
using System.Collections.Generic;
using System.Text;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Automation.Peers
{
    public class RadHexHubTileAutomationPeer : HubTileBaseAutomationPeer
    {
        public RadHexHubTileAutomationPeer(RadHexHubTile owner) 
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
            return "RadHexHubTile";
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad hex hub tile";
        }
    }
}
