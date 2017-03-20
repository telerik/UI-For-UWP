using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives.HubTile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines a hub tile with custom front and back contents and a swivel transition between them.
    /// </summary>
    [TemplatePart(Name = "PART_LayoutRoot", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_FlipControl", Type = typeof(FlipControl))]
    public class RadCustomHubTile : HubTileBase
    {
        /// <summary>
        /// Identifies the FrontContent dependency property.
        /// </summary>
        public static readonly DependencyProperty FrontContentProperty =
            DependencyProperty.Register(nameof(FrontContent), typeof(object), typeof(RadCustomHubTile), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the FrontContentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty FrontContentTemplateProperty =
            DependencyProperty.Register(nameof(FrontContentTemplate), typeof(DataTemplate), typeof(RadCustomHubTile), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the RadCustomHubTile class.
        /// </summary>
        public RadCustomHubTile()
        {
            this.DefaultStyleKey = typeof(RadCustomHubTile);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadCustomHubTileAutomationPeer(this);
        }

        /// <summary>
        /// Gets or sets the front content of the custom hub tile.
        /// </summary>
        public object FrontContent
        {
            get
            {
                return this.GetValue(RadCustomHubTile.FrontContentProperty);
            }

            set
            {
                this.SetValue(RadCustomHubTile.FrontContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the front content template of the custom hub tile.
        /// </summary>
        public object FrontContentTemplate
        {
            get
            {
                return this.GetValue(RadCustomHubTile.FrontContentTemplateProperty);
            }

            set
            {
                this.SetValue(RadCustomHubTile.FrontContentTemplateProperty, value);
            }
        }
    }
}
