using System;
using System.Diagnostics.CodeAnalysis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents <see cref="PolarLineSeries"/> that may optionally fill the area, enclosed by all points.
    /// </summary>
    public class PolarAreaSeries : PolarLineSeries, IFilledSeries
    {
        /// <summary>
        /// Identifies the <see cref="Fill"/> property.
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(PolarAreaSeries), new PropertyMetadata(null, OnFillChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="PolarAreaSeries"/> class.
        /// </summary>
        public PolarAreaSeries()
        {
            this.DefaultStyleKey = typeof(PolarAreaSeries);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Fill"/> property has been set locally.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        bool IFilledSeries.IsFillSetLocally
        {
            get
            {
                return this.ReadLocalValue(FillProperty) is Brush;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> that defines the interior of the area.
        /// </summary>
        public Brush Fill
        {
            get
            {
                return this.GetValue(FillProperty) as Brush;
            }
            set
            {
                this.SetValue(FillProperty, value);
            }
        }

        /// <summary>
        /// Gets the human-readable family of this instance. For example Area is the family for Area, SplineArea and PolarArea series.
        /// </summary>
        internal override string Family
        {
            get
            {
                return ChartPalette.AreaFamily;
            }
        }

        private static void OnFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolarAreaSeries series = d as PolarAreaSeries;
            series.renderer.strokeShape.Fill = e.NewValue as Brush;

            if (series.isPaletteApplied)
            {
                series.UpdatePalette(true);
            }
        }
    }
}
