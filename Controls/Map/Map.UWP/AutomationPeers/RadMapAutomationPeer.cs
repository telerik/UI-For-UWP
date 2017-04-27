using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Map;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="RadMap"/>.
    /// </summary>
    public class RadMapAutomationPeer : RadControlAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadMapAutomationPeer"/> class.
        /// </summary>
        public RadMapAutomationPeer(RadMap owner) : base(owner)
        {
        }

        internal RadMap MapOwner
        {
            get
            {
                return (RadMap)this.Owner;
            }
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "rad map";
        }

        /// <inheritdoc />
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            List<AutomationPeer> peers = new List<AutomationPeer>();

            foreach (MapLayer layer in this.MapOwner.Layers)
            {
                if (layer is MapShapeLayer)
                {
                    MapShapeLayerAutomationPeer layerPeer = CreatePeerForElement(layer) as MapShapeLayerAutomationPeer;
                    if (layerPeer != null)
                    {
                        layerPeer.GetChildren();
                        peers.Add(layerPeer);
                    }
                }
            }
            return peers;
        }
    }
}
