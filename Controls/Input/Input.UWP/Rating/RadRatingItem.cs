using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents an item from the <see cref="RadRating"/> control.
    /// </summary>
    [ContentProperty(Name = "Content")]
    public class RadRatingItem : RadContentControl
    {
        /// <summary>
        /// Identifies the <see cref="EmptyIconContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EmptyIconContentProperty = DependencyProperty.Register("EmptyIconContent", typeof(object), typeof(RadRatingItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="EmptyIconContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EmptyIconContentTemplateProperty = DependencyProperty.Register("EmptyIconContentTemplate", typeof(DataTemplate), typeof(RadRatingItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="EmptyIconStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EmptyIconStyleProperty = DependencyProperty.Register("EmptyIconStyle", typeof(Style), typeof(RadRatingItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="FilledIconContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FilledIconContentProperty = DependencyProperty.Register("FilledIconContent", typeof(object), typeof(RadRatingItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="FilledIconContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FilledIconContentTemplateProperty = DependencyProperty.Register("FilledIconContentTemplate", typeof(DataTemplate), typeof(RadRatingItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="FilledIconStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FilledIconStyleProperty = DependencyProperty.Register("FilledIconStyle", typeof(Style), typeof(RadRatingItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="HighlightedIconStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HighlightedIconStyleProperty = DependencyProperty.Register("HighlightedIconStyle", typeof(Style), typeof(RadRatingItem), new PropertyMetadata(null));

        internal bool isArranged;

        private const string InvalidOwnerException = "RadRatingItem is intended to be placed inside RadRating control template";
        private double fillRatio;
        private ContentPresenter filledIconPresenter;
        private Border filledIconContainer;
        private Border emptyIconContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadRatingItem" /> class.
        /// </summary>
        public RadRatingItem()
        {
            this.DefaultStyleKey = typeof(RadRatingItem);
        }

        /// <summary>
        /// Gets or sets the style of the icon content in highlighted state.
        /// </summary>
        public Style HighlightedIconStyle
        {
            get
            {
                return (Style)this.GetValue(HighlightedIconStyleProperty);
            }
            set
            {
                this.SetValue(HighlightedIconStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style of the icon content when the item is selected.
        /// </summary>
        public Style FilledIconStyle
        {
            get
            {
                return (Style)this.GetValue(FilledIconStyleProperty);
            }
            set
            {
                this.SetValue(FilledIconStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style of the icon content when the item is not selected.
        /// </summary>
        public Style EmptyIconStyle
        {
            get
            {
                return (Style)this.GetValue(EmptyIconStyleProperty);
            }
            set
            {
                this.SetValue(EmptyIconStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content of the icon when the item is selected.
        /// </summary>
        public object FilledIconContent
        {
            get
            {
                return this.GetValue(FilledIconContentProperty);
            }
            set
            {
                this.SetValue(FilledIconContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content of the icon when the item is not selected.
        /// </summary>
        public object EmptyIconContent
        {
            get
            {
                return this.GetValue(EmptyIconContentProperty);
            }
            set
            {
                this.SetValue(EmptyIconContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content template of the icon when the item is selected.
        /// </summary>
        public object FilledIconContentTemplate
        {
            get
            {
                return this.GetValue(FilledIconContentTemplateProperty);
            }
            set
            {
                this.SetValue(FilledIconContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content template of the icon when the item is not selected.
        /// </summary>
        public object EmptyIconContentTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(EmptyIconContentTemplateProperty);
            }
            set
            {
                this.SetValue(EmptyIconContentTemplateProperty, value);
            }
        }

        internal RadRating Owner { get; set; }

        internal double FillRatio
        {
            get
            {
                return this.fillRatio;
            }
            set
            {
                if (this.fillRatio != value)
                {
                    this.fillRatio = value;
                    this.UpdateClip();
                }
            }
        }

        internal bool HitTest(Point touchPoint)
        {
            Rect layoutSlot = LayoutInformation.GetLayoutSlot(this);
            return layoutSlot.Contains(touchPoint);
        }

        internal void SetHighlightMode(bool isHighlighted)
        {
            if (isHighlighted)
            {
                this.filledIconPresenter.Style = this.HighlightedIconStyle;
            }
            else
            {
                this.filledIconPresenter.Style = this.FilledIconStyle;
            }
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.filledIconPresenter = this.GetTemplateChild("FilledIconPresenter") as ContentPresenter;
            if (this.filledIconPresenter == null)
            {
                throw new MissingTemplatePartException("FilledIconPresenter", typeof(ContentPresenter));
            }

            this.filledIconContainer = this.GetTemplateChild("FilledIconContainer") as Border;
            if (this.filledIconContainer == null)
            {
                throw new MissingTemplatePartException("FilledIconContainer", typeof(Border));
            }

            this.emptyIconContainer = this.GetTemplateChild("EmptyIconContainer") as Border;
            if (this.filledIconContainer == null)
            {
                throw new MissingTemplatePartException("EmptyIconContainer", typeof(Border));
            }

            this.Initialize();
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.isArranged = false;
            var final = base.ArrangeOverride(finalSize);
            this.isArranged = true;
            this.UpdateClip();
            return final;
        }

        /// <inheritdoc/>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
            this.HandleTapEvent();
        }

        private void UpdateClip()
        {
            if (!this.IsTemplateApplied || !this.isArranged || this.Owner == null)
            {
                return;
            }

            this.UpdateElementClip(this.filledIconContainer, this.FillRatio, this.Owner.Orientation, 0);
            this.UpdateElementClip(this.emptyIconContainer, 1 - this.FillRatio, this.Owner.Orientation, this.FillRatio);
        }

        private void UpdateElementClip(FrameworkElement element, double fillRatio, Orientation orientation, double fillStart)
        {
            double iconHeight = element.ActualHeight;
            double iconWidth = element.ActualWidth;

            double clippingWidth;
            double clippingHeight;
            double clippingTop;
            double clippingLeft;
            RectangleGeometry clipper = new RectangleGeometry();

            if (orientation == Orientation.Horizontal)
            {
                clippingWidth = iconWidth * fillRatio;
                clippingHeight = iconHeight;
                clippingTop = 0;
                clippingLeft = iconWidth * fillStart;
                clipper.Rect = new Rect(clippingLeft, clippingTop, clippingWidth, clippingHeight);
            }
            else
            {
                clippingWidth = iconWidth;
                clippingHeight = iconHeight * fillRatio;
                clippingTop = (1 - fillStart) * iconHeight - clippingHeight;
                clippingLeft = 0;
                clipper.Rect = new Rect(clippingLeft, clippingTop, clippingWidth, clippingHeight);
            }

            element.Clip = clipper;
        }

        private void Initialize()
        {
            if (this.Owner == null)
            {
                RadRating parent = ElementTreeHelper.FindVisualAncestor<RadRating>(this);
                if (parent == null)
                {
                    throw new InvalidOperationException(InvalidOwnerException);
                }

                this.Owner = parent;
            }
        }

        private void HandleTapEvent()
        {
            if (!this.Owner.IsReadOnly)
            {
                this.Owner.HandleTapEvent(this);
            }
        }
    }
}
