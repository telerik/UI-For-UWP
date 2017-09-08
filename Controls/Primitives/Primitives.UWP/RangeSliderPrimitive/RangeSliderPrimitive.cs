using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives.RangeSlider;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents an input slider control that can select a logical range.
    /// </summary>
    public partial class RangeSliderPrimitive : SliderBase
    {
        /// <summary>
        /// Identifies the <see cref="SelectionStartThumbStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionStartThumbStyleProperty =
            DependencyProperty.Register(nameof(SelectionStartThumbStyle), typeof(Style), typeof(RangeSliderPrimitive), null);

        /// <summary>
        /// Identifies the <see cref="SelectionEndThumbStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionEndThumbStyleProperty =
            DependencyProperty.Register(nameof(SelectionEndThumbStyle), typeof(Style), typeof(RangeSliderPrimitive), null);

        /// <summary>
        /// Identifies the <see cref="SelectionMiddleThumbStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionMiddleThumbStyleProperty =
            DependencyProperty.Register(nameof(SelectionMiddleThumbStyle), typeof(Style), typeof(RangeSliderPrimitive), null);

        internal double coeficient;
        internal Popup rangeToolTip;
        internal RangeToolTip rangeToolTipContent;
        internal Point toolTipPosition;
        internal ToolTip valueToolTip;
        internal Point sliderPrimitivePosition;

        private const long RepeatTimeSpan = 10000000;

        private Thumb selectionEndThumb;
        private Thumb selectionStartThumb;
        private Thumb selectionMiddleThumb;
        private RangeTrackPrimitive trackBar;
        private ThumbsPanel thumbsPanel;
        private Grid sliderPrimitivePanel;

        private bool isLargeIncreaseButtonPressed;
        private bool shouldSwapValues;

        private bool isPointerOver = false;

        private double selectionStartOffset = 0d;
        private double selectionEndOffset = 0d;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeSliderPrimitive"/> class.
        /// </summary>
        public RangeSliderPrimitive()
        {
            this.rangeToolTip = new Popup();

            this.rangeToolTipContent = new RangeToolTip();

            this.rangeToolTip.Child = this.rangeToolTipContent;
            this.valueToolTip = new ToolTip();
            this.valueToolTip.LayoutUpdated += this.OnvalueToolTipLayoutUpdated;

            this.toolTipPosition = new Point();
            this.sliderPrimitivePosition = new Point();

            this.DefaultStyleKey = typeof(RangeSliderPrimitive);
            this.PointerReleased += this.OnSliderPointerReleased;

            this.thumbsTimer = new DispatcherTimer();
            this.thumbsTimer.Interval = new TimeSpan(RepeatTimeSpan);
            this.thumbsTimer.Tick += this.OnTimerTick;
            this.InitializeModel();
        }

        internal event EventHandler SelectionOffsetChanged;

        /// <summary>
        /// Gets or sets the <see cref="SelectionStartThumbStyle"/> that defines the appearance settings
        /// applied to the <see cref="SelectionStartThumb"/> control.
        /// </summary>
        /// <remarks>
        /// The Style should target the <see cref="Thumb"/> class.
        /// </remarks>
        /// <example>
        /// <para>This example demonstrates how to set a custom style to the <see cref="SelectionStartThumb"/> element.</para>
        /// <para>The following namespaces have to be added to the page:</para>
        /// <code language="xaml">
        /// xmlns:telerikInput="using:Telerik.UI.Xaml.Controls.Input"
        /// xmlns:telerikPrimitives="using:Telerik.UI.Xaml.Controls.Primitives"
        /// </code>
        /// <para>Here is how you change the style of the <see cref="SelectionStartThumb"/>:</para>
        /// <code language="xaml">
        /// &lt;telerikInput:RadRangeSlider &gt;
        ///     &lt;telerikInput:RadRangeSlider.SliderPrimitiveStyle&gt;
        ///         &lt;Style TargetType="telerikPrimitives:RangeSliderPrimitive"&gt;
        ///             &lt;Setter Property="SelectionStartThumbStyle"&gt;
        ///                 &lt;Setter.Value&gt;
        ///                     &lt;Style TargetType="Thumb"&gt;
        ///                         &lt;Setter Property="Background" Value="Orange"/&gt;
        ///                         &lt;Setter Property="Height" Value="13"/&gt;
        ///                         &lt;Setter Property="Width" Value="13"/&gt;
        ///                     &lt;/Style&gt;
        ///                 &lt;/Setter.Value&gt;
        ///             &lt;/Setter&gt;
        ///         &lt;/Style&gt;
        ///     &lt;/telerikInput:RadRangeSlider.SliderPrimitiveStyle&gt;
        /// &lt;/telerikInput:RadRangeSlider&gt;
        /// </code>
        /// </example>
        public Style SelectionStartThumbStyle
        {
            get
            {
                return (Style)GetValue(SelectionStartThumbStyleProperty);
            }
            set
            {
                this.SetValue(SelectionStartThumbStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="SelectionEndThumbStyle"/> that defines the appearance settings applied to the <see cref="SelectionEndThumb"/> control.
        /// </summary>
        /// <remarks>
        /// The Style should target the <see cref="Thumb"/> class.
        /// </remarks>
        /// <example>
        /// <para>This example demonstrates how to set a custom style to the <see cref="SelectionEndThumb"/> element.</para>
        /// <para>The following namespaces have to be added to the page:</para>
        /// <code language="xaml">
        /// xmlns:telerikInput="using:Telerik.UI.Xaml.Controls.Input"
        /// xmlns:telerikPrimitives="using:Telerik.UI.Xaml.Controls.Primitives"
        /// </code>
        /// <para>Here is how you change the style of the <see cref="SelectionStartThumb"/>:</para>
        /// <code language="xaml">
        /// &lt;telerikInput:RadRangeSlider &gt;
        ///     &lt;telerikInput:RadRangeSlider.SliderPrimitiveStyle&gt;
        ///         &lt;Style TargetType="telerikPrimitives:RangeSliderPrimitive"&gt;
        ///             &lt;Setter Property="SelectionEndThumbStyle"&gt;
        ///                 &lt;Setter.Value&gt;
        ///                     &lt;Style TargetType="Thumb"&gt;
        ///                         &lt;Setter Property="Background" Value="Orange"/&gt;
        ///                         &lt;Setter Property="Height" Value="13"/&gt;
        ///                         &lt;Setter Property="Width" Value="13"/&gt;
        ///                     &lt;/Style&gt;
        ///                 &lt;/Setter.Value&gt;
        ///             &lt;/Setter&gt;
        ///         &lt;/Style&gt;
        ///     &lt;/telerikInput:RadRangeSlider.SliderPrimitiveStyle&gt;
        /// &lt;/telerikInput:RadRangeSlider&gt;
        /// </code>
        /// </example>
        public Style SelectionEndThumbStyle
        {
            get
            {
                return (Style)GetValue(SelectionEndThumbStyleProperty);
            }
            set
            {
                this.SetValue(SelectionEndThumbStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="SelectionMiddleThumbStyle"/> that defines the appearance settings applied to the 
        /// <see cref="SelectionMiddleThumb"/> control (the one between <see cref="SelectionStartThumb"/> and <see cref="SelectionEndThumb"/>).
        /// </summary>
        /// <remarks>
        /// The Style should target the <see cref="Thumb"/> class.
        /// </remarks>
        /// <example>
        /// <para>This example demonstrates how to set a custom style to the <see cref="SelectionMiddleThumb"/> element.</para>
        /// <para>The following namespaces have to be added to the page:</para>
        /// <code language="xaml">
        /// xmlns:telerikInput="using:Telerik.UI.Xaml.Controls.Input"
        /// xmlns:telerikPrimitives="using:Telerik.UI.Xaml.Controls.Primitives"
        /// </code>
        /// <para>Here is how you change the style of the <see cref="SelectionMiddleThumb"/>:</para>
        /// <code language="xaml">
        /// &lt;telerikInput:RadRangeSlider &gt;
        ///     &lt;telerikInput:RadRangeSlider.SliderPrimitiveStyle&gt;
        ///         &lt;Style TargetType="telerikPrimitives:RangeSliderPrimitive"&gt;
        ///             &lt;Setter Property="SelectionMiddleThumbStyle"&gt;
        ///                 &lt;Setter.Value&gt;
        ///                     &lt;Style TargetType="Thumb"&gt;
        ///                         &lt;Setter Property="Background" Value="Orange"/&gt;
        ///                         &lt;Setter Property="Height" Value="13"/&gt;
        ///                     &lt;/Style&gt;
        ///                 &lt;/Setter.Value&gt;
        ///             &lt;/Setter&gt;
        ///         &lt;/Style&gt;
        ///     &lt;/telerikInput:RadRangeSlider.SliderPrimitiveStyle&gt;
        /// &lt;/telerikInput:RadRangeSlider&gt;
        /// </code>
        /// </example>
        public Style SelectionMiddleThumbStyle
        {
            get
            {
                return (Style)GetValue(SelectionMiddleThumbStyleProperty);
            }
            set
            {
                this.SetValue(SelectionMiddleThumbStyleProperty, value);
            }
        }

        internal double SelectionStartOffset
        {
            get
            {
                return this.selectionStartOffset;
            }
            private set
            {
                if (this.selectionStartOffset != value)
                {
                    this.selectionStartOffset = value;
                    this.OnSelectionOffsetChanged(EventArgs.Empty);
                }
            }
        }

        internal double SelectionEndOffset
        {
            get
            {
                return this.selectionEndOffset;
            }
            private set
            {
                if (this.selectionEndOffset != value)
                {
                    this.selectionEndOffset = value;
                    this.OnSelectionOffsetChanged(EventArgs.Empty);
                }
            }
        }

        internal Thumb SelectionEndThumb
        {
            get
            {
                return this.selectionEndThumb;
            }
        }

        internal Thumb SelectionStartThumb
        {
            get
            {
                return this.selectionStartThumb;
            }
        }

        internal Thumb SelectionMiddleThumb
        {
            get
            {
                return this.selectionMiddleThumb;
            }
        }

        internal FrameworkElement TrackBar
        {
            get
            {
                return this.trackBar;
            }
        }

        internal virtual void OnSelectionOffsetChanged(EventArgs e)
        {
            EventHandler handler = this.SelectionOffsetChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            if (double.IsNaN(newMinimum))
            {
                this.Minimum = RangeSliderPrimitive.DefaultMinimumValue;
            }

            this.TryUpdate();
            this.UpdateCoeficient();

            RangeSliderPrimitiveAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as RangeSliderPrimitiveAutomationPeer;
            if (peer != null)
            {
                peer.RaiseMinimumPropertyChangedEvent((double)oldMinimum, (double)newMinimum);
            }
        }

        internal override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            if (double.IsNaN(newMaximum))
            {
                this.Maximum = RangeSliderPrimitive.DefaultMaximumValue;
            }

            this.TryUpdate();
            this.UpdateCoeficient();

            RangeSliderPrimitiveAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as RangeSliderPrimitiveAutomationPeer;
            if (peer != null)
            {
                peer.RaiseMaximumPropertyChangedEvent((double)oldMaximum, (double)newMaximum);
            }
        }

        internal override void OnOrientationChanged(Orientation oldOrientation, Orientation newOrientation)
        {
            if (newOrientation == Orientation.Horizontal && oldOrientation == Orientation.Vertical)
            {
                this.shouldSwapValues = true;
                this.SwapWidthHeightProperties();
                this.valueToolTip.Placement = Windows.UI.Xaml.Controls.Primitives.PlacementMode.Top;
            }

            if (newOrientation == Orientation.Vertical && oldOrientation == Orientation.Horizontal)
            {
                this.shouldSwapValues = true;
                this.SwapWidthHeightProperties();
                this.valueToolTip.Placement = Windows.UI.Xaml.Controls.Primitives.PlacementMode.Left;
            }
        }

        internal override void OnSelectionStartChanged(double oldSelectionStart, double newSelectionStart)
        {
            if (double.IsNaN(newSelectionStart))
            {
                this.SelectionStart = RangeSliderPrimitive.DefaultSelectionStartValue;
            }

            if (!this.IsInternalPropertyChange)
            {
                this.DesiredSelectionStart = newSelectionStart;
                newSelectionStart = this.Snap(newSelectionStart);

                this.ChangePropertyInternally(RangeSliderPrimitive.SelectionStartProperty, newSelectionStart);
            }

            this.visualSelection.Start = newSelectionStart;
            this.TryUpdate();
        }

        internal override void OnSelectionEndChanged(double oldSelectionEnd, double newSelectionEnd)
        {
            if (double.IsNaN(newSelectionEnd))
            {
                this.SelectionEnd = RangeSliderPrimitive.DefaultSelectionEndValue;
            }

            if (!this.IsInternalPropertyChange)
            {
                this.DesiredSelectionEnd = newSelectionEnd;
                newSelectionEnd = this.Snap(newSelectionEnd);

                this.ChangePropertyInternally(RangeSliderPrimitive.SelectionEndProperty, newSelectionEnd);
            }

            this.visualSelection.End = newSelectionEnd;
            this.TryUpdate();
        }

        internal override void OnSnapsToPropertyChanged(SnapsTo oldSnapsTo, SnapsTo newSnapsTo)
        {
            this.UpdateValues();
            this.InvalidateThumbsPanelArrange();
        }

        internal override void OnShowValueToolTipPropertyChanged(bool oldShowValueToolTip, bool newShowValueToolTip)
        {
            this.valueToolTip.IsEnabled = newShowValueToolTip;
        }

        internal void UpdateThumbsOffset()
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            if (this.selectionStartThumb != null)
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    this.SelectionStartOffset = this.selectionStartThumb.DesiredSize.Width;
                }
                else
                {
                    this.SelectionStartOffset = this.selectionStartThumb.DesiredSize.Height;
                }
            }

            if (this.selectionEndThumb != null)
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    this.SelectionEndOffset = this.selectionEndThumb.DesiredSize.Width;
                }
                else
                {
                    this.SelectionEndOffset = this.selectionEndThumb.DesiredSize.Height;
                }
            }
        }

        internal string ReturnFormatedValue(double value)
        {
            return string.Format(CultureInfo.CurrentUICulture, this.ToolTipFormat, value);
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RangeSliderPrimitiveAutomationPeer(this);
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.sliderPrimitivePanel = this.GetTemplatePartField<Grid>("PART_SliderPrimitivePanel");
            applied = applied && this.sliderPrimitivePanel != null;

            this.thumbsPanel = this.GetTemplatePartField<ThumbsPanel>("PART_ThumbsPanel");
            applied = applied && this.thumbsPanel != null;

            this.trackBar = this.GetTemplatePartField<RangeTrackPrimitive>("PART_TrackBar");
            applied = applied && this.trackBar != null;

            this.selectionEndThumb = this.GetTemplatePartField<Thumb>("PART_SelectionEndThumb");
            applied = applied && this.selectionEndThumb != null;

            this.selectionStartThumb = this.GetTemplatePartField<Thumb>("PART_SelectionStartThumb");
            applied = applied && this.selectionStartThumb != null;

            this.selectionMiddleThumb = this.GetTemplatePartField<Thumb>("PART_SelectionMiddleThumb");
            applied = applied && this.selectionMiddleThumb != null;

            return applied;
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected override string ComposeVisualStateName()
        {
            if (this.isPointerOver)
            {
                return "PointerOver";
            }

            return base.ComposeVisualStateName();
        }

        /// <summary>
        /// Called before the PointerExited event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            this.isPointerOver = false;

            this.UpdateVisualState(true);
        }

        /// <summary>
        /// Called before the PointerEntered event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            this.isPointerOver = true;

            this.UpdateVisualState(true);
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.isLoaded = true;
            this.TryUpdate();

            this.thumbsPanel.Owner = this;

            if (this.shouldSwapValues)
            {
                this.SwapWidthHeightProperties();
            }

            ToolTipService.SetToolTip(this, this.valueToolTip);
            this.sliderPrimitivePanel.Children.Add(this.rangeToolTip);
            this.rangeToolTipContent.Transitions = new TransitionCollection();

            this.rangeToolTipContent.Transitions.Add(new PopupThemeTransition() { FromHorizontalOffset = 0, FromVerticalOffset = 0 });

            this.rangeToolTipContent.Owner = this;
            this.rangeToolTip.IsOpen = false;

            this.AttachThumbsEvents();
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            if (this.thumbsPanel != null)
            {
                this.thumbsPanel.Owner = null;
            }

            this.DetachThumbsEvents();

            base.UnapplyTemplateCore();
        }

        /// <summary>
        /// Called in the measure layout pass to determine the desired size.
        /// </summary>
        /// <param name="availableSize">The available size that was given by the layout system.</param>
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);

            return this.thumbsPanel.DesiredSize;
        }

        /// <summary>
        /// Provides the behavior for the Arrange pass of layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = base.ArrangeOverride(finalSize);
            this.UpdateCoeficient(finalSize);

            return size;
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Unloaded" /> event. Allows inheritors to provide their specific logic.
        /// </summary>
        protected override void UnloadCore()
        {
            base.UnloadCore();

            this.StopTimer();
        }

        private void OnvalueToolTipLayoutUpdated(object sender, object e)
        {
            if (this.ShowValueToolTip && this.ShowRangeToolTip && this.rangeToolTip.IsOpen)
            {
                this.valueToolTip.IsOpen = false;
            }
        }

        private void InvalidateThumbsPanelArrange()
        {
            if (this.thumbsPanel != null)
            {
                this.thumbsPanel.InvalidateArrange();
            }
        }

        private void UpdateCoeficient()
        {
            this.UpdateCoeficient(new Size(this.ActualWidth, this.ActualHeight));
        }

        private void UpdateCoeficient(Size size)
        {
            var point = this.Orientation == Orientation.Horizontal ? size.Width : size.Height;
            this.coeficient = this.CalculateCoeficient(point);
        }

        private void InvalidateThumbsPanelMeasure()
        {
            if (this.thumbsPanel != null)
            {
                this.thumbsPanel.InvalidateMeasure();
            }
        }

        private void SwapWidthHeightProperties()
        {
            if (this.selectionStartThumb != null && this.selectionEndThumb != null && this.selectionMiddleThumb != null && this.trackBar != null)
            {
                Size oldSize = new Size(this.selectionStartThumb.Width, this.selectionStartThumb.Height);
                this.selectionStartThumb.Width = oldSize.Height;
                this.selectionStartThumb.Height = oldSize.Width;

                oldSize = new Size(this.selectionEndThumb.Width, this.selectionEndThumb.Height);
                this.selectionEndThumb.Width = oldSize.Height;
                this.selectionEndThumb.Height = oldSize.Width;

                if (this.Orientation == Orientation.Vertical)
                {
                    this.selectionMiddleThumb.Width = this.selectionMiddleThumb.Height;
                    this.selectionMiddleThumb.Height = double.NaN;
                    this.trackBar.Width = this.trackBar.Height;
                    this.trackBar.Height = double.NaN;
                }
                else
                {
                    this.selectionMiddleThumb.Height = this.selectionMiddleThumb.Width;
                    this.selectionMiddleThumb.Width = double.NaN;
                    this.trackBar.Height = this.trackBar.Width;
                    this.trackBar.Width = double.NaN;
                }

                this.shouldSwapValues = false;
                this.UpdateThumbsOffset();
                this.InvalidateMeasure();
            }
        }
    }
}