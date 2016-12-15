using System;
using System.Collections.Generic;
using System.Diagnostics;
using Telerik.Core;

namespace Telerik.Charting
{
    /// <summary>
    /// Represents a chart area that needs two <see cref="AxisModel"/> instances to plot its points.
    /// Such chart areas are the <see cref="CartesianChartAreaModel"/> and <see cref="PolarChartAreaModel"/>.
    /// </summary>
    internal abstract class ChartAreaModelWithAxes : ChartAreaModel
    {
        internal readonly List<AxisModel> FirstAxes;
        internal readonly List<AxisModel> SecondAxes;
        internal readonly ReferenceDictionary<AxisModel, ChartSeriesCombineStrategy> SeriesCombineStrategies;
        internal readonly List<ChartAnnotationModel> Annotations;
        internal AxisModel primaryFirstAxis;
        internal AxisModel primarySecondAxis;
        internal ChartGridModel grid;
        internal bool axesUpdateNeeded;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAreaModelWithAxes"/> class.
        /// </summary>
        public ChartAreaModelWithAxes()
        {
            this.SeriesCombineStrategies = new ReferenceDictionary<AxisModel, ChartSeriesCombineStrategy>();
            this.FirstAxes = new List<AxisModel>();
            this.SecondAxes = new List<AxisModel>();
            this.Annotations = new List<ChartAnnotationModel>();

            this.axesUpdateNeeded = true;
        }

        /// <summary>
        /// Determines whether the plot area is fully loaded and may be visualized.
        /// </summary>
        public override bool IsTreeLoaded
        {
            get
            {
                return base.IsTreeLoaded && this.FirstAxes.Count > 0 && this.SecondAxes.Count > 0;
            }
        }

        internal abstract Tuple<object, object> ConvertPointToData(RadPoint coordinates);

        internal abstract RadPoint ConvertDataToPoint(Tuple<object, object> data);

        internal void SetAxis(AxisModel axis, AxisType type)
        {
            Debug.Assert(axis != null, "axis should not be null!");
            if (type == AxisType.First)
            {
                if (this.FirstAxes.Contains(axis))
                {
                    return;
                }
                this.FirstAxes.Add(axis);
                if (axis.isPrimary)
                {
                    this.primaryFirstAxis = axis;
                }
            }
            else
            {
                if (this.SecondAxes.Contains(axis))
                {
                    return;
                }
                this.SecondAxes.Add(axis);
                if (axis.isPrimary)
                {
                    this.primarySecondAxis = axis;
                }
            }

            axis.Type = type;
            this.AttachAxis(axis);
        }

        internal void RemoveAxis(AxisModel axis)
        {
            if (axis.type == AxisType.First)
            {
                this.FirstAxes.Remove(axis);
                if (axis.isPrimary)
                {
                    this.primaryFirstAxis = null;
                }
            }
            else
            {
                this.SecondAxes.Remove(axis);
                if (axis.isPrimary)
                {
                    this.primarySecondAxis = null;
                }
            }

            this.SeriesCombineStrategies.Remove(axis);
            this.DetachAxis(axis);
        }

        internal void SetGrid(ChartGridModel gridModel)
        {
            if (this.grid == gridModel)
            {
                return;
            }

            if (this.grid != null)
            {
                this.children.Remove(this.grid);
            }

            this.grid = gridModel;

            if (this.grid != null)
            {
                this.children.Add(this.grid);
            }

            this.Invalidate(ChartAreaInvalidateFlags.All);
        }

        internal void AddAnnotation(ChartAnnotationModel annotation)
        {
            this.Annotations.Add(annotation);

            this.children.Add(annotation);
        }

        internal void RemoveAnnotation(ChartAnnotationModel annotation)
        {
            this.Annotations.Remove(annotation);

            this.children.Remove(annotation);
        }

