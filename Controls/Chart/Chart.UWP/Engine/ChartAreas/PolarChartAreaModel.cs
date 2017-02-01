using System;
using System.Collections.Generic;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class PolarChartAreaModel : ChartAreaModelWithAxes
    {
        internal const string NoPolarAxisKey = "NoPolarAxis";
        internal const string NoAngleAxisKey = "NoAngleAxis";

        internal static int RotateRadialLabelsPropertyKey = PropertyKeys.Register(typeof(PolarChartAreaModel), "RotateRadialLabels", ChartAreaInvalidateFlags.All);
        internal static int StartAnglePropertyKey = PropertyKeys.Register(typeof(PolarChartAreaModel), "StartAngle", ChartAreaInvalidateFlags.All);

        private bool rotateLabels;
        private double startAngle;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolarChartAreaModel"/> class.
        /// </summary>
        public PolarChartAreaModel()
        {
            this.TrackPropertyChanged = true;
        }

        /// <summary>
        /// Gets or sets the angle, measured counter-clockwise at which the PolarAxis is anchored.
        /// </summary>
        public double StartAngle
        {
            get
            {
                return this.startAngle;
            }
            set
            {
                this.SetValue(StartAnglePropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the radius axis of the polar coordinate system.
        /// </summary>
        public PolarAxisModel PolarAxis
        {
            get
            {
                return this.primaryFirstAxis as PolarAxisModel;
            }
            set
            {
                if (this.primaryFirstAxis == value)
                {
                    return;
                }

                if (this.primaryFirstAxis != null)
                {
                    this.DetachAxis(this.primaryFirstAxis);
                }

                this.primaryFirstAxis = value;

                if (this.primaryFirstAxis != null)
                {
                    this.primaryFirstAxis.Type = AxisType.First;
                    this.AttachAxis(this.primaryFirstAxis);
                }
            }
        }

        /// <summary>
        /// Gets or sets the angle axis of the polar coordinate system.
        /// </summary>
        public AxisModel AngleAxis
        {
            get
            {
                return this.primarySecondAxis;
            }
            set
            {
                if (this.primarySecondAxis == value)
                {
                    return;
                }

                if (this.primarySecondAxis != null)
                {
                    this.DetachAxis(this.primarySecondAxis);
                }

                this.primarySecondAxis = value;

                if (this.primarySecondAxis != null)
                {
                    this.primarySecondAxis.Type = AxisType.Second;
                    this.AttachAxis(this.primarySecondAxis);
                }
            }
        }

        /// <summary>
        /// Normalizes the specified angle so that it reflects the counter-clockwise plot direction as well as the starting angle of the polar axis.
        /// </summary>
        public double NormalizeAngle(double angle)
        {
            // normalization uses the following formula: 360 - StartAngle - angle
            // rendering uses clockwise direction, we want to work in counter-clockwise direction, that is why we subtract the angle from 360
            double normalizedAngle = (this.AngleAxis != null && this.AngleAxis.IsInverse) ? this.startAngle + angle : 360 - this.startAngle - angle;
            normalizedAngle = normalizedAngle % 360;
            if (normalizedAngle < 0)
            {
                normalizedAngle += 360;
            }

            return normalizedAngle;
        }

        internal override Tuple<object, object> ConvertPointToData(RadPoint coordinates)
        {
            object polarValue = null;
            object radialValue = null;

            if (this.view != null)
            {
                RadRect plotArea = this.plotArea.layoutSlot;
                double panOffsetX = this.view.PlotOriginX * plotArea.Width;
                double panOffsetY = this.view.PlotOriginY * plotArea.Height;

                // todo: The plotArea.Width/Height are the width/height of the whole (zoomed) polar chart, not the visual part of it.
                RadRect plotAreaVirtualSize = new RadRect(plotArea.X, plotArea.Y, plotArea.Width, plotArea.Height);

                var polarCoordinates = RadMath.ToPolarCoordinates(new RadPoint(coordinates.X - panOffsetX, coordinates.Y - panOffsetY), plotAreaVirtualSize.Center, true);
                var radius = polarCoordinates.Item1;
                var angle = polarCoordinates.Item2;

                if (this.primaryFirstAxis != null && this.primaryFirstAxis.isUpdated)
                {
                    NumericalAxisModel polarAxis = this.primaryFirstAxis as NumericalAxisModel;
                    var polarAxisPlotArea = new RadRect(plotAreaVirtualSize.Center.X, plotAreaVirtualSize.Y, plotAreaVirtualSize.Width / 2, plotAreaVirtualSize.Height / 2);
                    polarValue = polarAxis.ConvertPhysicalUnitsToData(radius + polarAxisPlotArea.X, polarAxisPlotArea);
                }

                if (this.primarySecondAxis != null && this.primarySecondAxis.isUpdated)
                {
                    var actualAngle = this.primarySecondAxis.IsInverse ? (360 - angle) % 360 : angle;
                    
                    if (this.primarySecondAxis is CategoricalAxisModel)
                    {
                        CategoricalAxisModel radarAxis = this.primarySecondAxis as CategoricalRadialAxisModel;
                        radialValue = radarAxis.ConvertPhysicalUnitsToData(actualAngle, plotAreaVirtualSize);
                    }
                    else
                    {
                        radialValue = actualAngle;
                    }
                }
            }

            return new Tuple<object, object>(polarValue, radialValue);
        }

        internal override RadPoint ConvertDataToPoint(Tuple<object, object> data)
        {
            var pointRadius = double.NaN;
            if (this.primaryFirstAxis != null && this.primaryFirstAxis.isUpdated)
            {
                NumericalAxisPlotInfo plotInfo = this.primaryFirstAxis.CreatePlotInfo(data.Item1) as NumericalAxisPlotInfo;
                if (plotInfo != null)
                {
                    double radius = this.plotArea.layoutSlot.Width / 2;
                    pointRadius = plotInfo.NormalizedValue * radius;
                }
            }

            var pointAngle = double.NaN;
            if (this.primarySecondAxis != null && this.primarySecondAxis.isUpdated)
            {
                if (this.primarySecondAxis is CategoricalAxisModel)
                {
                    CategoricalAxisPlotInfo categoricalPlotInfo = this.primarySecondAxis.CreatePlotInfo(data.Item2) as CategoricalAxisPlotInfo;
                    if (categoricalPlotInfo != null)
                    {
                        pointAngle = categoricalPlotInfo.ConvertToAngle(this);
                    }
                }
                else
                {
                    NumericalAxisPlotInfo numericalPlotInfo = this.primarySecondAxis.CreatePlotInfo(data.Item2) as NumericalAxisPlotInfo;
                    if (numericalPlotInfo != null)
                    {
                        pointAngle = numericalPlotInfo.ConvertToAngle();
                    }
                }
            }

            return RadMath.GetArcPoint(pointAngle, this.plotArea.layoutSlot.Center, pointRadius);
        }

        internal override void ApplyLayoutRounding()
        {
            this.primaryFirstAxis.ApplyLayoutRounding();

            // ask each series to apply layout rounding
            foreach (ChartSeriesModel series in this.plotArea.series)
            {
                series.ApplyLayoutRounding();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Method is internal")]
        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            if (e.Key == RotateRadialLabelsPropertyKey)
            {
                this.rotateLabels = (bool)e.NewValue;
            }
            else if (e.Key == StartAnglePropertyKey)
            {
                this.startAngle = (double)e.NewValue;
            }

            base.OnPropertyChanged(e);
        }

        internal override IEnumerable<string> GetNotLoadedReasons()
        {
            if (this.primaryFirstAxis == null)
            {
                yield return NoPolarAxisKey;
            }

            if (this.primarySecondAxis == null)
            {
                yield return NoAngleAxisKey;
            }
        }

        internal override RadRect ArrangeOverride(RadRect rect)
        {
            rect.Width *= this.view.ZoomWidth;
            rect.Height *= this.view.ZoomHeight;

            return base.ArrangeOverride(rect);
        }

        protected override RadRect ArrangeAxes(RadRect rect)
        {
            IRadialAxis angleAxis = this.primarySecondAxis as IRadialAxis;
            if (angleAxis == null)
            {
                throw new InvalidOperationException("AngleAxis of a polar chart area must be radial.");
            }

            RadRect ellipseRect = RadRect.ToSquare(rect, false);
            ellipseRect = RadRect.CenterRect(ellipseRect, rect);
            RadSize ellipseSize = new RadSize(ellipseRect.Width, ellipseRect.Height);
            RadRect remaining = ellipseRect;

            // Measure the second (radial) axis first; it will inflate the plot area
            this.primarySecondAxis.Measure(ellipseSize);
            this.primarySecondAxis.Arrange(ellipseRect);

            RadThickness margins = this.primarySecondAxis.desiredMargin;
            remaining.X += margins.Left;
            remaining.Y += margins.Top;
            remaining.Width -= margins.Left + margins.Right;
            remaining.Height -= margins.Top + margins.Bottom;

            remaining = RadRect.ToSquare(remaining, false);
            remaining = RadRect.CenterRect(remaining, ellipseRect);

            this.primaryFirstAxis.Measure(ellipseSize);
            this.primaryFirstAxis.Arrange(remaining);

            return remaining;
        }
    }
}