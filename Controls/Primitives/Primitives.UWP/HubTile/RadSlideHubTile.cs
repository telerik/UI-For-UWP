using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives.HubTile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines a tile that behaves like the WP OS tile that is created when a contact is pinned to the start screen.
    /// </summary>
    [TemplatePart(Name = "PART_LayoutRoot", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_FlipControl", Type = typeof(FlipControl))]
    public class RadSlideHubTile : HubTileBase
    {
        /// <summary>
        /// Identifies the <see cref="TopContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TopContentProperty =
            DependencyProperty.Register(nameof(TopContent), typeof(object), typeof(RadSlideHubTile), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TopContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TopContentTemplateProperty =
            DependencyProperty.Register(nameof(TopContentTemplate), typeof(DataTemplate), typeof(RadSlideHubTile), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="BottomContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomContentProperty =
            DependencyProperty.Register(nameof(BottomContent), typeof(object), typeof(RadSlideHubTile), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="BottomContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomContentTemplateProperty =
            DependencyProperty.Register(nameof(BottomContentTemplate), typeof(DataTemplate), typeof(RadSlideHubTile), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ExpandedState"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandedStateProperty =
            DependencyProperty.Register(nameof(ExpandedState), typeof(SlideTileExpandedState), typeof(RadSlideHubTile), new PropertyMetadata(SlideTileExpandedState.Normal));

        private SlideControl slideControl;
        private FlipControl flipControl;

        /// <summary>
        /// Initializes a new instance of the RadSlideHubTile class.
        /// </summary>
        public RadSlideHubTile()
        {
            this.DefaultStyleKey = typeof(RadSlideHubTile);
        }

        /// <summary>
        /// Gets or sets the expanded state of the control.
        /// </summary>
        public SlideTileExpandedState ExpandedState
        {
            get { return (SlideTileExpandedState)this.GetValue(ExpandedStateProperty); }
            set { this.SetValue(ExpandedStateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the front content of the tile.
        /// Typically this will be a picture but it may be any arbitrary content.
        /// </summary>
        public object TopContent
        {
            get
            {
                return this.GetValue(TopContentProperty);
            }
            set
            {
                this.SetValue(TopContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> that defines the appearance of the <see cref="TopContent"/> value.
        /// </summary>
        public DataTemplate TopContentTemplate
        {
            get
            {
                return this.GetValue(TopContentTemplateProperty) as DataTemplate;
            }
            set
            {
                this.SetValue(TopContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the front content of the tile.
        /// Typically this will be a picture but it may be any arbitrary content.
        /// </summary>
        public object BottomContent
        {
            get
            {
                return this.GetValue(BottomContentProperty);
            }
            set
            {
                this.SetValue(BottomContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> that defines the appearance of the <see cref="BottomContent"/> value.
        /// </summary>
        public DataTemplate BottomContentTemplate
        {
            get
            {
                return this.GetValue(BottomContentTemplateProperty) as DataTemplate;
            }
            set
            {
                this.SetValue(BottomContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets the HubTile SlideControl. Exposed for testing purposes.
        /// </summary>
        internal SlideControl SlideControl
        {
            get
            {
                return this.slideControl;
            }
        }

        /// <summary>
        /// Gets the HubTile <see cref="FlipControl"/>. Exposed for testing purposes.
        /// </summary>
        internal FlipControl FlipControl
        {
            get
            {
                return this.flipControl;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a rectangle clip is set on the LayoutRoot.
        /// </summary>
        protected override bool ShouldClip
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Determines whether the update timer used to update the tile's VisualState needs to be started.
        /// </summary>
        protected override bool IsUpdateTimerNeeded
        {
            get
            {
                if ((this.TopContent != null || this.TopContentTemplate != null) && (this.BottomContent != null || this.BottomContentTemplate != null))
                {
                    return true;
                }

                return base.IsUpdateTimerNeeded;
            }
        }

        /// <summary>
        /// Re-evaluates the current visual state for the control and updates it if necessary.
        /// </summary>
        /// <param name="animate">True to use transitions during state update, false otherwise.</param>
        protected internal override void UpdateVisualState(bool animate)
        {
            base.UpdateVisualState(animate);

            if (this.slideControl != null)
            {
                this.slideControl.UpdateVisualState(animate);
            }
        }

        /// <summary>
        /// Initializes the PART_Panel template part.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.slideControl = this.GetTemplatePartField<SlideControl>("PART_SlideControl");
            applied = applied && this.slideControl != null;

            this.flipControl = this.GetTemplatePartField<FlipControl>("PART_FlipControl");
            applied = applied && this.flipControl != null;

            return applied;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadSlideHubTileAutomationPeer(this);
        }
    }
}
