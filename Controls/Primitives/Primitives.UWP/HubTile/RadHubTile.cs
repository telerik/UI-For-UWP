using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives.HubTile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines a standard tile with a picture, title, message and notification count. Similar to Mail, Messaging or Internet Explorer.
    /// </summary>
    [TemplatePart(Name = "PART_LayoutRoot", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_FlipControl", Type = typeof(FlipControl))]
    [TemplatePart(Name = "PART_Notification", Type = typeof(ContentControl))]
    public class RadHubTile : HubTileBase
    {
        /// <summary>
        /// Identifies the ImageSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(RadHubTile), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Message dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(object), typeof(RadHubTile), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the MessageTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageTemplateProperty =
            DependencyProperty.Register(nameof(MessageTemplate), typeof(DataTemplate), typeof(RadHubTile), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the Notification dependency property.
        /// </summary>
        public static readonly DependencyProperty NotificationProperty =
            DependencyProperty.Register(nameof(Notification), typeof(object), typeof(RadHubTile), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the NotificationTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty NotificationTemplateProperty =
            DependencyProperty.Register(nameof(NotificationTemplate), typeof(DataTemplate), typeof(RadHubTile), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the RadHubTile class.
        /// </summary>
        public RadHubTile()
        {
            this.DefaultStyleKey = typeof(RadHubTile);
        }

        /// <summary>
        /// Gets or sets the source of the tile image.
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                return (ImageSource)this.GetValue(ImageSourceProperty);
            }

            set
            {
                this.SetValue(ImageSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the message of the tile. That is the content displayed in the top-left corner of the control.
        /// </summary>
        public object Message
        {
            get
            {
                return this.GetValue(MessageProperty);
            }

            set
            {
                this.SetValue(MessageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets <see cref="DataTemplate"/> instance that defines the appearance of the displayed <see cref="Message"/>.
        /// </summary>
        public DataTemplate MessageTemplate
        {
            get
            {
                return this.GetValue(MessageTemplateProperty) as DataTemplate;
            }
            set
            {
                this.SetValue(MessageTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the additional information (notification) displayed on the tile.
        /// Typically this may be the Message Count and it is displayed in the bottom-right corner of the control.
        /// </summary>
        public object Notification
        {
            get
            {
                return this.GetValue(NotificationProperty);
            }
            set
            {
                this.SetValue(NotificationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets <see cref="DataTemplate"/> instance that defines the appearance of the displayed <see cref="Notification"/>.
        /// </summary>
        public DataTemplate NotificationTemplate
        {
            get
            {
                return this.GetValue(NotificationTemplateProperty) as DataTemplate;
            }
            set
            {
                this.SetValue(NotificationTemplateProperty, value);
            }
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadHubTileAutomationPeer(this);
        }
    }
}
