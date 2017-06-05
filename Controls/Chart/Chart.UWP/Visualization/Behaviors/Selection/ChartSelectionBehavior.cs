using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Charting;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a behavior that changes the IsSelected state of data points and series depending on user input.
    /// </summary>
    public class ChartSelectionBehavior : ChartBehavior
    {
        private ChartSelectionMode seriesSelectionMode;
        private ChartSelectionMode dataPointSelectionMode;
        private Thickness touchTargetOverhang;
        private Rect touchRect;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSelectionBehavior"/> class.
        /// </summary>
        public ChartSelectionBehavior()
        {
            this.touchTargetOverhang = new Thickness(0);
            this.seriesSelectionMode = ChartSelectionMode.None;
            this.dataPointSelectionMode = ChartSelectionMode.Single;
        }

        /// <summary>
        /// Occurs when the IsSelected property of a <see cref="ChartSeries"/> and/or <see cref="DataPoint"/> instance changes.
        /// </summary>
        public event EventHandler SelectionChanged;

        /// <summary>
        /// Gets or sets the <see cref="ChartSelectionMode"/> that controls the selection behavior of the chart series.
        /// </summary>
        public ChartSelectionMode SeriesSelectionMode
        {
            get
            {
                return this.seriesSelectionMode;
            }
            set
            {
                this.seriesSelectionMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ChartSelectionMode"/> that controls the selection behavior of the data points within the chart series.
        /// </summary>
        public ChartSelectionMode DataPointSelectionMode
        {
            get
            {
                return this.dataPointSelectionMode;
            }
            set
            {
                this.dataPointSelectionMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref=" Windows.UI.Xaml.Thickness(double)"/> struct that will be used when calculating the touch rect.
        /// </summary>
        public Thickness TouchTargetOverhang
        {
            get
            {
                return this.touchTargetOverhang;
            }
            set
            {
                this.touchTargetOverhang = value;
            }
        }

        /// <summary>
        /// Gets all the points from all series within the chart plot area that are currently selected.
        /// </summary>
        public IEnumerable<DataPoint> SelectedPoints
        {
            get
            {
                if (this.chart == null)
                {
                    yield break;
                }

                foreach (ChartSeriesModel series in this.chart.chartArea.Series)
                {
                    if (!series.presenter.IsVisible)
                    {
                        continue;
                    }

                    foreach (DataPoint point in series.DataPointsInternal)
                    {
                        if (point.isSelected)
                        {
                            yield return point;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets all the <see cref="ChartSeries"/> instances within the plot area that are currently selected.
        /// </summary>
        public IEnumerable<ChartSeries> SelectedSeries
        {
            get
            {
                if (this.chart == null)
                {
                    yield break;
                }

                foreach (ChartSeries series in this.chart.SeriesInternal)
                {
                    if (series.IsSelected)
                    {
                        yield return series;
                    }
                }
            }
        }

        /// <summary>
        /// Removes the current selection within the chart.
        /// </summary>
        /// <param name="dataPoints">True to clear the selected state of each data point, false otherwise.</param>
        /// <param name="chartSeries">True to clear the selected state of each chart series, false otherwise.</param>
        public void ClearSelection(bool dataPoints = true, bool chartSeries = true)
        {
            if (this.chart == null)
            {
                return;
            }

            foreach (ChartSeries series in this.chart.SeriesInternal)
            {
                if (chartSeries)
                {
                    series.IsSelected = false;
                }

                if (dataPoints)
                {
                    foreach (DataPoint point in series.Model.DataPointsInternal)
                    {
                        if (point.isSelected)
                        {
                            point.IsSelected = false;
                        }
                    }
                }
            }
        }

        internal bool HandleTap(Point position)
        {
            this.touchRect = new Rect(
                position.X - this.touchTargetOverhang.Left,
                position.Y - this.touchTargetOverhang.Top,
                this.touchTargetOverhang.Left + this.touchTargetOverhang.Right,
                this.touchTargetOverhang.Top + this.touchTargetOverhang.Bottom);

            bool handled = this.UpdateSelection();
            if (handled)
            {
                this.RaiseSelectionChanged();
            }

            return handled;
        }

        /// <summary>
        /// Performs the core selection logic.
        /// </summary>
        protected internal override void OnTapped(TappedRoutedEventArgs args)
        {
            base.OnTapped(args);

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            Point position = args.GetPosition(this.chart);
            args.Handled = this.HandleTap(position);
        }

        private bool UpdateSelection()
        {
            bool handled = false;

            if (this.UpdateSeriesSelection())
            {
                handled = true;
            }

            foreach (ChartSeries series in this.chart.SeriesInternal)
            {
                if (this.UpdateDataPointSelection(series))
                {
                    handled = true;
                }
            }

            return handled;
        }

        private bool UpdateSeriesSelection()
        {
            bool changed = false;
            bool wasSelected = false;
            bool seriesHit = false;

            foreach (ChartSeries series in this.chart.SeriesInternal.Cast<ChartSeries>().Reverse())
            {
                wasSelected = series.IsSelected;
                if (!series.AllowSelect || series.Visibility == Visibility.Collapsed)
                {
                    series.IsSelected = false;
                }
                else
                {
                    switch (this.seriesSelectionMode)
                    {
                        case ChartSelectionMode.None:
                            series.IsSelected = false;
                            break;
                        case ChartSelectionMode.Single:
                            if (series.HitTest(this.touchRect) && !seriesHit)
                            {
                                series.IsSelected ^= true;
                                seriesHit = true;
                            }
                            else
                            {
                                series.IsSelected = false;
                            }
                            break;
                        case ChartSelectionMode.Multiple:
                            if (series.HitTest(this.touchRect) && !seriesHit)
                            {
                                series.IsSelected ^= true;
                                seriesHit = true;
                            }
                            break;
                    }
                    if (wasSelected != series.IsSelected)
                    {
                        this.UpdateDataPointsSelectionForSeries(series);
                    }
                }

                changed = changed || wasSelected != series.IsSelected; 
            }

            return changed;
        }

        private void UpdateDataPointsSelectionForSeries(ChartSeries series)
        {
            foreach (DataPoint dataPoint in series.Model.DataPointsInternal)
            {
                dataPoint.IsSelected = series.IsSelected;
            }
        }

        private bool UpdateDataPointSelection(ChartSeries series)
        {
            if (series.Visibility == Visibility.Collapsed || this.seriesSelectionMode != ChartSelectionMode.None)
            {
                return false;
            }

            if (series.IsSelected)
            {
                foreach (DataPoint dataPoint in series.Model.DataPointsInternal)
                {
                    dataPoint.isSelected = true;
                }
                return true;
            }

            bool handled = false;

            if (this.dataPointSelectionMode == ChartSelectionMode.Multiple)
            {
                foreach (DataPoint point in series.HitTestDataPoints(this.touchRect))
                {
                    point.IsSelected ^= true;
                    handled = true;
                }

                return handled;
            }

            // clear selection first
            foreach (DataPoint point in series.Model.DataPointsInternal)
            {
                handled = point.isSelected;
                point.wasSelected = handled;

                if (handled)
                {
                    point.IsSelected = false;
                }
            }

            if (this.dataPointSelectionMode == ChartSelectionMode.Single)
            {
                foreach (DataPoint point in series.HitTestDataPoints(this.touchRect))
                {
                    point.IsSelected = !point.wasSelected;
                    handled = true;
                    break;
                }
            }

            return handled;
        }

        private void RaiseSelectionChanged()
        {
            EventHandler eh = this.SelectionChanged;
            if (eh != null)
            {
                eh(this, EventArgs.Empty);
            }
        }
    }
}