        internal override void InvalidateCore(ChartAreaInvalidateFlags flags)
        {
            if ((flags & ChartAreaInvalidateFlags.ResetAxes) == ChartAreaInvalidateFlags.ResetAxes)
            {
                foreach (var axis in this.FirstAxes)
                {
                    axis.ResetState();
                }
                foreach (var axis in this.SecondAxes)
                {
                    axis.ResetState();
                }
                this.SeriesCombineStrategies.Clear();
                this.axesUpdateNeeded = true;
            }
            if ((flags & ChartAreaInvalidateFlags.InvalidateAxes) == ChartAreaInvalidateFlags.InvalidateAxes)
            {
                foreach (var axis in this.FirstAxes)
                {
                    axis.Invalidate();
                }
                foreach (var axis in this.SecondAxes)
                {
                    axis.Invalidate();
                }
            }
            if ((flags & ChartAreaInvalidateFlags.InvalidateGrid) == ChartAreaInvalidateFlags.InvalidateGrid)
            {
                if (this.grid != null)
                {
                    this.grid.Invalidate();
                }
            }
            if ((flags & ChartAreaInvalidateFlags.ResetAnnotations) == ChartAreaInvalidateFlags.ResetAnnotations)
            {
                foreach (var annotation in this.Annotations)
                {
                    annotation.ResetState();
                }
            }
            if ((flags & ChartAreaInvalidateFlags.InvalidateAnnotations) == ChartAreaInvalidateFlags.InvalidateAnnotations)
            {
                foreach (var annotation in this.Annotations)
                {
                    annotation.Invalidate();
                }
            }

            base.InvalidateCore(flags);
        }

        internal override ModifyChildrenResult CanAddChild(Node child)
        {
            var axisModelChild = child as AxisModel;
            if (child == this.plotArea || child == this.grid || child is ChartAnnotationModel ||
                (axisModelChild != null && (this.FirstAxes.Contains(axisModelChild) || this.SecondAxes.Contains(axisModelChild))))
            {
                return ModifyChildrenResult.Accept;
            }

            return base.CanAddChild(child);
        }

        internal override RadRect ArrangeOverride(RadRect rect)
        {
            this.BeginUpdate();

            if (this.axesUpdateNeeded)
            {
                this.UpdateAxes();
                this.axesUpdateNeeded = false;
            }

            RadRect seriesRect = this.ArrangeAxes(rect);

            this.plotArea.Arrange(seriesRect);

            this.ApplyLayoutRounding();

            if (this.view.ZoomWidth > 1 || this.view.ZoomHeight > 1)
            {
                // update ticks and labels visibility
                // this is done after the axes and plot area are arranged so that all the layout information is available
                RadRect clipRect = this.view.PlotAreaClip;
                foreach (var axis in this.FirstAxes)
                {
                    axis.UpdateTicksVisibility(clipRect);
                }
                foreach (var axis in this.SecondAxes)
                {
                    axis.UpdateTicksVisibility(clipRect);
                }
            }

            // arrange the grid within the series rect
            if (this.grid != null)
            {
                this.grid.Arrange(seriesRect);
            }

            // arrange the annotations within the series rect
            foreach (ChartAnnotationModel annotation in this.Annotations)
            {
                annotation.Arrange(seriesRect);
            }

            this.EndUpdate(false);

            return rect;
        }

        protected override void ProcessZoomChanged()
        {
            foreach (var axis in this.FirstAxes)
            {
                axis.OnZoomChanged();
            }
            foreach (var axis in this.SecondAxes)
            {
                axis.OnZoomChanged();
            }

            base.ProcessZoomChanged();
        }

        protected override void ProcessPlotOriginChanged()
        {
            foreach (var axis in this.FirstAxes)
            {
                axis.OnPlotOriginChanged();
            }
            foreach (var axis in this.SecondAxes)
            {
                axis.OnPlotOriginChanged();
            }

            base.ProcessPlotOriginChanged();
        }

        protected void AttachAxis(AxisModel axis)
        {
            this.children.Add(axis);
            this.Invalidate(ChartAreaInvalidateFlags.All);
        }

        protected void DetachAxis(AxisModel axis)
        {
            this.children.Remove(axis);
            this.Invalidate(ChartAreaInvalidateFlags.All);
        }

        protected abstract RadRect ArrangeAxes(RadRect rect);

