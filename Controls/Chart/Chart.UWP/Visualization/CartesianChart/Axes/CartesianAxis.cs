using System;
using System.Linq;
using Telerik.Charting;
using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents an axis used to plot points within a <see cref="RadCartesianChart"/> instance.
    /// </summary>
    public abstract class CartesianAxis : LineAxis
    {
        /// <summary>
        /// Identifies the <see cref="HorizontalLocation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalLocationProperty =
            DependencyProperty.Register(nameof(HorizontalLocation), typeof(AxisHorizontalLocation), typeof(CartesianAxis), new PropertyMetadata(AxisHorizontalLocation.Left, OnHorizontalLocationChanged));

        /// <summary>
        /// Identifies the <see cref="VerticalLocation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalLocationProperty =
            DependencyProperty.Register(nameof(VerticalLocation), typeof(AxisVerticalLocation), typeof(CartesianAxis), new PropertyMetadata(AxisVerticalLocation.Bottom, OnVerticalLocationChanged));

        internal int linkedSeriesCount;

        /// <summary>
        /// Gets or sets the horizontal location of the axis in relation to the plot area.
        /// </summary>
        /// <value>The horizontal location.</value>
        public AxisHorizontalLocation HorizontalLocation
        {
            get
            {
                return (AxisHorizontalLocation)this.GetValue(HorizontalLocationProperty);
            }
            set
            {
                this.SetValue(HorizontalLocationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical location of the axis in relation to the plot area.
        /// </summary>
        /// <value>The vertical location.</value>
        public AxisVerticalLocation VerticalLocation
        {
            get
            {
                return (AxisVerticalLocation)this.GetValue(VerticalLocationProperty);
            }
            set
            {
                this.SetValue(VerticalLocationProperty, value);
            }
        }

        internal override void UpdateAxisLine(ChartLayoutContext context)
        {
            if (this.drawWithComposition && this.lineContainer != null)
            {
                this.chart.ContainerVisualsFactory.PrepareCartesianAxisLineVisual(this, this.lineContainer, this.model.layoutSlot, this.type);
            }
            else
            {
                double antiAliasOffset = this.model.LineThickness % 2 == 1 ? 0.5 : 0;

                // update line points
                if (this.type == AxisType.First)
                {
                    this.line.X1 = this.model.layoutSlot.X;
                    this.line.X2 = this.model.layoutSlot.Right;
                    if (this.model.VerticalLocation == AxisVerticalLocation.Bottom)
                    {
                        this.line.Y1 = this.line.Y2 = this.model.layoutSlot.Y - antiAliasOffset;
                    }
                    else
                    {
                        this.line.Y1 = this.line.Y2 = this.model.layoutSlot.Bottom - antiAliasOffset;
                    }
                }
                else
                {
                    this.line.Y1 = this.model.layoutSlot.Y;
                    this.line.Y2 = this.model.layoutSlot.Bottom;

                    if (this.model.HorizontalLocation == AxisHorizontalLocation.Left)
                    {
                        this.line.X1 = this.line.X2 = this.model.layoutSlot.Right + antiAliasOffset;
                    }
                    else
                    {
                        this.line.X1 = this.line.X2 = this.model.layoutSlot.X + antiAliasOffset;
                    }
                }
            }
          
            base.UpdateAxisLine(context);
        }

        private static void OnHorizontalLocationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Axis presenter = sender as Axis;
            presenter.model.HorizontalLocation = (AxisHorizontalLocation)e.NewValue;
        }

        private static void OnVerticalLocationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Axis presenter = sender as Axis;
            presenter.model.VerticalLocation = (AxisVerticalLocation)e.NewValue;
        }
    }
}
