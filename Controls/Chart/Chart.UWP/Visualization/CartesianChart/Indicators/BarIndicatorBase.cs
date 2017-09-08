using System;
using System.Collections.Generic;
using Telerik.Charting;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a financial indicator, whose value depends on the values of DataPoints in financial series.
    /// </summary>
    public abstract class BarIndicatorBase : IndicatorBase
    {
        /// <summary>
        /// Identifies the <see cref="ValueBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty ValueBindingProperty =
            DependencyProperty.Register(nameof(ValueBinding), typeof(DataPointBinding), typeof(BarIndicatorBase), new PropertyMetadata(null, OnValueBindingChanged));

        /// <summary>
        /// Identifies the <see cref="DefaultVisualStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty DefaultVisualStyleProperty = 
            DependencyProperty.Register(nameof(DefaultVisualStyle), typeof(Style), typeof(BarIndicatorBase), new PropertyMetadata(null, OnDefaultVisualStyleChanged));

        internal Style defaultVisualStyleCache;
        internal List<FrameworkElement> realizedDataPoints;
        internal List<ContainerVisual> realizedVisualDataPoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarIndicatorBase" /> class.
        /// </summary>
        internal BarIndicatorBase()
        {
            this.DefaultStyleKey = typeof(BarIndicatorBase);

            this.renderSurface = new Canvas();
            this.realizedDataPoints = new List<FrameworkElement>();
            this.realizedVisualDataPoints = new List<ContainerVisual>();
        }

        /// <summary>
        /// Gets or sets the binding that will be used to fill the <see cref="SingleValueDataPoint.Value"/> member of the contained data points.
        /// </summary>
        public DataPointBinding ValueBinding
        {
            get
            {
                return this.GetValue(ValueBindingProperty) as DataPointBinding;
            }
            set
            {
                this.SetValue(ValueBindingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> instance that will define the appearance of series' default visuals (if any).
        /// For example a BarSeries will create <see cref="Border"/> instances as its default visuals.
        /// Point templates (if specified) however have higher precedence compared to the default visuals.
        /// </summary>
        public Style DefaultVisualStyle
        {
            get
            {
                return this.defaultVisualStyleCache;
            }
            set
            {
                this.SetValue(DefaultVisualStyleProperty, value);
            }
        }

        internal override string Family
        {
            get { throw new System.NotSupportedException(); }
        }

        internal static bool IsDefaultVisual(FrameworkElement visual)
        {
            return visual is Border;
        }

        private static void OnValueBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BarIndicatorBase presenter = d as BarIndicatorBase;
            (presenter.dataSource as CategoricalSeriesDataSource).ValueBinding = e.NewValue as DataPointBinding;
        }

        private static void OnDefaultVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BarIndicatorBase presenter = d as BarIndicatorBase;
            presenter.defaultVisualStyleCache = e.NewValue as Style;
            presenter.UpdateDefaultVisualsStyle();
        }

        private void UpdateDefaultVisualsStyle()
        {
            foreach (FrameworkElement visual in this.realizedDataPoints)
            {
                if (IsDefaultVisual(visual))
                {
                    visual.Style = this.defaultVisualStyleCache;
                }
            }

            this.UpdatePalette(true);
        }
    }
}
