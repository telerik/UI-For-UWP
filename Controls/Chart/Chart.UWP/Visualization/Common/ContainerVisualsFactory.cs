using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Telerik.Charting;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// This factory helps preparing the ContainerVisual that will be used for the rendering of the Chart control.
    /// It creates new <see cref="ContainerVisual"/> instances and position them on the Chart's Canvas.
    /// </summary>
    public class ContainerVisualsFactory
    {
        private static readonly Color telerikChartAxisBorderBrushLightColor = Color.FromArgb(0x30, 0, 0, 0);
        private static readonly Color telerikChartAxisBorderBrushDarkColor = Color.FromArgb(0x59, 0xFF, 0xFF, 0xFF);

        private readonly SolidColorBrush telerikChartAxisBorderBrush = new SolidColorBrush(telerikChartAxisBorderBrushLightColor);
        private readonly SolidColorBrush telerikChartStrokeBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x1E, 0x98, 0xE4));
        private readonly SolidColorBrush ohlcBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x60, 0xC2, 0xFF));

        private DoubleCollection dashArrayCache;

        internal async void SetCompositionColorBrush(ContainerVisual containerVisual, Brush brush, bool isInternallyChanged = false)
        {
            var spriteVisual = containerVisual as SpriteVisual;
            if (spriteVisual != null)
            {
                var solidColorBrush = brush as SolidColorBrush;
                if (solidColorBrush == null && spriteVisual.Brush != null)
                {
                    spriteVisual.Brush = null;
                }
                else if (solidColorBrush != null)
                {
                    await brush.Dispatcher.RunAsync(
                         Windows.UI.Core.CoreDispatcherPriority.Normal,
                         () =>
                         {
                             var compositionColorBrush = spriteVisual.Brush as CompositionColorBrush;
                             if (compositionColorBrush == null)
                             {
                                 spriteVisual.Brush = spriteVisual.Compositor.CreateColorBrush(solidColorBrush.Color);
                                 spriteVisual.Opacity = (float)solidColorBrush.Opacity;
                             }
                             else if (compositionColorBrush != null && compositionColorBrush.Color != solidColorBrush.Color && isInternallyChanged)
                             {
                                 spriteVisual.Brush = spriteVisual.Compositor.CreateColorBrush(solidColorBrush.Color);
                                 spriteVisual.Opacity = (float)solidColorBrush.Opacity;
                             }
                         });
                }
            }
        }

        /// <summary>
        /// Indicates whether the visual can be drawn using the Composition API.
        /// </summary>
        /// <param name="visualElement">The visual element that needs to be drawn.</param>
        /// <returns>Return true if the visual element does not have set Styles and Templates - returns false if it has.</returns>
        protected internal virtual bool CanDrawContainerVisual(PresenterBase visualElement)
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                return false;
            }

            var axis = visualElement as Axis;
            if (axis != null)
            {
                if (axis is CartesianAxis)
                {
                    return axis.ReadLocalValue(Axis.LineStyleProperty) == DependencyProperty.UnsetValue
                        && axis.ReadLocalValue(Axis.MajorTickTemplateProperty) == DependencyProperty.UnsetValue
                        && axis.ReadLocalValue(Axis.MajorTickStyleProperty) == DependencyProperty.UnsetValue;
                }

                return false;
            }

            var pointTemplateSeries = visualElement as PointTemplateSeries;
            if (pointTemplateSeries != null)
            {
                if (pointTemplateSeries is AreaSeries || pointTemplateSeries is PointSeries || pointTemplateSeries is SplineSeries
                    || pointTemplateSeries is ScatterSplineSeries || pointTemplateSeries is PolarSeries || (pointTemplateSeries.GetType() == typeof(ScatterPointSeries)))
                {
                    return false;
                }

                return pointTemplateSeries.ReadLocalValue(PointTemplateSeries.DefaultVisualStyleProperty) == DependencyProperty.UnsetValue
                   && pointTemplateSeries.ReadLocalValue(PointTemplateSeries.PointTemplateProperty) == DependencyProperty.UnsetValue
                   && pointTemplateSeries.ReadLocalValue(PointTemplateSeries.PointTemplateSelectorProperty) == DependencyProperty.UnsetValue;
            }

            var cartesianChartGrid = visualElement as CartesianChartGrid;
            if (cartesianChartGrid != null)
            {
                return cartesianChartGrid.ReadLocalValue(CartesianChartGrid.MajorXLineStyleProperty) == DependencyProperty.UnsetValue
                   && cartesianChartGrid.ReadLocalValue(CartesianChartGrid.MajorYLineStyleProperty) == DependencyProperty.UnsetValue;
            }

            var macdhIndicator = visualElement as MacdhIndicator;
            if (macdhIndicator != null)
            {
                return macdhIndicator.ReadLocalValue(MacdhIndicator.PointTemplateProperty) == DependencyProperty.UnsetValue
                   && macdhIndicator.ReadLocalValue(MacdhIndicator.PointTemplateSelectorProperty) == DependencyProperty.UnsetValue
                   && macdhIndicator.ReadLocalValue(BarIndicatorBase.DefaultVisualStyleProperty) == DependencyProperty.UnsetValue;
            }

            return true;
        }

        /// <summary>
        /// Creates a new <see cref="ContainerVisual"/> instance.
        /// </summary>
        /// <param name="compositor">The <see cref="Compositor"/> that creates the visual objects.</param>
        /// <param name="elementType">The <see cref="Type"/> of the element for which <see cref="ContainerVisual"/> is created.</param>
        /// <returns>A new instance of the <see cref="SpriteVisual"/> class.</returns>
        protected internal virtual ContainerVisual CreateContainerVisual(Compositor compositor, Type elementType)
        {
            return compositor.CreateSpriteVisual();
        }

        /// <summary>
        /// Prepares the <see cref="ContainerVisual"/> used for the visualization of the ticks by setting its Size, Color and Offset.
        /// </summary>
        /// <param name="containerVisual">The <see cref="ContainerVisual"/> that is used by the Composition API.</param>
        /// <param name="layoutSlot"> A Rectangle in the Euclidean plane geometry used for the Size and Offset of the <see cref="ContainerVisual"/>.</param>
        protected internal virtual ContainerVisual PrepareTickVisual(ContainerVisual containerVisual, RadRect layoutSlot)
        {
            if (containerVisual.Size.X != layoutSlot.Width || containerVisual.Size.Y != layoutSlot.Height)
            {
                containerVisual.Size = new Vector2((float)layoutSlot.Width, (float)layoutSlot.Height);
            }

            this.ChangeBrushesAccordingToAppTheme();
            this.SetCompositionColorBrush(containerVisual, this.telerikChartAxisBorderBrush);
            containerVisual.Offset = new Vector3((float)layoutSlot.Location.X, (float)layoutSlot.Location.Y, 0);

            return containerVisual;
        }

        /// <summary>
        /// Prepares the <see cref="ContainerVisual"/> used for the visualization of the lines of the CartesianChartGrid by setting its Size, Color and Offset.
        /// </summary>
        /// <param name="containerVisual">The <see cref="ContainerVisual"/> that is used by the Composition API.</param>
        /// <param name="layoutSlot"> A Rectangle in the Euclidean plane geometry used for the Size and Offset of the <see cref="ContainerVisual"/>.</param>
        /// <param name="orientation"> The orientation of the CharGrid's line.</param>
        /// <param name="dashArray"> An array of doubles used for positioning of the dashes of the CharGrid's line.</param>
        protected internal virtual ContainerVisual PrepareCartesianChartGridLineVisual(ContainerVisual containerVisual, RadRect layoutSlot, Orientation orientation, DoubleCollection dashArray)
        {
            if (orientation == Orientation.Horizontal)
            {
                containerVisual.Offset = new Vector3((float)layoutSlot.X, (float)layoutSlot.Bottom, 0);
                if (containerVisual.Size.X != layoutSlot.Width || containerVisual.Size.Y != 1)
                {
                    containerVisual.Size = new Vector2((float)layoutSlot.Width, 1);

                    if (this.dashArrayCache != null)
                    {
                        var oldDashesSize = containerVisual.Children.Count * (this.dashArrayCache[0] + this.dashArrayCache[1]);
                        if (containerVisual.Children.Count > 0 && oldDashesSize < layoutSlot.Width)
                        {
                            this.ArrangeChartGridDashes(containerVisual, orientation, dashArray, (int)oldDashesSize);
                        }
                    }
                }
            }
            else
            {
                containerVisual.Offset = new Vector3((float)layoutSlot.X, (float)layoutSlot.Y, 0);
                if (containerVisual.Size.X != 1 || containerVisual.Size.Y != layoutSlot.Height)
                {
                    containerVisual.Size = new Vector2(1, (float)layoutSlot.Height);

                    if (this.dashArrayCache != null)
                    {
                        var oldDashesSize = containerVisual.Children.Count * (this.dashArrayCache[0] + this.dashArrayCache[1]);
                        if (containerVisual.Children.Count > 0 && oldDashesSize < layoutSlot.Height)
                        {
                            this.ArrangeChartGridDashes(containerVisual, orientation, dashArray, (int)oldDashesSize);
                        }
                    }
                }
            }

            if (dashArray[0] > 0 && dashArray[1] > 0)
            {
                if (containerVisual.Children.Count == 0)
                {
                    this.ArrangeChartGridDashes(containerVisual, orientation, dashArray);
                    this.dashArrayCache = dashArray;
                }
                else if (this.dashArrayCache != null && (this.dashArrayCache[0] != dashArray[0] || this.dashArrayCache[1] != dashArray[1]))
                {
                    containerVisual.Children.RemoveAll();
                    this.ArrangeChartGridDashes(containerVisual, orientation, dashArray);
                    this.dashArrayCache = dashArray;
                }
            }

            if (dashArray[0] > 0 && dashArray[1] == 0)
            {
                if (containerVisual.Children.Count > 0)
                {
                    containerVisual.Children.RemoveAll();
                }

                this.SetCompositionColorBrush(containerVisual, new SolidColorBrush(Color.FromArgb(0x23, 0, 0, 0)));
            }

            if (dashArray[0] == 0)
            {
                if (containerVisual.Children.Count > 0)
                {
                    containerVisual.Children.RemoveAll();
                }

                this.SetCompositionColorBrush(containerVisual, null);
            }

            return containerVisual;
        }

        /// <summary>
        /// Prepares the <see cref="ContainerVisual"/> used for the visualization of the <see cref="PointTemplateSeries"/> by setting its Size, Color and Offset.
        /// </summary>
        /// <param name="containerVisual">The <see cref="ContainerVisual"/> that is used by the <see cref="Compositor"/> API.</param>
        /// <param name="dataPoint"> The <see cref="DataPoint"/> used for the calculation of the Size and Offset of the <see cref="ContainerVisual"/>.</param>
        protected internal virtual ContainerVisual PreparePointTemplateSeriesVisual(ContainerVisual containerVisual, DataPoint dataPoint)
        {
            containerVisual.Offset = new Vector3((float)dataPoint.LayoutSlot.Location.X, (float)dataPoint.LayoutSlot.Location.Y, 0);
            if (containerVisual.Size.X != dataPoint.LayoutSlot.Width || containerVisual.Size.Y != dataPoint.LayoutSlot.Height)
            {
                containerVisual.Size = new Vector2((float)dataPoint.LayoutSlot.Width, (float)dataPoint.LayoutSlot.Height);
            }

            var ohlcDataPoint = dataPoint as OhlcDataPoint;
            if (ohlcDataPoint != null)
            {
                if (dataPoint.Presenter is CandlestickSeries)
                {
                    return this.CreateCandleStick(containerVisual, ohlcDataPoint);
                }

                if (dataPoint.Presenter is OhlcSeries)
                {
                    return this.CreateOhlcStick(containerVisual, ohlcDataPoint);
                }
            }

            this.SetCompositionColorBrush(containerVisual, this.telerikChartStrokeBrush);
            return containerVisual;
        }

        /// <summary>
        /// Prepares the <see cref="ContainerVisual"/> used for the visualization of the Axis's Lines by setting its Size, Color and Offset.
        /// </summary>
        /// <param name="axis">The <see cref="CartesianAxis"/> that will be visualized.</param>
        /// <param name="lineContainer">The <see cref="ContainerVisual"/> that is used by the Composition API.</param>
        /// <param name="layoutSlot">A Rectangle in the Euclidean plane geometry used for the Size and Offset of the <see cref="ContainerVisual"/>.</param>
        /// <param name="axisType">The <see cref="AxisType"/> of the Axis's line.</param>
        protected internal virtual ContainerVisual PrepareCartesianAxisLineVisual(CartesianAxis axis, ContainerVisual lineContainer, RadRect layoutSlot, AxisType axisType = AxisType.First)
        {
            if (axisType == AxisType.First)
            {
                float antiAliasOffset = axis.model.LineThickness % 2 == 1 ? 0.5f : 0;

                lineContainer.Offset = axis.VerticalLocation == AxisVerticalLocation.Bottom
                    ? new Vector3((float)layoutSlot.Location.X, (float)layoutSlot.Location.Y - antiAliasOffset, 0)
                    : new Vector3((float)layoutSlot.Location.X, (float)layoutSlot.Bottom - antiAliasOffset, 0);

                if (lineContainer.Size.X != layoutSlot.Width || lineContainer.Size.Y != ((float)axis.model.LineThickness - antiAliasOffset))
                {
                    lineContainer.Size = new Vector2((float)layoutSlot.Width, (float)axis.model.LineThickness - antiAliasOffset);
                }
            }
            else
            {
                lineContainer.Offset = axis.HorizontalLocation == AxisHorizontalLocation.Left
                    ? new Vector3((float)layoutSlot.Right, (float)layoutSlot.Location.Y, 0)
                    : new Vector3((float)layoutSlot.X, (float)layoutSlot.Location.Y, 0);

                if (lineContainer.Size.X != axis.model.LineThickness || lineContainer.Size.Y != layoutSlot.Height)
                {
                    lineContainer.Size = new Vector2((float)axis.model.LineThickness, (float)layoutSlot.Height);
                }
            }

            this.ChangeBrushesAccordingToAppTheme();
            this.SetCompositionColorBrush(lineContainer, this.telerikChartAxisBorderBrush);

            return lineContainer;
        }

        /// <summary>
        /// Prepares the <see cref="ContainerVisual"/> used for the visualization of the line indicators by setting its Size, Color and Offset.
        /// </summary>
        /// <param name="containerVisual">The <see cref="ContainerVisual"/> that is used by the <see cref="Compositor"/> API.</param>
        /// <param name="points"> The Point used for the calculation of the Size and Offset of the <see cref="ContainerVisual"/>.</param>
        /// <param name="stroke"> The stroke set to the <see cref="ContainerVisual"/> and used by its color brush.</param>
        /// <param name="strokeThickness"> The thickness of the stroke.</param>
        protected internal virtual ContainerVisual PrepareLineRenderVisual(ContainerVisual containerVisual, IEnumerable<Point> points, Brush stroke, double strokeThickness)
        {
            containerVisual.Children.RemoveAll();

            using (var enumerator = points.GetEnumerator())
            {
                var haveStartPoint = false;
                var startPoint = default(Point);

                while (enumerator.MoveNext())
                {
                    var endPoint = enumerator.Current;

                    if (!haveStartPoint)
                    {
                        startPoint = endPoint;
                        if (!enumerator.MoveNext())
                        {
                            break;
                        }

                        haveStartPoint = true;
                        endPoint = enumerator.Current;
                    }

                    // draw the line if we have both points
                    var childSpiteVisual = containerVisual.Compositor.CreateSpriteVisual();
                    var angle = Math.Atan2(endPoint.Y - startPoint.Y, endPoint.X - startPoint.X) * (180.0 / Math.PI);
                    var width = Math.Sqrt(Math.Pow(startPoint.X - endPoint.X, 2) + Math.Pow(startPoint.Y - endPoint.Y, 2));

                    childSpiteVisual.RotationAngleInDegrees = (float)angle;
                    childSpiteVisual.Offset = new Vector3((float)startPoint.X, (float)startPoint.Y, 0);
                    childSpiteVisual.Size = new Vector2((float)width, (float)strokeThickness);
                    this.SetCompositionColorBrush(childSpiteVisual, stroke);
                    containerVisual.Children.InsertAtBottom(childSpiteVisual);

                    startPoint = endPoint;
                }
            }

            return containerVisual;
        }

        /// <summary>
        /// Prepares the <see cref="ContainerVisual"/> used for the visualization of the <see cref="BarIndicatorBase"/> by setting its Size, Color and Offset.
        /// </summary>
        /// <param name="containerVisual">The <see cref="ContainerVisual"/> that is used by the <see cref="Compositor"/> API.</param>
        /// <param name="dataPoint"> The <see cref="DataPoint"/> used for the calculation of the Size and Offset of the <see cref="ContainerVisual"/>.</param>
        protected internal virtual ContainerVisual PrepareBarIndicatorVisual(ContainerVisual containerVisual, DataPoint dataPoint)
        {
            containerVisual.Offset = new Vector3((float)dataPoint.LayoutSlot.Location.X, (float)dataPoint.LayoutSlot.Location.Y, 0);

            if (containerVisual.Size.X != dataPoint.LayoutSlot.Width || containerVisual.Size.Y != dataPoint.LayoutSlot.Height)
            {
                containerVisual.Size = new System.Numerics.Vector2((float)dataPoint.LayoutSlot.Width, (float)dataPoint.LayoutSlot.Height);
            }

            this.SetCompositionColorBrush(containerVisual, this.telerikChartStrokeBrush);

            return containerVisual;
        }

        private ContainerVisual CreateCandleStick(ContainerVisual parentSpriteVisual, OhlcDataPoint ohlcDataPoint)
        {
            int thickness = 2;
            var numericalPlot = ohlcDataPoint.numericalPlot;

            var boxHeight = ohlcDataPoint.layoutSlot.Height;
            var boxWidth = ohlcDataPoint.layoutSlot.Width;

            double upperShadowMinValue = Math.Min(numericalPlot.PhysicalOpen, numericalPlot.PhysicalClose);
            double lowerShadowMaxValue = Math.Max(numericalPlot.PhysicalOpen, numericalPlot.PhysicalClose);
            double halfBoxWidth = (int)(0.5 * boxWidth);

            if (parentSpriteVisual.Children.Count == 0)
            {
                this.GenerateContainerVisualChildren(parentSpriteVisual, 8);
            }

            this.PositionOhlcDataPointVisual(parentSpriteVisual, 0, new Vector3((float)halfBoxWidth, (float)upperShadowMinValue, 0), new Vector2((float)(halfBoxWidth + thickness), thickness), this.ohlcBrush);
            this.PositionOhlcDataPointVisual(parentSpriteVisual, 1, new Vector3((float)boxWidth, (float)upperShadowMinValue, 0), new Vector2(thickness, (float)(lowerShadowMaxValue - upperShadowMinValue)), this.ohlcBrush);
            this.PositionOhlcDataPointVisual(parentSpriteVisual, 2, new Vector3((float)boxWidth, (float)lowerShadowMaxValue, 0), new Vector2((float)boxWidth, thickness), this.ohlcBrush, 180);
            this.PositionOhlcDataPointVisual(parentSpriteVisual, 3, new Vector3(0, (float)lowerShadowMaxValue, 0), new Vector2(thickness, (float)(lowerShadowMaxValue - upperShadowMinValue)), this.ohlcBrush, 180);
            this.PositionOhlcDataPointVisual(parentSpriteVisual, 4, new Vector3(0, (float)upperShadowMinValue, 0), new Vector2((float)halfBoxWidth, 2), this.ohlcBrush);
            this.PositionOhlcDataPointVisual(parentSpriteVisual, 5, new Vector3((float)halfBoxWidth, 0, 0), new Vector2(thickness, (float)upperShadowMinValue), this.ohlcBrush);
            this.PositionOhlcDataPointVisual(parentSpriteVisual, 6, new Vector3((float)halfBoxWidth, (float)lowerShadowMaxValue, 0), new Vector2(thickness, (float)boxHeight - (float)lowerShadowMaxValue), this.ohlcBrush);

            var fillBrush = ohlcDataPoint.IsFalling
                ? new SolidColorBrush(Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF))
                : this.telerikChartStrokeBrush;

            this.PositionOhlcDataPointVisual(parentSpriteVisual, 7, new Vector3(0, (float)upperShadowMinValue + thickness, 0), new Vector2((float)boxWidth, (float)(lowerShadowMaxValue - upperShadowMinValue - 4)), fillBrush);

            return parentSpriteVisual;
        }

        private ContainerVisual CreateOhlcStick(ContainerVisual parentSpriteVisual, OhlcDataPoint ohlcDataPoint)
        {
            int thickness = 2;
            var numericalPlot = ohlcDataPoint.numericalPlot;

            var boxHeight = ohlcDataPoint.layoutSlot.Height;
            var boxWidth = ohlcDataPoint.layoutSlot.Width;

            double leftTickValue = numericalPlot.PhysicalOpen;
            double rightTickValue = numericalPlot.PhysicalClose;
            double halfBoxWidth = (int)(0.5 * boxWidth);

            if (parentSpriteVisual.Children.Count == 0)
            {
                this.GenerateContainerVisualChildren(parentSpriteVisual, 3);
            }

            var opacity = ohlcDataPoint.IsFalling ? 0.5 : 1;
            this.ohlcBrush.Opacity = opacity;

            this.PositionOhlcDataPointVisual(parentSpriteVisual, 0, new Vector3((float)halfBoxWidth, 0, 0), new Vector2(thickness, (float)boxHeight), this.ohlcBrush);
            this.PositionOhlcDataPointVisual(parentSpriteVisual, 1, new Vector3(0, (float)leftTickValue, 0), new Vector2((float)halfBoxWidth, thickness), this.ohlcBrush);
            this.PositionOhlcDataPointVisual(parentSpriteVisual, 2, new Vector3((float)halfBoxWidth, (float)rightTickValue, 0), new Vector2((float)halfBoxWidth, thickness), this.ohlcBrush);

            return parentSpriteVisual;
        }

        private void GenerateContainerVisualChildren(ContainerVisual parentVisual, int numOfChildren)
        {
            for (int i = 0; i < numOfChildren; i++)
            {
                parentVisual.Children.InsertAtTop(parentVisual.Compositor.CreateSpriteVisual());
            }
        }

        private void PositionOhlcDataPointVisual(ContainerVisual parentVisual, int index, Vector3 offset, Vector2 size, SolidColorBrush brush, float angle = 0)
        {
            ContainerVisual childVisual = parentVisual.Children.ElementAt(index) as ContainerVisual;
            if (childVisual != null)
            {
                childVisual.Offset = offset;
                childVisual.Size = size;
                childVisual.RotationAngleInDegrees = angle;
                this.SetCompositionColorBrush(childVisual, brush);
            }
        }

        private void ArrangeChartGridDashes(ContainerVisual line, Orientation orientation, DoubleCollection visualDashArray, double startValue = 0)
        {
            int index = startValue > 0 ? line.Children.Count : 0;
            var conditionValue = orientation == Orientation.Horizontal ? line.Size.X : line.Size.Y;

            while (startValue < conditionValue)
            {
                var dashContainerVisual = this.GetDashContainerVisual(line, orientation, visualDashArray, index);
                if (orientation == Orientation.Horizontal)
                {
                    dashContainerVisual.Offset = new Vector3((float)startValue, 0, 0);
                }
                else
                {
                    dashContainerVisual.Offset = new Vector3(0, (float)startValue, 0);
                }

                index++;
                startValue += visualDashArray[0] + visualDashArray[1];
            }

            while (index < line.Children.Count)
            {
                line.Children.ElementAt(index).IsVisible = false;
                index++;
            }
        }

        private ContainerVisual GetDashContainerVisual(ContainerVisual parentVisual, Orientation orientation, DoubleCollection visualDashArray, int childIndex)
        {
            ContainerVisual childVisual;
            if (childIndex < parentVisual.Children.Count)
            {
                childVisual = parentVisual.Children.ElementAtOrDefault(childIndex) as SpriteVisual;
                if (childVisual != null && !childVisual.IsVisible)
                {
                    childVisual.IsVisible = true;
                }

                return childVisual;
            }

            childVisual = parentVisual.Compositor.CreateSpriteVisual();
            if (orientation == Orientation.Horizontal)
            {
                childVisual.Size = new Vector2((float)visualDashArray[0], parentVisual.Size.Y);
            }
            else
            {
                childVisual.Size = new Vector2(parentVisual.Size.X, (float)visualDashArray[0]);
            }

            this.SetCompositionColorBrush(childVisual, this.telerikChartAxisBorderBrush);
            parentVisual.Children.InsertAtBottom(childVisual);

            return childVisual;
        }

        private void ChangeBrushesAccordingToAppTheme()
        {
            var windowContent = Window.Current.Content as FrameworkElement;
            if (windowContent != null)
            {
                if (windowContent.RequestedTheme == ElementTheme.Light || windowContent.RequestedTheme == ElementTheme.Default)
                {
                    if (this.telerikChartAxisBorderBrush.Color != telerikChartAxisBorderBrushLightColor)
                    {
                        this.telerikChartAxisBorderBrush.Color = telerikChartAxisBorderBrushLightColor;
                    }
                }
                else
                {
                    if (this.telerikChartAxisBorderBrush.Color != telerikChartAxisBorderBrushDarkColor)
                    {
                        this.telerikChartAxisBorderBrush.Color = telerikChartAxisBorderBrushDarkColor;
                    }
                }
            }
        }
    }
}
