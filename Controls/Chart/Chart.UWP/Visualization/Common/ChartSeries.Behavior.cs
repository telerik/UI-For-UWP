using System;
using System.Collections.Generic;
using Telerik.Charting;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.Chart
{
    public partial class ChartSeries
    {
        /// <summary>
        /// Identifies the <see cref="IsSelected"/> property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(ChartSeries), new PropertyMetadata(false, OnIsSelectedChanged));

        /// <summary>
        /// Identifies the <see cref="AllowSelect"/> property.
        /// </summary>
        public static readonly DependencyProperty AllowSelectProperty =
            DependencyProperty.Register(nameof(AllowSelect), typeof(bool), typeof(ChartSeries), new PropertyMetadata(false, OnAllowSelectChanged));

        private bool isSelectedCache;
        private bool allowSelectCache;
        private bool setPropertySilently;

        /// <summary>
        /// Gets or sets a value indicating whether the series is currently in a "Selected" state. Usually this state is indicated by a change in the visual representation of the series.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.isSelectedCache;
            }
            set
            {
                this.SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the series might enter the IsSelected state.
        /// </summary>
        public bool AllowSelect
        {
            get
            {
                return this.allowSelectCache;
            }
            set
            {
                this.SetValue(AllowSelectProperty, value);
            }
        }

        /// <summary>
        /// Gets the UI that represents the series itself (not the data points).
        /// </summary>
        internal virtual FrameworkElement SeriesVisual
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Determines whether the provided touch rect is within the series visual representation.
        /// </summary>
        public virtual bool HitTest(Rect touchRect)
        {
            if (!this.IsTemplateApplied || this.SeriesVisual == null)
            {
                return false;
            }

            foreach (var child in this.HitTestElementsCore(touchRect, false, false))
            {
                if (child == this.SeriesVisual)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Finds all the data points which visual representations contain the specified touch rect.
        /// </summary>
        public IEnumerable<DataPoint> HitTestDataPoints(Rect touchRect)
        {
            return this.HitTestDataPoints(touchRect, false);
        }

        internal IEnumerable<DataPoint> HitTestDataPoints(Rect touchRect, bool addAlldataPoints)
        {
            if (this.chart == null || !this.IsLoaded)
            {
                yield break;
            }

            foreach (DataPoint point in this.HitTestDataPointsCore(touchRect, addAlldataPoints))
            {
                yield return point;
            }
        }

        /// <summary>
        /// Performs the core logic behind the HitTestElements method.
        /// </summary>
        protected virtual IEnumerable<FrameworkElement> HitTestElementsCore(Rect touchRect, bool dataPointsOnly, bool includeAllElements)
        {
            if (this.chart == null || !this.IsLoaded)
            {
                yield break;
            }

            // NOTE: All hit tests with VisualTreeHelper.FindElementsInHostCoordinates(intersectingRect, subtree, includeAllElements) 
            // are done in the global coordinate system of the application and not in the coordinate system of the subtree passed as parameter.
            if (Window.Current != null && Window.Current.Content != null)
            {
                touchRect = this.Chart.TransformToVisual(Window.Current.Content).TransformBounds(touchRect);
            }

            foreach (var element in this.GetElements(touchRect, this.renderSurface, includeAllElements, dataPointsOnly))
            {
                yield return element;
            }
        }

        /// <summary>
        /// Returns an array of rectangle <see cref="Windows.Foundation.Point(double, double)"/>.
        /// </summary>
        protected virtual Point[] SelectRectPoints(ref Rect touchRect)
        {
            var points = new Point[] 
            {
                new Point((touchRect.Left + touchRect.Right) / 2, (touchRect.Top + touchRect.Bottom) / 2)
            };
            return points;
        }

        /// <summary>
        /// Performs the core logic behind the HitTestDataPoints method.
        /// </summary>
        /// <param name="touchRect">The touch rectangle.</param>
        protected IEnumerable<DataPoint> HitTestDataPointsCore(Rect touchRect)
        {
            return this.HitTestDataPointsCore(touchRect, false);
        }

        /// <summary>
        /// Performs the core logic behind the HitTestDataPoints method.
        /// </summary>
        /// <param name="touchRect">The touch rectangle.</param>
        /// <param name="includeAllDataPoints">True to return all data points in the touch rectangle.</param>
        protected virtual IEnumerable<DataPoint> HitTestDataPointsCore(Rect touchRect, bool includeAllDataPoints)
        {
            foreach (FrameworkElement element in this.HitTestElementsCore(touchRect, true, includeAllDataPoints))
            {
                yield return element.Tag as DataPoint;
            }
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            series.isSelectedCache = (bool)e.NewValue;

            if (series.setPropertySilently)
            {
                return;
            }

            bool refresh = true;
            if (!series.allowSelectCache && series.isSelectedCache)
            {
                refresh = false;
                series.SetPropertySilently(IsSelectedProperty, false);
            }

            if (refresh)
            {
                series.InvalidatePalette();
            }
        }

        private static void OnAllowSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeries series = d as ChartSeries;
            series.allowSelectCache = (bool)e.NewValue;

            if (series.isSelectedCache)
            {
                series.IsSelected = false;
            }
        }

        private static FrameworkElement DecideReturnElement(bool dataPointsOnly, FrameworkElement frameworkElement)
        {
            if (!dataPointsOnly || frameworkElement.Tag is DataPoint)
            {
                return frameworkElement;
            }
            return null;
        }

        private IEnumerable<FrameworkElement> GetElements(Rect touchRect, UIElement subtree, bool includeAllElements, bool dataPointsOnly)
        {
            var points = this.SelectRectPoints(ref touchRect);

            foreach (var point in points)
            {
                foreach (UIElement element in VisualTreeHelper.FindElementsInHostCoordinates(point, this.renderSurface, includeAllElements))
                {
                    FrameworkElement frameworkElement = element as FrameworkElement;
                    if (frameworkElement == null)
                    {
                        continue;
                    }

                    FrameworkElement result = DecideReturnElement(dataPointsOnly, frameworkElement);
                    if (result != null)
                    {
                        yield return result;
                    }
                }
            }
        }
        
        private void SetPropertySilently(DependencyProperty property, object value)
        {
            this.setPropertySilently = true;
            this.SetValue(property, value);
            this.setPropertySilently = false;
        }
    }
}
