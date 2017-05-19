using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Input.RangeSlider;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents an input control that can be used for visual selection of a logical range. The selection range is visualized with scaled axes.
    /// </summary>
    public class RadRangeSlider : SliderBase
    {
        /// <summary>
        /// Identifies the <see cref="TopLeftScaleStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TopLeftScaleStyleProperty =
          DependencyProperty.Register(nameof(TopLeftScaleStyle), typeof(Style), typeof(RadRangeSlider), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="BottomRightScaleStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomRightScaleStyleProperty =
          DependencyProperty.Register(nameof(BottomRightScaleStyle), typeof(Style), typeof(RadRangeSlider), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SliderPrimitiveStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SliderPrimitiveStyleProperty =
          DependencyProperty.Register(nameof(SliderPrimitiveStyle), typeof(Style), typeof(RadRangeSlider), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="LabelFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFormatProperty =
          DependencyProperty.Register(nameof(LabelFormat), typeof(string), typeof(RadRangeSlider), new PropertyMetadata(null));

        private const string RangeSliderPanelName = "PART_RangeSliderPanel";
        private const string RangeSliderPrimitivePartName = "PART_RangeSliderPrimitive";
        private const string TopLeftScalePartName = "PART_ScaleTopLeft";
        private const string BottomRightScalePartName = "PART_ScaleBottomRight";

        private RangeSliderPanel rangeSliderPanel;
        private RangeSliderPrimitive rangeSlider;
        private ScalePrimitive scaleTopLeft;
        private ScalePrimitive scaleBottomRight;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadRangeSlider"/> class.
        /// </summary>
        public RadRangeSlider()
        {
            this.DefaultStyleKey = typeof(RadRangeSlider);
        }

        /// <summary>
        /// Gets or sets the <see cref="SliderPrimitiveStyle"/> that defines the appearance settings applied to the inner RangeSliderPrimitive control.
        /// </summary>
        /// <remarks>
        /// The style should target the <see cref="Telerik.UI.Xaml.Controls.Primitives.RangeSliderPrimitive"/> type.
        /// </remarks>
        /// <example>
        /// <para>THis example demonstrates how to set a custom style to the <see cref="RangeSliderPrimitive"/> control.</para>
        /// <para>The following namespaces have to be added to the page:</para>
        /// <code language="xaml">
        /// xmlns:telerikInput="using:Telerik.UI.Xaml.Controls.Input"
        /// xmlns:telerikPrimitives="using:Telerik.UI.Xaml.Controls.Primitives"
        /// </code>
        /// <para>Here is how you change the style of the <see cref="RangeSliderPrimitive.SelectionStartThumb"/>:</para>
        /// <code language="xaml">
        /// &lt;telerikInput:RadRangeSlider &gt;
        ///     &lt;telerikInput:RadRangeSlider.SliderPrimitiveStyle&gt;
        ///         &lt;Style TargetType="telerikPrimitives:RangeSliderPrimitive"&gt;
        ///             &lt;!-- set the RangeSliderPrimitive properties here --&gt;
        ///         &lt;/Style&gt;
        ///     &lt;/telerikInput:RadRangeSlider.SliderPrimitiveStyle&gt;
        /// &lt;/telerikInput:RadRangeSlider&gt;
        /// </code>
        /// </example>
        public Style SliderPrimitiveStyle
        {
            get
            {
                return (Style)this.GetValue(SliderPrimitiveStyleProperty);
            }
            set
            {
                this.SetValue(SliderPrimitiveStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="TopLeftScaleStyle"/> that defines the appearance settings applied to the top/left Scale control.
        /// </summary>
        /// <remarks>
        /// The style should target the <see cref="Telerik.UI.Xaml.Controls.Primitives.ScalePrimitive"/> type.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadRangeSlider&gt;
        ///     &lt;telerikInput:RadRangeSlider.TopLeftScaleStyle&gt;
        ///         &lt;Style TargetType="telerikPrimitives:ScalePrimitive"&gt;
        ///             &lt;Setter Property="LabelPlacement" Value="None"/&gt;
        ///             &lt;Setter Property="TickPlacement" Value="BottomRight"/&gt;
        ///             &lt;Setter Property="TickTemplate"&gt;
        ///                 &lt;Setter.Value&gt;
        ///                     &lt;DataTemplate&gt;
        ///                         &lt;Ellipse Width="5" Height="5" Margin="-2.5,0,0,10" Fill="LimeGreen"/&gt;
        ///                     &lt;/DataTemplate&gt;
        ///                 &lt;/Setter.Value&gt;
        ///             &lt;/Setter&gt;
        ///         &lt;/Style&gt;
        ///     &lt;/telerikInput:RadRangeSlider.TopLeftScaleStyle&gt;
        /// &lt;/telerikInput:RadRangeSlider&gt;
        /// </code>
        /// </example>
        public Style TopLeftScaleStyle
        {
            get
            {
                return (Style)this.GetValue(TopLeftScaleStyleProperty);
            }
            set
            {
                this.SetValue(TopLeftScaleStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="BottomRightScaleStyle"/> that defines the appearance settings applied to the bottom/right Scale control.
        /// </summary>
        /// <remarks>
        /// The style should target the <see cref="Telerik.UI.Xaml.Controls.Primitives.ScalePrimitive"/> type.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadRangeSlider&gt;
        ///     &lt;telerikInput:RadRangeSlider.BottomRightScaleStyle&gt;
        ///         &lt;Style TargetType="telerikPrimitives:ScalePrimitive"&gt;
        ///             &lt;Setter Property="LabelPlacement" Value="BottomRight"/&gt;
        ///             &lt;Setter Property="TickPlacement" Value="TopLeft"/&gt;
        ///             &lt;Setter Property="TickTemplate"&gt;
        ///                 &lt;Setter.Value&gt;
        ///                     &lt;DataTemplate&gt;
        ///                         &lt;Ellipse Width="5" Height="5" Margin="-2.5,10,0,0" Fill="LimeGreen"/&gt;
        ///                     &lt;/DataTemplate&gt;
        ///                 &lt;/Setter.Value&gt;
        ///             &lt;/Setter&gt;
        ///         &lt;/Style&gt;
        ///     &lt;/telerikInput:RadRangeSlider.BottomRightScaleStyle&gt;
        /// &lt;/telerikInput:RadRangeSlider&gt;
        /// </code>
        /// </example>
        public Style BottomRightScaleStyle
        {
            get
            {
                return (Style)this.GetValue(BottomRightScaleStyleProperty);
            }
            set
            {
                this.SetValue(BottomRightScaleStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the string that is used to format the labels of the component. 
        /// The value passed should follow the syntax, expected by the runtime 
        /// String.Format: { index[,alignment][:formatString] }.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadRangeSlider Minimum="0" Maximum="1" LabelFormat="{}{0:P0}"/&gt;
        /// </code>
        /// </example>
        public string LabelFormat
        {
            get
            {
                return (string)this.GetValue(LabelFormatProperty);
            }
            set
            {
                this.SetValue(LabelFormatProperty, value);
            }
        }

        internal RangeSliderPrimitive RangeSlider
        {
            get
            {
                return this.rangeSlider;
            }
        }

        internal ScalePrimitive ScaleTopLeft
        {
            get
            {
                return this.scaleTopLeft;
            }
        }

        internal ScalePrimitive ScaleBottomRight
        {
            get
            {
                return this.scaleBottomRight;
            }
        }

        internal override void OnSelectionStartChanged(double oldSelectionStart, double newSelectionStart)
        {
            base.OnSelectionStartChanged(oldSelectionStart, newSelectionStart);
            if (!this.IsTemplateApplied)
            {
                return;
            }

            double calculatedSelectionStart = RangeSliderPrimitive.CalculateSelectionStart(this.Minimum, this.Maximum, this.SelectionStart, this.SelectionStart);
            this.SelectionStart = this.rangeSlider.Snap(calculatedSelectionStart);

            RadRangeSliderAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as RadRangeSliderAutomationPeer;
            if (peer != null)
            {
                peer.RaiseValuePropertyChangedEvent(oldSelectionStart, newSelectionStart);
            }
        }

        internal override void OnSelectionEndChanged(double oldSelectionEnd, double newSelectionEnd)
        {
            base.OnSelectionEndChanged(oldSelectionEnd, newSelectionEnd);
            if (!this.IsTemplateApplied)
            {
                return;
            }

            double calculatedSelectionEnd = this.rangeSlider.CalculateSelectionEnd(this.Maximum, this.SelectionStart, this.SelectionEnd, this.SelectionEnd);
            this.SelectionEnd = this.rangeSlider.Snap(calculatedSelectionEnd);

            RadRangeSliderAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as RadRangeSliderAutomationPeer;
            if (peer != null)
            {
                peer.RaiseValuePropertyChangedEvent(oldSelectionEnd, newSelectionEnd);
            }
        }

        /// <summary>
        /// Called when the Framework <see cref="M:RadControl.OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.rangeSliderPanel = this.GetTemplatePartField<RangeSliderPanel>(RangeSliderPanelName);
            applied = applied && this.rangeSliderPanel != null;

            this.rangeSlider = this.GetTemplatePartField<RangeSliderPrimitive>(RangeSliderPrimitivePartName);
            applied = applied && this.rangeSlider != null;

            this.scaleTopLeft = this.GetTemplatePartField<ScalePrimitive>(TopLeftScalePartName);
            applied = applied && this.scaleTopLeft != null;

            this.scaleBottomRight = this.GetTemplatePartField<ScalePrimitive>(BottomRightScalePartName);
            applied = applied && this.scaleBottomRight != null;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.rangeSlider.SelectionOffsetChanged += this.OnSelectionOffsetChanged;

            this.rangeSliderPanel.Owner = this;
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            this.rangeSlider.SelectionOffsetChanged -= this.OnSelectionOffsetChanged;
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadRangeSliderAutomationPeer(this);
        }

        private void OnSelectionOffsetChanged(object sender, EventArgs e)
        {
            this.rangeSliderPanel.InvalidateMeasure();
        }
    }
}
