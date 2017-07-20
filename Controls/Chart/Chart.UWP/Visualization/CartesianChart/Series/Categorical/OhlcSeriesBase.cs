using System.Collections.Generic;
using System.Linq;
using Telerik.Charting;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// A base class for chart series that plot financial indicators using High, Low, Open, Close values.
    /// </summary>
    [ContentProperty(Name = "DataPoints")]
    public abstract class OhlcSeriesBase : CartesianSeries
    {
        /// <summary>
        /// Identifies the <see cref="HighBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty HighBindingProperty =
            DependencyProperty.Register(nameof(HighBinding), typeof(DataPointBinding), typeof(OhlcSeriesBase), new PropertyMetadata(null, OnHighBindingChanged));

        /// <summary>
        /// Identifies the <see cref="LowBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty LowBindingProperty =
            DependencyProperty.Register(nameof(LowBinding), typeof(DataPointBinding), typeof(OhlcSeriesBase), new PropertyMetadata(null, OnLowBindingChanged));

        /// <summary>
        /// Identifies the <see cref="OpenBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty OpenBindingProperty =
            DependencyProperty.Register(nameof(OpenBinding), typeof(DataPointBinding), typeof(OhlcSeriesBase), new PropertyMetadata(null, OnOpenBindingChanged));

        /// <summary>
        /// Identifies the <see cref="CloseBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty CloseBindingProperty =
            DependencyProperty.Register(nameof(CloseBinding), typeof(DataPointBinding), typeof(OhlcSeriesBase), new PropertyMetadata(null, OnCloseBindingChanged));

        /// <summary>
        /// Identifies the <see cref="CategoryBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty CategoryBindingProperty =
            DependencyProperty.Register(nameof(CategoryBinding), typeof(DataPointBinding), typeof(OhlcSeriesBase), new PropertyMetadata(null, OnCategoryBindingChanged));

        private OhlcSeriesModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="OhlcSeriesBase"/> class.
        /// </summary>
        protected OhlcSeriesBase()
        {
            this.model = new OhlcSeriesModel();
        }

        /// <summary>
        /// Gets the collection of data points associated with the series.
        /// </summary>
        public DataPointCollection<OhlcDataPoint> DataPoints
        {
            get
            {
                return this.model.DataPoints;
            }
        }

        /// <summary>
        /// Gets or sets the binding that will be used to fill the <see cref="CategoricalDataPointBase.Category"/> member of the contained data points.
        /// </summary>
        public DataPointBinding CategoryBinding
        {
            get
            {
                return (DataPointBinding)this.GetValue(CategoryBindingProperty);
            }
            set
            {
                this.SetValue(CategoryBindingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the binding that will be used to fill the <see cref="OhlcDataPoint.High"/> member of the contained data points.
        /// </summary>
        public DataPointBinding HighBinding
        {
            get
            {
                return (DataPointBinding)this.GetValue(HighBindingProperty);
            }
            set
            {
                this.SetValue(HighBindingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the binding that will be used to fill the <see cref="OhlcDataPoint.Low"/> member of the contained data points.
        /// </summary>
        public DataPointBinding LowBinding
        {
            get
            {
                return (DataPointBinding)this.GetValue(LowBindingProperty);
            }
            set
            {
                this.SetValue(LowBindingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the binding that will be used to fill the <see cref="OhlcDataPoint.Open"/> member of the contained data points.
        /// </summary>
        public DataPointBinding OpenBinding
        {
            get
            {
                return (DataPointBinding)this.GetValue(OpenBindingProperty);
            }
            set
            {
                this.SetValue(OpenBindingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the binding that will be used to fill the <see cref="OhlcDataPoint.Close"/> member of the contained data points.
        /// </summary>
        public DataPointBinding CloseBinding
        {
            get
            {
                return (DataPointBinding)this.GetValue(CloseBindingProperty);
            }
            set
            {
                this.SetValue(CloseBindingProperty, value);
            }
        }

        internal override ChartSeriesModel Model
        {
            get
            {
                return this.model;
            }
        }

        internal override string Family
        {
            get
            {
                return ChartPalette.OhlcFamily;
            }
        }

        internal override bool SupportsDefaultVisuals
        {
            get
            {
                return true;
            }
        }

        internal override IEnumerable<LegendItem> LegendItems
        {
            get
            {
                return Enumerable.Empty<LegendItem>();
            }
        }

        internal override void ArrangeUIElement(FrameworkElement presenter, RadRect layoutSlot, bool setSize = true)
        {
            base.ArrangeUIElement(presenter, layoutSlot, false);

            var ohlcElement = presenter as OhlcShape;
            if (ohlcElement != null)
            {
                ohlcElement.UpdateGeometry();
                ohlcElement.UpdateElementAppearance();
            }
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new OhlcSeriesDataSource();
        }

        internal override void SetDefaultVisualContent(FrameworkElement visual, DataPoint point)
        {
            (visual as OhlcShape).dataPoint = point as OhlcDataPoint;
        }

        internal override void ApplyPaletteToDefaultVisual(FrameworkElement visual, DataPoint point)
        {
            int index = this.ActualPaletteIndex;

            Brush paletteStroke = this.chart.GetPaletteBrush(index, PaletteVisualPart.Stroke, this.Family, point.isSelected);
            Brush paletteSpecialStroke = this.chart.GetPaletteBrush(index, PaletteVisualPart.SpecialStroke, this.Family, point.isSelected);
            if (paletteSpecialStroke == null)
            {
                paletteSpecialStroke = paletteStroke;
            }

            if (paletteStroke != null)
            {
                visual.SetValue(OhlcShape.UpStrokeProperty, paletteStroke);
            }
            else
            {
                visual.ClearValue(OhlcStick.UpStrokeProperty);
            }

            if (paletteSpecialStroke != null)
            {
                visual.SetValue(OhlcShape.DownStrokeProperty, paletteSpecialStroke);
            }
            else
            {
                visual.ClearValue(OhlcStick.DownStrokeProperty);
            }

            (visual as OhlcShape).UpdateElementAppearance();
        }

        internal override void ApplyPaletteToContainerVisual(SpriteVisual visual, DataPoint point)
        {
            int index = this.ActualPaletteIndex;
            var ohlcDataPoint = point as OhlcDataPoint;
            Brush paletteStroke = this.chart.GetPaletteBrush(index, PaletteVisualPart.Stroke, this.Family, point.isSelected);
            Brush specialPaletteStroke = this.chart.GetPaletteBrush(index, PaletteVisualPart.SpecialStroke, this.Family, point.isSelected);

            if (paletteStroke != null)
            {
                foreach (var child in visual.Children)
                {
                    var childVisual = child as SpriteVisual;
                    if (childVisual != null)
                    {
                        if (ohlcDataPoint != null && ohlcDataPoint.IsFalling && specialPaletteStroke != null)
                        {
                            this.chart.ContainerVisualsFactory.SetCompositionColorBrush(childVisual, specialPaletteStroke, true);
                        }
                        else
                        {
                            this.chart.ContainerVisualsFactory.SetCompositionColorBrush(childVisual, paletteStroke, true);
                        }
                    }
                }
            }
            else
            {
                foreach (var child in visual.Children)
                {
                    var childVisual = child as SpriteVisual;
                    if (childVisual != null)
                    {
                        this.chart.ContainerVisualsFactory.SetCompositionColorBrush(childVisual, null, true);
                    }
                }
            }

            base.ApplyPaletteToContainerVisual(visual, point);
        }

        internal override bool IsDefaultVisual(FrameworkElement visual)
        {
            return visual is OhlcShape;
        }

        internal override void SetDefaultVisualStyle(FrameworkElement visual)
        {
            base.SetDefaultVisualStyle(visual);

            (visual as OhlcShape).UpdateElementAppearance();
        }

        internal override void UpdateLegendItem(FrameworkElement visual, DataPoint dataPoint)
        {
            // Implement this when the legend start to makes sense for this series.
        }

        private static void OnCategoryBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OhlcSeriesBase presenter = d as OhlcSeriesBase;
            (presenter.dataSource as CategoricalSeriesDataSourceBase).CategoryBinding = e.NewValue as DataPointBinding;
        }

        private static void OnHighBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OhlcSeriesBase presenter = d as OhlcSeriesBase;
            (presenter.dataSource as OhlcSeriesDataSource).HighBinding = e.NewValue as DataPointBinding;
        }

        private static void OnLowBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OhlcSeriesBase presenter = d as OhlcSeriesBase;
            (presenter.dataSource as OhlcSeriesDataSource).LowBinding = e.NewValue as DataPointBinding;
        }

        private static void OnOpenBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OhlcSeriesBase presenter = d as OhlcSeriesBase;
            (presenter.dataSource as OhlcSeriesDataSource).OpenBinding = e.NewValue as DataPointBinding;
        }

        private static void OnCloseBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OhlcSeriesBase presenter = d as OhlcSeriesBase;
            (presenter.dataSource as OhlcSeriesDataSource).CloseBinding = e.NewValue as DataPointBinding;
        }
    }
}
