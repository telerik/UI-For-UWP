using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Charting;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    public abstract partial class PointTemplateSeries
    {
        /// <summary>
        /// Identifies the <see cref="LegendTitle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendTitleProperty =
            DependencyProperty.Register(nameof(LegendTitle), typeof(string), typeof(PointTemplateSeries), new PropertyMetadata(null, OnLegendTitleChanged));

        /// <summary>
        /// Identifies the <see cref="IsVisibleInLegend"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsVisibleInLegendProperty =
            DependencyProperty.Register(nameof(IsVisibleInLegend), typeof(bool), typeof(PointTemplateSeries), new PropertyMetadata(true, OnIsVisibleInLegendChanged));

        private List<LegendItem> legendItems = new List<LegendItem> { new LegendItem() };

        /// <summary>
        /// Gets or sets the title that will be used by <see cref="RadLegendControl"/> to display chart legend.
        /// </summary>
        public string LegendTitle
        {
            get { return (string)GetValue(LegendTitleProperty); }
            set { SetValue(LegendTitleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this series will be  used in <see cref="RadLegendControl"/> to display chart legend.
        /// </summary>
        public bool IsVisibleInLegend
        {
            get { return (bool)GetValue(IsVisibleInLegendProperty); }
            set { SetValue(IsVisibleInLegendProperty, value); }
        }

        internal virtual IEnumerable<FrameworkElement> RealizedDefaultVisualElements 
        {
            get
            {
                return this.realizedDataPointPresenters.Where(v => this.IsDefaultVisual(v) && v.Visibility == Visibility.Visible);
            }
        }

        internal virtual IEnumerable<LegendItem> LegendItems
        {
            get
            {
                return this.legendItems;
            }
        }

        internal static void UpdateLegendItemProperties(LegendItem item, Brush fill, Brush stroke)
        {
            if (item != null)
            {
                UpdateLegendFill(item, fill);
                UpdateLegendStroke(item, stroke);
            }
        }

        internal override void SetDynamicLegendTitle(string titlePath, string extractedValue)
        {
            base.SetDynamicLegendTitle(titlePath, extractedValue);

            this.LegendTitle = extractedValue;
        }

        internal void UpdateLegendItemProperties(Brush fill, Brush stroke)
        {
            UpdateLegendItemProperties(this.legendItems.FirstOrDefault(), fill, stroke);
        }

        internal virtual void UpdateLegendItems()
        {
            foreach (FrameworkElement element in this.RealizedDefaultVisualElements)
            {
                if (element != null)
                {
                    this.UpdateLegendItem(element, element.Tag as DataPoint);
                }
            }
        }

        internal abstract void UpdateLegendItem(FrameworkElement visual, DataPoint dataPoint);

        internal virtual void OnLegendTitleChanged(string oldValue, string newValue)
        {
            if (this.LegendItems.Count() == 1)
            {
                this.legendItems[0].Title = newValue;
            }
        }

        private static void UpdateLegendFill(LegendItem item, Brush fill)
        {
            item.Fill = fill;
        }

        private static void UpdateLegendStroke(LegendItem item, Brush stroke)
        {
            item.Stroke = stroke;

            if (item.Fill == null)
            {
                item.Fill = item.Stroke;
            }
        }

        private static void OnLegendTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = (PointTemplateSeries)d;

            series.OnLegendTitleChanged(e.OldValue as string, e.NewValue as string);
        }

        private static void OnIsVisibleInLegendChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = (PointTemplateSeries)d;

            if (series.Chart != null)
            {
                if ((bool)e.NewValue)
                {
                    foreach (var item in series.LegendItems)
                    {
                        series.Chart.LegendInfosInternal.Add(item);
                    }
                }
                else
                {
                    foreach (var item in series.LegendItems)
                    {
                        series.Chart.LegendInfosInternal.Remove(item);
                    }
                }
            }
        }
    }
}
