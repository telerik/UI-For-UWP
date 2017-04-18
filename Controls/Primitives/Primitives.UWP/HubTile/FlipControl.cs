using System;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Primitives.HubTile
{
    /// <summary>
    /// Represents a control that has a front and back content and can go to either content through a Flip transition (that is a PlaneProjection animation).
    /// </summary>
    [TemplateVisualState(Name = "NotFlipped")]
    [TemplateVisualState(Name = "Flipped")]
    public class FlipControl : RadControl
    {
        /// <summary>
        /// Identifies the FrontContent dependency property.
        /// </summary>
        public static readonly DependencyProperty FrontContentProperty =
            DependencyProperty.Register(nameof(FrontContent), typeof(object), typeof(FlipControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the FrontContent dependency property.
        /// </summary>
        public static readonly DependencyProperty BackContentProperty =
            DependencyProperty.Register(nameof(BackContent), typeof(object), typeof(FlipControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the FrontContentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty FrontContentTemplateProperty =
            DependencyProperty.Register(nameof(FrontContentTemplate), typeof(DataTemplate), typeof(FlipControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the BackContentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty BackContentTemplateProperty =
            DependencyProperty.Register(nameof(BackContentTemplate), typeof(DataTemplate), typeof(FlipControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the IsFlipped dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFlippedProperty =
            DependencyProperty.Register(nameof(IsFlipped), typeof(bool), typeof(FlipControl), new PropertyMetadata(false, OnIsFlippedChanged));

        private bool updatingIsFlipped;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlipControl" /> class.
        /// </summary>
        public FlipControl()
        {
            this.DefaultStyleKey = typeof(FlipControl);
        }

        /// <summary>
        /// Gets or sets the content on the front side of the flip tile.
        /// </summary>
        public object FrontContent
        {
            get
            {
                return this.GetValue(FrontContentProperty);
            }

            set
            {
                this.SetValue(FrontContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content on the back side of the flip tile.
        /// Test row.
        /// </summary>
        public object BackContent
        {
            get
            {
                return this.GetValue(BackContentProperty);
            }

            set
            {
                this.SetValue(BackContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the front content template.
        /// </summary>
        public DataTemplate FrontContentTemplate
        {
            get
            {
                return this.GetValue(FrontContentTemplateProperty) as DataTemplate;
            }

            set
            {
                this.SetValue(FrontContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the back content template.
        /// </summary>
        public DataTemplate BackContentTemplate
        {
            get
            {
                return this.GetValue(BackContentTemplateProperty) as DataTemplate;
            }

            set
            {
                this.SetValue(BackContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is flipped.
        /// </summary>
        public bool IsFlipped
        {
            get
            {
                return (bool)this.GetValue(IsFlippedProperty);
            }
            set
            {
                this.SetValue(IsFlippedProperty, value);
            }
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected override string ComposeVisualStateName()
        {
            bool isInitialUpdate = string.IsNullOrEmpty(this.CurrentVisualState);

            if (this.BackContent == null && this.BackContentTemplate == null)
            {
                this.ChangeIsFlippedProperty(false);
            }
            else if (!isInitialUpdate)
            {
                // Toggle the IsFlipped property
                this.ChangeIsFlippedProperty(!this.IsFlipped);
            }

            return this.IsFlipped ? "Flipped" : "NotFlipped";
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new FlipControlAutomationPeer(this);
        }

        private static void OnIsFlippedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var flipControl = (FlipControl)d;

            if (!flipControl.IsInternalPropertyChange)
            {
                flipControl.updatingIsFlipped = true;
                flipControl.UpdateVisualState(flipControl.IsLoaded);
                flipControl.updatingIsFlipped = false;
            }
        }

        private void ChangeIsFlippedProperty(bool value)
        {
            if (!this.updatingIsFlipped)
            {
                this.ChangePropertyInternally(FlipControl.IsFlippedProperty, value);
            }
        }
    }
}
