using Telerik.Charting;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// This is the base class for all chart annotations.
    /// </summary>
    public abstract class ChartAnnotation : ChartElementPresenter
    {
        /// <summary>
        /// Identifies the <see cref="ClipToPlotArea"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ClipToPlotAreaProperty =
            DependencyProperty.Register(nameof(ClipToPlotArea), typeof(bool), typeof(ChartAnnotation), new PropertyMetadata(true));

        private bool isInvalidateRequired = false;

        /// <summary>
        /// Gets or sets a value indicating whether this instance will be clipped to the bounds of the plot area.
        /// </summary>
        public bool ClipToPlotArea
        {
            get
            {
                return (bool)this.GetValue(ClipToPlotAreaProperty);
            }
            set
            {
                this.SetValue(ClipToPlotAreaProperty, value);
            }
        }

        internal override int DefaultZIndex
        {
            get
            {
                return RadChartBase.AnnotationZIndex + this.Model.Index;
            }
        }

        internal abstract ChartAnnotationModel Model { get; }

        internal override Element Element
        {
            get
            {
                return this.Model;
            }
        }

        internal abstract void SetPresenterVisibility(Visibility annotationVisibility);

        internal abstract Visibility GetPresenterVisibility();

        internal override void UpdateUICore(ChartLayoutContext context)
        {
            base.UpdateUICore(context);

            this.Update(context);
        }

        internal override void OnPlotOriginChanged(ChartLayoutContext context)
        {
            base.OnPlotOriginChanged(context);

            this.Update(context);
        }

        internal void Update(ChartLayoutContext context)
        {
            if (this.renderSurface == null)
            {
                this.isInvalidateRequired = true;
                return;
            }

            this.UpdateVisibility();
            if (this.Visibility == Visibility.Collapsed)
            {
                return;
            }

            if (this.Model.IsUpdated)
            {
                this.UpdatePresenters();
            }
            this.UpdateClip(context);
        }

        internal virtual void UpdatePresenters()
        {
        }

        internal abstract void UpdateVisibility();

        internal virtual void UpdateClip(ChartLayoutContext context)
        {
            if (this.renderSurface == null)
            {
                return;
            }

            if (this.ClipToPlotArea)
            {
                RadRect clipArea = this.chart.chartArea.plotArea.layoutSlot;
                RectangleGeometry clip = new RectangleGeometry();
                clip.Rect = new Rect(clipArea.X, clipArea.Y, clipArea.Width, clipArea.Height);
                this.renderSurface.Clip = clip;
            }
            else
            {
                this.renderSurface.Clip = null;
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.isInvalidateRequired)
            {
                this.InvalidateCore();
                this.isInvalidateRequired = false;
            }
        }

        /// <summary>
        /// Occurs when the presenter has been successfully attached to its owning <see cref="RadChartBase" /> instance.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            (this.chart.chartArea as ChartAreaModelWithAxes).AddAnnotation(this.Model);
        }

        /// <summary>
        /// Occurs when the presenter has been successfully detached from its owning <see cref="RadChartBase" /> instance.
        /// </summary>
        protected override void OnDetached(RadChartBase oldChart)
        {
            base.OnDetached(oldChart);

            if (oldChart != null)
            {
                (oldChart.chartArea as ChartAreaModelWithAxes).RemoveAnnotation(this.Model);
            }
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ChartAnnotationAutomationPeer(this);
        }
    }
}