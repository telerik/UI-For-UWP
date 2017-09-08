using System;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Primitives.HubTile
{
    /// <summary>
    /// Represents a control, used by <see cref="RadSlideHubTile"/> to display two different contents with a slide transition.
    /// </summary>
    [TemplateVisualState(Name = "Normal")]
    [TemplateVisualState(Name = "SemiExpanded")]
    [TemplateVisualState(Name = "Expanded")]
    public class SlideControl : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="TopContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TopContentProperty =
            DependencyProperty.Register(nameof(TopContent), typeof(object), typeof(SlideControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TopContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TopContentTemplateProperty =
            DependencyProperty.Register(nameof(TopContentTemplate), typeof(DataTemplate), typeof(SlideControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="BottomContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomContentProperty =
            DependencyProperty.Register(nameof(BottomContent), typeof(object), typeof(SlideControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="BottomContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomContentTemplateProperty =
            DependencyProperty.Register(nameof(BottomContentTemplate), typeof(DataTemplate), typeof(SlideControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ExpandedState"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandedStateProperty =
            DependencyProperty.Register(nameof(ExpandedState), typeof(SlideTileExpandedState), typeof(SlideControl), new PropertyMetadata(SlideTileExpandedState.Normal, OnExpandedStateChanged));

        private static readonly int ExpandedStatesCount = Enum.GetNames(typeof(SlideTileExpandedState)).Length;
        private bool updatingExpandedState;

        /// <summary>
        /// Initializes a new instance of the <see cref="SlideControl"/> class.
        /// </summary>
        public SlideControl()
        {
            this.DefaultStyleKey = typeof(SlideControl);
        }

        /// <summary>
        /// Gets or sets the front content of the tile. Typically this will be a picture but it may be any arbitrary content.
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
        /// Gets or sets the front content of the tile. Typically this will be a picture but it may be any arbitrary content.
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
        /// Gets or sets the expanded state of the control.
        /// </summary>
        public SlideTileExpandedState ExpandedState
        {
            get { return (SlideTileExpandedState)this.GetValue(ExpandedStateProperty); }
            set { this.SetValue(ExpandedStateProperty, value); }
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected override string ComposeVisualStateName()
        {
            if (!this.updatingExpandedState)
            {
                this.UpdateExpandedState();
            }

            return this.ExpandedState.ToString();
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new SlideControlAutomationPeer(this);
        }

        private static void OnExpandedStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slide = (SlideControl)d;

            if (!slide.IsInternalPropertyChange)
            {
                slide.updatingExpandedState = true;

                slide.UpdateVisualState(slide.IsLoaded);

                slide.updatingExpandedState = false;

                SlideControlAutomationPeer peer = SlideControlAutomationPeer.FromElement(slide) as SlideControlAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseExpandCollapseAutomationEvent((SlideTileExpandedState)e.OldValue, (SlideTileExpandedState)e.NewValue);
                }
            }
        }

        private void UpdateExpandedState()
        {
            // Do not increment if the control is initially loading.
            if (!string.IsNullOrEmpty(this.CurrentVisualState))
            {
                var nextState = (SlideTileExpandedState)(((int)this.ExpandedState + 1) % ExpandedStatesCount);

                this.ChangePropertyInternally(SlideControl.ExpandedStateProperty, nextState);
            }
        }
    }
}
