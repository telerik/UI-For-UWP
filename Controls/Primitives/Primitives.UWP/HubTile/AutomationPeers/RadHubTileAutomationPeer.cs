using System;
using System.Collections.Generic;
using System.Text;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    public class RadHubTileAutomationPeer : HubTileBaseAutomationPeer
    {
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
                return name;

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