        private void UpdateAxes()
        {
            if (this.Series.Count == 0)
            {
                this.ResetAxes();
                return;
            }

            this.AssertSeriesAxisAssociation();

            ReferenceDictionary<AxisModel, List<ChartSeriesModel>> seriesByStackAxis = new ReferenceDictionary<AxisModel, List<ChartSeriesModel>>();
            ReferenceDictionary<AxisModel, List<ChartSeriesModel>> seriesByValueAxis = new ReferenceDictionary<AxisModel, List<ChartSeriesModel>>();

            foreach (ChartSeriesModel series in this.Series)
            {
                IPlotAreaElementModelWithAxes seriesWithAxes = series as IPlotAreaElementModelWithAxes;
                AxisModel firstAxis = seriesWithAxes.FirstAxis;
                AxisModel secondAxis = seriesWithAxes.SecondAxis;
                List<ChartSeriesModel> seriesModelsByFirstAxis;
                List<ChartSeriesModel> seriesModelsBySecondAxis;

                if (firstAxis.SupportsCombinedPlot)
                {
                    if (!seriesByStackAxis.TryGetValue(firstAxis, out seriesModelsByFirstAxis))
                    {
                        seriesModelsByFirstAxis = new List<ChartSeriesModel>();
                        seriesByStackAxis.Set(firstAxis, seriesModelsByFirstAxis);
                    }
                    seriesModelsByFirstAxis.Add(series);
                }
                else
                {
                    if (!seriesByValueAxis.TryGetValue(firstAxis, out seriesModelsByFirstAxis))
                    {
                        seriesModelsByFirstAxis = new List<ChartSeriesModel>();
                        seriesByValueAxis.Set(firstAxis, seriesModelsByFirstAxis);
                    }
                    seriesModelsByFirstAxis.Add(series);
                }

                if (secondAxis.SupportsCombinedPlot)
                {
                    if (!seriesByStackAxis.TryGetValue(secondAxis, out seriesModelsBySecondAxis))
                    {
                        seriesModelsBySecondAxis = new List<ChartSeriesModel>();
                        seriesByStackAxis.Set(secondAxis, seriesModelsBySecondAxis);
                    }
                    seriesModelsBySecondAxis.Add(series);
                }
                else
                {
                    if (!seriesByValueAxis.TryGetValue(secondAxis, out seriesModelsBySecondAxis))
                    {
                        seriesModelsBySecondAxis = new List<ChartSeriesModel>();
                        seriesByValueAxis.Set(secondAxis, seriesModelsBySecondAxis);
                    }
                    seriesModelsBySecondAxis.Add(series);
                }
            }

            // NOTE: All stack axes (i.e. their associated combine strategies) should be updated first 
            // as the stack value axes' ranges can be affected by multiple combine strategies.
            foreach (var pair in seriesByStackAxis)
            {
                this.UpdateCombineStrategy(pair.Key, pair.Value);
                this.UpdateAxis(pair.Key, pair.Value);
            }

            foreach (var pair in seriesByValueAxis)
            {
                this.UpdateAxis(pair.Key, pair.Value);
            }

            // Update primary axes if they are not associated with series
            if (this.primaryFirstAxis != null && !this.primaryFirstAxis.isUpdated)
            {
                this.UpdateAxis(this.primaryFirstAxis, null);
            }

            if (this.primarySecondAxis != null && !this.primarySecondAxis.isUpdated)
            {
                this.UpdateAxis(this.primarySecondAxis, null);
            }
        }

        private void ResetAxes()
        {
            foreach (AxisModel axis in this.FirstAxes)
            {
                axis.Reset();
            }
            foreach (AxisModel axis in this.SecondAxes)
            {
                axis.Reset();
            }
        }

        [Conditional("DEBUG")]
        private void AssertSeriesAxisAssociation()
        {
            foreach (IPlotAreaElementModelWithAxes series in this.Series)
            {
                Debug.Assert(series.FirstAxis != null, "We should have first axis!");
                Debug.Assert(series.SecondAxis != null, "We should have second axis!");
            }
        }

        private void UpdateCombineStrategy(AxisModel stackAxis, IList<ChartSeriesModel> series)
        {
            ChartSeriesCombineStrategy strategy;
            if (!this.SeriesCombineStrategies.TryGetValue(stackAxis, out strategy))
            {
                strategy = new ChartSeriesCombineStrategy();
                this.SeriesCombineStrategies.Set(stackAxis, strategy);
            }

            strategy.Update(series, stackAxis);
        }

        private void UpdateAxis(AxisModel axis, IList<ChartSeriesModel> series)
        {
            Debug.Assert(axis != null, "A series has not found any axes!");
            AxisUpdateContext context = new AxisUpdateContext(axis, series, this.SeriesCombineStrategies.EnumerateValues());

            axis.Update(context);

            bool plotInvalid = axis.isPlotValid;

            // plot points
            axis.Plot(context);

            if (axis.SupportsCombinedPlot && !plotInvalid && this.SeriesCombineStrategies.ContainsKey(axis))
            {
                this.SeriesCombineStrategies[axis].Plot();
            }
        }
    }
}