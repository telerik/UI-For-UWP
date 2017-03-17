using System;
using System.Collections.Generic;
using System.Text;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Automation.Peers
{
    public class RadCycleHubTileAutomationPeer : HubTileBaseAutomationPeer
    {
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
