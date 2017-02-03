using System;
using System.Globalization;
using Telerik.UI.Xaml.Controls.Primitives.Scale;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a component which visualize a line scale with ticks and labels.
    /// </summary>
    public class ScalePrimitive : RangeControlBase
    {
        /// <summary>
        /// Identifies the <see cref="TickFrequency"/> property.
        /// </summary>
        public static readonly DependencyProperty TickFrequencyProperty =
            DependencyProperty.Register(nameof(TickFrequency), typeof(double), typeof(ScalePrimitive), new PropertyMetadata(1d, OnTickFrequencyChanged));

        /// <summary>
        /// Identifies the <see cref="Orientation"/> property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(ScalePrimitive), new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

        /// <summary>
        /// Identifies the <see cref="LineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty LineStyleProperty =
            DependencyProperty.Register(nameof(LineStyle), typeof(Style), typeof(ScalePrimitive), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TickPlacement"/> property.
        /// </summary>
        public static readonly DependencyProperty TickPlacementProperty =
            DependencyProperty.Register(nameof(TickPlacement), typeof(ScaleElementPlacement), typeof(ScalePrimitive), new PropertyMetadata(DefaultTickPlacement, OnTickPlacementChanged));

        /// <summary>
        /// Identifies the <see cref="LabelPlacement"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelPlacementProperty =
            DependencyProperty.Register(nameof(LabelPlacement), typeof(ScaleElementPlacement), typeof(ScalePrimitive), new PropertyMetadata(DefaultTickPlacement, OnLabelPlacementChanged));

        /// <summary>
        /// Identifies the <see cref="TickStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty TickStyleProperty =
            DependencyProperty.Register(nameof(TickStyle), typeof(Style), typeof(ScalePrimitive), new PropertyMetadata(null, OnTickStyleChanged));

        /// <summary>
        /// Identifies the <see cref="TickThickness"/> property.
        /// </summary>
        public static readonly DependencyProperty TickThicknessProperty =
            DependencyProperty.Register(nameof(TickThickness), typeof(double), typeof(ScalePrimitive), new PropertyMetadata(DefaultTickThickness, OnTickThicknessChanged));

        /// <summary>
        /// Identifies the <see cref="TickThickness"/> property.
        /// </summary>
        public static readonly DependencyProperty TickLengthProperty =
            DependencyProperty.Register(nameof(TickLength), typeof(double), typeof(ScalePrimitive), new PropertyMetadata(DefaultTickLength, OnTickLengthChanged));

        /// <summary>
        /// Identifies the <see cref="TickTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty TickTemplateProperty =
            DependencyProperty.Register(nameof(TickTemplate), typeof(DataTemplate), typeof(ScalePrimitive), new PropertyMetadata(null, OnTickTemplateChanged));

        /// <summary>
        /// Identifies the <see cref="LabelFormat"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelFormatProperty =
            DependencyProperty.Register(nameof(LabelFormat), typeof(string), typeof(ScalePrimitive), new PropertyMetadata(null, OnLabelFormatChanged));

        /// <summary>
        /// Identifies the <see cref="LabelStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelStyleProperty =
            DependencyProperty.Register(nameof(LabelStyle), typeof(Style), typeof(ScalePrimitive), new PropertyMetadata(null, OnLabelStyleChanged));

        /// <summary>
        /// Identifies the <see cref="LabelTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelTemplateProperty =
            DependencyProperty.Register(nameof(LabelTemplate), typeof(DataTemplate), typeof(ScalePrimitive), new PropertyMetadata(null, OnLabelTemplateChanged));

        internal const double DefaultMinimum = 0;
        internal const double DefaultMaximum = 1;
        internal const double DefaultTickFrequency = 0.1;
        internal const Orientation DefaultOrientation = Orientation.Horizontal;
        internal const ScaleElementPlacement DefaultTickPlacement = ScaleElementPlacement.BottomRight;

        internal static readonly DependencyProperty AxisLineOffsetProperty =
    DependencyProperty.Register(nameof(AxisLineOffset), typeof(Thickness), typeof(ScalePrimitive), new PropertyMetadata(new Thickness(0), OnAxisLineOffsetChanged));

        private const double DefaultTickLength = 5;
        private const double DefaultTickThickness = 1;

        private const string PanelPartName = "PART_Panel";
        private const string LinePartName = "PART_Line";
        private const string AxisLineOffsetPropertyName = "AxisLineOffset";

        private Style tickStyleCache;
        private Style labelStyleCache;
        private DataTemplate tickTemplateCache;
        private DataTemplate labelTemplateCache;
        private Thickness axisLineOffsetCache;

        private ScalePanel panel;
        private Rectangle line;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScalePrimitive"/> class.
        /// </summary>
        public ScalePrimitive()
        {
            this.DefaultStyleKey = typeof(ScalePrimitive);
        }

        /// <summary>
        /// Gets or sets the style that defines the appearance of the the axis line of the <see cref="ScalePrimitive"/> control.
        /// </summary>
        /// <remarks>
        /// The Style should have <see cref="Style.TargetType"/>="<see cref="Rectangle"/>".
        /// </remarks>
        /// <example>
        /// <para>The following namespaces have to be added to the page:</para>
        /// <code language="xaml">
        /// xmlns:telerikInput="using:Telerik.UI.Xaml.Controls.Input"
        /// xmlns:telerikPrimitives="using:Telerik.UI.Xaml.Controls.Primitives"
        /// </code>
        /// <para>And here is how you set the LineStyle property:</para>
        /// <code language="xaml">
        /// &lt;telerikInput:RadRangeSlider&gt;
        ///     &lt;telerikInput:RadRangeSlider.BottomRightScaleStyle&gt;
        ///         &lt;Style TargetType="telerikPrimitives:ScalePrimitive"&gt;
        ///             &lt;Setter Property="LineStyle"&gt;
        ///                 &lt;Setter.Value&gt;
        ///                     &lt;Style TargetType="Rectangle"&gt;
        ///                         &lt;Setter Property="Stroke" Value="Red"/&gt;
        ///                     &lt;/Style&gt;
        ///                 &lt;/Setter.Value&gt;
        ///             &lt;/Setter&gt;
        ///         &lt;/Style&gt;
        ///     &lt;/telerikInput:RadRangeSlider.BottomRightScaleStyle&gt;
        /// &lt;/telerikInput:RadRangeSlider&gt;
        /// </code>
        /// </example>
        public Style LineStyle
        {
            get
            {
                return (Style)this.GetValue(LineStyleProperty);
            }
            set
            {
                this.SetValue(LineStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the logical tick frequency of the scale.
        /// </summary>
        /// <remarks>
        /// When the <see cref="ScalePrimitive"/> control is used in a <see cref="T:Telerik.UI.Xaml.Controls.Input.RadRangeSlider"/> control,
        /// its TickFrequency property is set by the <see cref="P:Telerik.UI.Xaml.Controls.Primitives.SliderBase.TickFrequency"/> property.
        /// </remarks>
        public double TickFrequency
        {
            get
            {
                return (double)this.GetValue(TickFrequencyProperty);
            }
            set
            {
                this.SetValue(TickFrequencyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the control.
        /// </summary>
        /// <remarks>
        /// When the <see cref="ScalePrimitive"/> control is used in a <see cref="T:Telerik.UI.Xaml.Controls.Input.RadRangeSlider"/> control,
        /// its Orientation property is set by the <see cref="P:Telerik.UI.Xaml.Controls.Primitives.SliderBase.Orientation"/> property.
        /// </remarks>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(OrientationProperty);
            }
            set
            {
                this.SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the position of the ticks relative to the axis line.
        /// </summary>
        /// <example>
        /// <para>The following namespaces have to be added to the page:</para>
        /// <code language="xaml">
        /// xmlns:telerikInput="using:Telerik.UI.Xaml.Controls.Input"
        /// xmlns:telerikPrimitives="using:Telerik.UI.Xaml.Controls.Primitives"
        /// </code>
        /// <para>And here is how you set the TickPlacement property:</para>
        /// <code language="xaml">
        /// &lt;telerikInput:RadRangeSlider&gt;
        ///     &lt;telerikInput:RadRangeSlider.BottomRightScaleStyle&gt;
        ///         &lt;Style TargetType="telerikPrimitives:ScalePrimitive"&gt;
        ///             &lt;Setter Property="TickPlacement" Value="Center"/&gt;
        ///             &lt;Setter Property="LineStyle"&gt;
        ///                 &lt;Setter.Value&gt;
        ///                     &lt;Style TargetType="Rectangle"&gt;
        ///                         &lt;Setter Property="Stroke" Value="Red"/&gt;
        ///                     &lt;/Style&gt;
        ///                 &lt;/Setter.Value&gt;
        ///             &lt;/Setter&gt;
        ///         &lt;/Style&gt;
        ///     &lt;/telerikInput:RadRangeSlider.BottomRightScaleStyle&gt;
        /// &lt;/telerikInput:RadRangeSlider&gt;
        /// </code>
        /// </example>
        public ScaleElementPlacement TickPlacement
        {
            get
            {
                return (ScaleElementPlacement)this.GetValue(TickPlacementProperty);
            }
            set
            {
                this.SetValue(TickPlacementProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the position of the labels relative to the axis line.
        /// </summary>
        /// <example>
        /// <para>The following namespaces have to be added to the page:</para>
        /// <code language="xaml">
        /// xmlns:telerikInput="using:Telerik.UI.Xaml.Controls.Input"
        /// xmlns:telerikPrimitives="using:Telerik.UI.Xaml.Controls.Primitives"
        /// </code>
        /// <para>And here is how you set the LabelPlacement property:</para>
        /// <code language="xaml">
        /// &lt;telerikInput:RadRangeSlider&gt;
        ///     &lt;telerikInput:RadRangeSlider.BottomRightScaleStyle&gt;
        ///         &lt;Style TargetType="telerikPrimitives:ScalePrimitive"&gt;
        ///             &lt;Setter Property="LabelPlacement" Value="BottomRight"/&gt;
        ///             &lt;Setter Property="LineStyle"&gt;
        ///                 &lt;Setter.Value&gt;
        ///                     &lt;Style TargetType="Rectangle"&gt;
        ///                         &lt;Setter Property="Stroke" Value="Red"/&gt;
        ///                     &lt;/Style&gt;
        ///                 &lt;/Setter.Value&gt;
        ///             &lt;/Setter&gt;
        ///         &lt;/Style&gt;
        ///     &lt;/telerikInput:RadRangeSlider.BottomRightScaleStyle&gt;
        /// &lt;/telerikInput:RadRangeSlider&gt;
        /// </code>
        /// </example>
        public ScaleElementPlacement LabelPlacement
        {
            get
            {
                return (ScaleElementPlacement)this.GetValue(LabelPlacementProperty);
            }
            set
            {
                this.SetValue(LabelPlacementProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the thickness of the scale ticks.
        /// </summary>
        /// <example>
        /// <para>The following namespaces have to be added to the page:</para>
        /// <code language="xaml">
        /// xmlns:telerikInput="using:Telerik.UI.Xaml.Controls.Input"
        /// xmlns:telerikPrimitives="using:Telerik.UI.Xaml.Controls.Primitives"
        /// </code>
        /// <para>And here is how you set the TickThickness property:</para>
        /// <code language="xaml">
        /// &lt;telerikInput:RadRangeSlider&gt;
        ///     &lt;telerikInput:RadRangeSlider.BottomRightScaleStyle&gt;
        ///         &lt;Style TargetType="telerikPrimitives:ScalePrimitive"&gt;
        ///             &lt;Setter Property="TickThickness" Value="5"/&gt;
        ///         &lt;/Style&gt;
        ///     &lt;/telerikInput:RadRangeSlider.BottomRightScaleStyle&gt;
        /// &lt;/telerikInput:RadRangeSlider&gt;
        /// </code>
        /// </example>
        public double TickThickness
        {
            get
            {
                return (double)this.GetValue(TickThicknessProperty);
            }
            set
            {
                this.SetValue(TickThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the length of the scale ticks.
        /// </summary>
        /// <example>
        /// <para>The following namespaces have to be added to the page:</para>
        /// <code language="xaml">
        /// xmlns:telerikInput="using:Telerik.UI.Xaml.Controls.Input"
        /// xmlns:telerikPrimitives="using:Telerik.UI.Xaml.Controls.Primitives"
        /// </code>
        /// <para>And here is how you set the TickLength property:</para>
        /// <code language="xaml">
        /// &lt;telerikInput:RadRangeSlider&gt;
        ///     &lt;telerikInput:RadRangeSlider.BottomRightScaleStyle&gt;
        ///         &lt;Style TargetType="telerikPrimitives:ScalePrimitive"&gt;
        ///             &lt;Setter Property="TickLength" Value="10"/&gt;
        ///         &lt;/Style&gt;
        ///     &lt;/telerikInput:RadRangeSlider.BottomRightScaleStyle&gt;
        /// &lt;/telerikInput:RadRangeSlider&gt;
        /// </code>
        /// </example>
        public double TickLength
        {
            get
            {
                return (double)this.GetValue(TickLengthProperty);
            }
            set
            {
                this.SetValue(TickLengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style defining the appearance of the scale ticks.
        /// </summary>
        /// <remarks>
        /// The style should have <see cref="Style.TargetType"/>="<see cref="Rectangle"/>".
        /// </remarks>
        /// <example>
        /// <para>The following namespaces have to be added to the page:</para>
        /// <code language="xaml">
        /// xmlns:telerikInput="using:Telerik.UI.Xaml.Controls.Input"
        /// xmlns:telerikPrimitives="using:Telerik.UI.Xaml.Controls.Primitives"
        /// </code>
        /// <para>And here is how you set the TickStyle property:</para>
        /// <code language="xaml">
        /// &lt;telerikInput:RadRangeSlider&gt;
        ///     &lt;telerikInput:RadRangeSlider.BottomRightScaleStyle&gt;
        ///         &lt;Style TargetType="telerikPrimitives:ScalePrimitive"&gt;
        ///             &lt;Setter Property="TickStyle"&gt;
        ///                 &lt;Setter.Value&gt;
        ///                     &lt;Style TargetType="Rectangle"&gt;
        ///                         &lt;Setter Property="Stroke" Value="Red"/&gt;
        ///                         &lt;Setter Property="StrokeThickness" Value="2"/&gt;
        ///                     &lt;/Style&gt;
        ///                 &lt;/Setter.Value&gt;
        ///             &lt;/Setter&gt;
        ///         &lt;/Style&gt;
        ///     &lt;/telerikInput:RadRangeSlider.BottomRightScaleStyle&gt;
        /// &lt;/telerikInput:RadRangeSlider&gt;
        /// </code>
        /// </example>  
        public Style TickStyle
        {
            get
            {
                return this.tickStyleCache;
            }
            set
            {
                this.SetValue(TickStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a custom <see cref="DataTemplate"/> for the ticks.
        /// </summary>
        /// <example>
        /// <para>The example demonstrates how to change the default ticks to green dots.</para>
        /// <para>The following namespaces have to be added to the page:</para>
        /// <code language="xaml">
        /// xmlns:telerikInput="using:Telerik.UI.Xaml.Controls.Input"
        /// xmlns:telerikPrimitives="using:Telerik.UI.Xaml.Controls.Primitives"
        /// </code>
        /// <para>And here is how you set the TickTemplate property:</para>
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
        public DataTemplate TickTemplate
        {
            get
            {
                return this.tickTemplateCache;
            }
            set
            {
                this.SetValue(TickTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the string that is used to format the labels of the control.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value passed should follow the syntax, expected by the <see cref="System.String.Format(String, Object[])"/> method: { index[,alignment][:formatString] }.
        /// </para>
        /// <para>
        /// When the <see cref="ScalePrimitive"/> control is used in a <see cref="T:Telerik.UI.Xaml.Controls.Input.RadRangeSlider"/> control,
        /// its <legacyBold>LabelFormat</legacyBold> property is set by the <see cref="P:Telerik.UI.Xaml.Controls.Input.RadRangeSlider.LabelFormat"/> property.
        /// </para>
        /// </remarks>
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

        /// <summary>
        /// Gets or sets the style that defines the appearance of all labels of the scale.
        /// </summary>
        /// <remarks>
        /// The Style should have <see cref="Style.TargetType"/>="<see cref="TextBlock"/>".
        /// </remarks>
        /// <example>
        /// <para>The following namespaces have to be added to the page:</para>
        /// <code language="xaml">
        /// xmlns:telerikInput="using:Telerik.UI.Xaml.Controls.Input"
        /// xmlns:telerikPrimitives="using:Telerik.UI.Xaml.Controls.Primitives"
        /// </code>
        /// <para>HSere is how you set the LabelStyle property:</para>
        /// <code language="xaml">
        /// &lt;telerikInput:RadRangeSlider&gt;
        ///     &lt;telerikInput:RadRangeSlider.BottomRightScaleStyle&gt;
        ///         &lt;Style TargetType="telerikPrimitives:ScalePrimitive"&gt;
        ///             &lt;Setter Property="LabelStyle"&gt;
        ///                 &lt;Setter.Value&gt;
        ///                     &lt;Style TargetType="TextBlock"&gt;
        ///                         &lt;Setter Property="FontSize" Value="15"/&gt;
        ///                         &lt;Setter Property="Foreground" Value="Green"/&gt;
        ///                     &lt;/Style&gt;
        ///                 &lt;/Setter.Value&gt;
        ///             &lt;/Setter&gt;
        ///         &lt;/Style&gt;
        ///     &lt;/telerikInput:RadRangeSlider.BottomRightScaleStyle&gt;
        /// &lt;/telerikInput:RadRangeSlider&gt;
        /// </code>
        /// </example>  
        public Style LabelStyle
        {
            get
            {
                return this.labelStyleCache;
            }
            set
            {
                this.SetValue(LabelStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets custom template for all labels of the scale.
        /// </summary>
        /// <example>
        /// <para>The following namespaces have to be added to the page:</para>
        /// <code language="xaml">
        /// xmlns:telerikInput="using:Telerik.UI.Xaml.Controls.Input"
        /// xmlns:telerikPrimitives="using:Telerik.UI.Xaml.Controls.Primitives"
        /// </code>
        /// <para>And here is how you set the LabelTemplate property:</para>
        /// <code language="xaml">
        /// &lt;telerikInput:RadRangeSlider&gt;
        ///     &lt;telerikInput:RadRangeSlider.BottomRightScaleStyle&gt;
        ///         &lt;Style TargetType="telerikPrimitives:ScalePrimitive"&gt;
        ///             &lt;Setter Property="LabelTemplate"&gt;
        ///                 &lt;Setter.Value&gt;
        ///                     &lt;DataTemplate&gt;
        ///                         &lt;Border BorderBrush="RoyalBlue" BorderThickness="2" CornerRadius="20" Width="40" Height="40" Margin="5"&gt;
        ///                         &lt;TextBlock Text="{Binding}" Foreground="Green" FontWeight="Black" FontSize="15" VerticalAlignment="Center" HorizontalAlignment="Center"/&gt;
        ///                         &lt;/Border&gt;
        ///                     &lt;/DataTemplate&gt;
        ///                 &lt;/Setter.Value&gt;
        ///             &lt;/Setter&gt;
        ///         &lt;/Style&gt;
        ///     &lt;/telerikInput:RadRangeSlider.BottomRightScaleStyle&gt;
        /// &lt;/telerikInput:RadRangeSlider&gt;
        /// </code>
        /// </example>
        public DataTemplate LabelTemplate
        {
            get
            {
                return this.labelTemplateCache;
            }
            set
            {
                this.SetValue(LabelTemplateProperty, value);
            }
        }

        internal Thickness AxisLineOffset
        {
            get
            {
                return this.axisLineOffsetCache;
            }
        }

        internal Rectangle Line
        {
            get
            {
                return this.line;
            }
        }

        internal override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            if (this.IsTemplateApplied)
            {
                this.panel.Model.Maximum = newMaximum;
            }
        }

        internal override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            if (this.IsTemplateApplied)
            {
                this.panel.Model.Minimum = newMinimum;
            }
        }

        /// <summary>
        /// Called in the measure layout pass to determine the desired size.
        /// </summary>
        /// <param name="availableSize">The available size that was given by the layout system.</param>
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);

            return this.panel.DesiredSize;
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="RadControl.IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.panel = this.GetTemplatePartField<ScalePanel>(PanelPartName);
            applied = applied && this.panel != null;

            this.line = this.GetTemplatePartField<Rectangle>(LinePartName);
            applied = applied && this.line != null;

            return applied;
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            this.panel.ClearVisuals();
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.panel.Owner = this;
        }

        private static void OnOrientationChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScalePrimitive presenter = target as ScalePrimitive;

            if (presenter.IsTemplateApplied)
            {
                presenter.panel.Model.Orientation = (Orientation)e.NewValue;
            }
        }

        private static void OnTickFrequencyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScalePrimitive presenter = target as ScalePrimitive;

            if (presenter.IsTemplateApplied)
            {
                presenter.panel.Model.TickFrequency = (double)e.NewValue;
            }
        }

        private static void OnTickPlacementChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScalePrimitive presenter = target as ScalePrimitive;

            if (presenter.IsTemplateApplied)
            {
                presenter.panel.Model.TickPlacement = (ScaleElementPlacement)e.NewValue;
            }
        }

        private static void OnTickThicknessChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScalePrimitive presenter = target as ScalePrimitive;

            if (presenter.IsTemplateApplied)
            {
                presenter.panel.Model.TickThickness = (double)e.NewValue;
            }
        }

        private static void OnTickLengthChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScalePrimitive presenter = target as ScalePrimitive;

            if (presenter.IsTemplateApplied)
            {
                presenter.panel.Model.TickLength = (double)e.NewValue;
            }
        }

        private static void OnLabelPlacementChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScalePrimitive presenter = target as ScalePrimitive;

            if (presenter.IsTemplateApplied)
            {
                presenter.panel.Model.LabelPlacement = (ScaleElementPlacement)e.NewValue;
            }
        }

        private static void OnTickStyleChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScalePrimitive scale = target as ScalePrimitive;
            scale.tickStyleCache = e.NewValue as Style;
        }

        private static void OnTickTemplateChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScalePrimitive presenter = target as ScalePrimitive;
            presenter.tickTemplateCache = e.NewValue as DataTemplate;
        }

        private static void OnLabelFormatChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScalePrimitive presenter = target as ScalePrimitive;

            if (presenter.IsTemplateApplied)
            {
                presenter.panel.Model.LabelFormat = (string)e.NewValue;
            }
        }

        private static void OnLabelStyleChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScalePrimitive presenter = target as ScalePrimitive;
            presenter.labelStyleCache = e.NewValue as Style;
        }

        private static void OnLabelTemplateChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScalePrimitive presenter = target as ScalePrimitive;
            presenter.labelTemplateCache = e.NewValue as DataTemplate;
        }

        private static void OnAxisLineOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScalePrimitive scale = target as ScalePrimitive;
            if (scale.IsInternalPropertyChange)
            {
                scale.axisLineOffsetCache = (Thickness)e.NewValue;
            }
            else
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, "{0} is a read-only property.", AxisLineOffsetPropertyName));
            }
        }
    }
}
