using System;
using Telerik.Charting;
using Telerik.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Base class for all <see cref="PresenterBase"/> instances that visualize a logical chart element.
    /// </summary>
    [TemplatePart(Name = "PART_RenderSurface", Type = typeof(Canvas))]
    public abstract class ChartElementPresenter : PresenterBase
    {
        internal RadChartBase chart;
        private int userZIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartElementPresenter"/> class.
        /// </summary>
        protected ChartElementPresenter()
        {
            this.DefaultStyleKey = typeof(ChartElementPresenter);
        }

        /// <summary>
        /// Gets or sets the Z-index of this series. Useful when adjusting the appearance of multiple series.
        /// </summary>
        public int ZIndex
        {
            get
            {
                return this.userZIndex;
            }
            set
            {
                this.userZIndex = value;

                if (this.renderSurface != null)
                {
                    Canvas.SetZIndex(this, this.DefaultZIndex + this.userZIndex);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="RadChartBase"/> instance to which this <see cref="ChartElementPresenter"/>
        /// belongs.
        /// </summary>
        public RadChartBase Chart
        {
            get
            {
                return this.chart;
            }
        }

        internal abstract Element Element
        {
            get;
        }

        internal abstract int DefaultZIndex
        {
            get;
        }

        internal void Attach(RadChartBase chartBase)
        {
            this.chart = chartBase;
            this.drawWithComposition = this.chart.ContainerVisualsFactory.CanDrawContainerVisual(this);
            this.Element.presenter = this;
            this.OnAttached();

            Canvas.SetZIndex(this, this.DefaultZIndex + this.userZIndex);
        }

        internal void Detach()
        {
            RadChartBase oldChart = this.chart;
            this.chart = null;
            this.Element.presenter = null;
            this.OnDetached(oldChart);

            // invalidate the layout context and isPalette applied flag
            this.InvalidateCore();
        }

        internal override void InvalidateCore()
        {
            base.InvalidateCore();

            if (this.chart != null)
            {
                this.chart.InvalidateCore();
            }
        }

        internal override bool CanApplyPalette()
        {
            return this.chart != null && this.chart.paletteCache != null && base.CanApplyPalette();
        }

        /// <summary>
        /// Occurs when the presenter has been successfully attached to its owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected virtual void OnAttached()
        {
        }

        /// <summary>
        /// Occurs when the presenter has been successfully detached from its owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected virtual void OnDetached(RadChartBase oldChart)
        {
        }
    }
}
